using System.Text.Json.Serialization;

namespace XYZ.WShop.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     authentication configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection ApiServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("CorsAllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddAuthentication(configuration);
            services.AddAuthorization();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddSwagerGenUI();
            services.AddHealthChecks();
            return services;
        }
    }
}
