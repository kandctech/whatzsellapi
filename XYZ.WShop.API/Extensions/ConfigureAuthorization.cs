using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace XYZ.WShop.API.Extensions
{
    public static class ConfigureAuthorization
    {
        public static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(
              options =>
              {
                  options.AddPolicy(
                      JwtBearerDefaults.AuthenticationScheme,
                      new AuthorizationPolicyBuilder()
                          .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                          .RequireAuthenticatedUser()
                          .Build());
              });

            return services;
        }
    }
}
