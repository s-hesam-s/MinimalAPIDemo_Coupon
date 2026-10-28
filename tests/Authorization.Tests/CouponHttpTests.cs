using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Endpoints;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository.IRepository;
using MagicVilla_CouponAPI.Security;
using MagicVilla_CouponAPI.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Authorization.Tests;

public class CouponHttpTests
{
    private const string fixtureKey = "test-only-fixture-signing-key-not-a-credential-2026";

    [Theory]
    [InlineData("GET", "/api/coupon")]
    [InlineData("GET", "/api/coupon/1")]
    [InlineData("POST", "/api/coupon")]
    [InlineData("PUT", "/api/coupon")]
    [InlineData("DELETE", "/api/coupon/1")]
    public async Task AnonymousAndCustomerCannotAccessCouponEndpoints(string method, string path)
    {
        await using var app = await createApp();
        var client = app.GetTestClient();
        using var anonymous = await client.SendAsync(request(method, path));
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token("customer"));
        using var customer = await client.SendAsync(request(method, path));
        Assert.Equal(HttpStatusCode.Forbidden, customer.StatusCode);
    }

    [Theory]
    [InlineData("issuer")]
    [InlineData("audience")]
    [InlineData("expired")]
    [InlineData("signature")]
    public async Task InvalidAdminTokensAreRejected(string defect)
    {
        await using var app = await createApp();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token("admin", defect));
        using var response = await client.GetAsync("/api/coupon");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ValidAdminCanReadAndValidatedWritesReachHandler()
    {
        await using var app = await createApp();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token("admin"));
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/coupon")).StatusCode);
        // Empty name/invalid percent must reach validation, never persistence.
        Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(request("POST", "/api/coupon"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(request("PUT", "/api/coupon"))).StatusCode);
    }

    private static HttpRequestMessage request(string method, string path) => new(new HttpMethod(method), path)
    {
        Content = method is "POST" or "PUT" ? new StringContent("{\"id\":1,\"name\":\"\",\"percent\":0}", Encoding.UTF8, "application/json") : null
    };

    private static string token(string role, string defect = "")
    {
        var jwt = new JwtSecurityToken(defect == "issuer" ? "wrong" : "magicvilla-coupon",
            defect == "audience" ? "wrong" : "magicvilla-coupon-client",
            new[] { new Claim(ClaimTypes.NameIdentifier, "fixture-user"), new Claim(ClaimTypes.Role, role) },
            DateTime.UtcNow.AddHours(-1), defect == "expired" ? DateTime.UtcNow.AddMinutes(-5) : DateTime.UtcNow.AddMinutes(5),
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(defect == "signature" ? fixtureKey + "wrong" : fixtureKey)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static async Task<WebApplication> createApp()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> { ["ApiSettings:Secret"] = fixtureKey });
        builder.WebHost.UseTestServer();
        builder.Services.AddCouponAuthentication(builder.Configuration);
        builder.Services.AddSingleton(new Mock<ICouponRepository>().Object);
        builder.Services.AddSingleton<MappingConfig>();
        builder.Services.AddSingleton<IValidator<CouponCreateDTO>, CouponCreateValidation>();
        builder.Services.AddSingleton<IValidator<CouponUpdateDTO>, CouponUpdateValidation>();
        var app = builder.Build();
        app.UseAuthentication(); app.UseAuthorization();
        app.ConfigureCouponEndpoints();
        await app.StartAsync();
        return app;
    }
}
