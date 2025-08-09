using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using XZY.WShop.Infrastructure.Services.Identity;

namespace XYZ.WShop.API.Extensions
{
    public static class ConfigureAuthentication
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityToken token = configuration.GetSection("token").Get<IdentityToken>()!;
            byte[] secret = Encoding.ASCII.GetBytes(token!.Secret);

            services
                .AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtBearer(
                    options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.SaveToken = true;
                        options.ClaimsIssuer = token.Issuer;
                        options.IncludeErrorDetails = true;
                        options.Validate(JwtBearerDefaults.AuthenticationScheme);
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ClockSkew = TimeSpan.Zero,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = token.Issuer,
                                ValidAudience = token.Audience,
                                IssuerSigningKey = new SymmetricSecurityKey(secret),
                                NameClaimType = ClaimTypes.NameIdentifier,
                                RequireSignedTokens = true,
                                RequireExpirationTime = true
                            };
                    });

            return services;
        }
    }
}
