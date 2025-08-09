using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

namespace XYZ.WShop.API.Extensions
{
    public static class ConfigureSwagger
    {
        public static IServiceCollection AddSwagerGenUI(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWT Token Auth API",
                    Version = "v1",
                    TermsOfService = new Uri("https://xyx.laundry.com/tandc")
                });
                c.IgnoreObsoleteActions();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
{
    new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
        }
    },
    new string[] {}
}
});
            });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddFluentValidationRulesToSwagger();

            return services;
        }
    }
}
