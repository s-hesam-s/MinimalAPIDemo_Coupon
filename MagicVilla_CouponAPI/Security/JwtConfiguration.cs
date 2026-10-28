using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MagicVilla_CouponAPI.Security;

public static class JwtConfiguration
{
    public static string Issuer(IConfiguration configuration) => configuration["ApiSettings:Issuer"] ?? "magicvilla-coupon";
    public static string Audience(IConfiguration configuration) => configuration["ApiSettings:Audience"] ?? "magicvilla-coupon-client";

    public static IServiceCollection AddCouponAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var secret = configuration["ApiSettings:Secret"];
            if (string.IsNullOrWhiteSpace(secret) || Encoding.UTF8.GetByteCount(secret) < 32)
                throw new InvalidOperationException("Configure an API signing secret with at least 32 bytes.");
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true, ValidIssuer = Issuer(configuration),
                ValidateAudience = true, ValidAudience = Audience(configuration),
                ValidateLifetime = true, RequireExpirationTime = true,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }, ClockSkew = TimeSpan.FromSeconds(30)
            };
        });
        services.AddAuthorization(options => options.AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser().RequireRole("admin")));
        return services;
    }
}
