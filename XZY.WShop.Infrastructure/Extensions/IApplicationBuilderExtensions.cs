using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Helpers;

namespace XZY.WShop.Infrastructure.Extensions
{
    /// <summary>
    ///     Extension methods for IApplicationBuilder. 
    /// </summary>
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

            if (dbContext.Database.GetPendingMigrations().Any() && dbContext.Database.IsNpgsql())
            {
                await dbContext.Database.MigrateAsync();
            }
        }

        public static void SeedData(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;

            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            // DbInitializer.SeedData(dbContext);
        }

        public static void UseGetCurrentLoginUser(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
