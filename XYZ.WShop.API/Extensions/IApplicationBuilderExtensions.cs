using Microsoft.EntityFrameworkCore;
using System;
using XZY.WShop.Infrastructure.Data;

namespace XYZ.WShop.API.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        ///     Initialise Db
        /// </summary>
        /// <param name="builder"></param>
        public static async Task InitialiseDb(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;

            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Database.IsNpgsql())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
