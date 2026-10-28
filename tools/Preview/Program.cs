using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Endpoints;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository.IRepository;
using MagicVilla_CouponAPI.Security;
using MagicVilla_CouponAPI.Validations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using Moq;

var root = PreviewSetup.Root("MagicVilla_CouponAPI"); var builder = PreviewSetup.Builder(root, 8842);
var fixtureBuilder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
fixtureBuilder.Configuration.Sources.Clear(); fixtureBuilder.Logging.ClearProviders(); fixtureBuilder.WebHost.UseTestServer();
var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); fixtureBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["ApiSettings:Secret"] = key });
fixtureBuilder.Services.AddCouponAuthentication(fixtureBuilder.Configuration);
var repository = new Mock<ICouponRepository>(MockBehavior.Strict); var coupons = new List<Coupon> { new() { Id = 1, Name = "SAMPLE10", Percent = 10, IsActive = true } };
repository.Setup(r => r.GetAllAsync()).ReturnsAsync(coupons); repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((int id) => coupons.SingleOrDefault(c => c.Id == id)!);
fixtureBuilder.Services.AddSingleton(repository.Object); fixtureBuilder.Services.AddSingleton<MappingConfig>();
fixtureBuilder.Services.AddSingleton<IValidator<CouponCreateDTO>, CouponCreateValidation>(); fixtureBuilder.Services.AddSingleton<IValidator<CouponUpdateDTO>, CouponUpdateValidation>();
await using var fixture = fixtureBuilder.Build(); fixture.Use(async (c, next) => { if (!HttpMethods.IsGet(c.Request.Method)) { c.Response.StatusCode = 405; return; } await next(); });
fixture.UseAuthentication(); fixture.UseAuthorization(); fixture.ConfigureCouponEndpoints(); await fixture.StartAsync();
var client = fixture.GetTestClient();
string token(string role) => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(JwtConfiguration.Issuer(fixtureBuilder.Configuration), JwtConfiguration.Audience(fixtureBuilder.Configuration), new[] { new Claim(ClaimTypes.NameIdentifier, "fixture-user"), new Claim(ClaimTypes.Role, role) }, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(10), new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)));
async Task<object> probe(string label, string path, string? role)
{ using var request = new HttpRequestMessage(HttpMethod.Get, path); if (role is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token(role)); using var response = await client.SendAsync(request); var body = await response.Content.ReadAsStringAsync(); return new { scenario = label, method = "GET", path, status = (int)response.StatusCode, body }; }
var evidence = new[] { await probe("Anonymous", "/api/coupon", null), await probe("Customer", "/api/coupon", "customer"), await probe("Admin", "/api/coupon", "admin"), await probe("Endpoint validation", "/api/coupon/0", "admin") };
var app = builder.Build(); PreviewSetup.Guard(app); app.MapGet("/evidence.json", () => evidence); app.MapGet("/", () => Results.Content(EvidencePage.Html("Minimal API · access boundaries", "Real registered coupon endpoints and JWT policy execute in an in-process HTTP host. The repository is a strict fixture; writes are blocked. Signing material is ephemeral and never displayed.", evidence), "text/html")); await app.RunAsync();
