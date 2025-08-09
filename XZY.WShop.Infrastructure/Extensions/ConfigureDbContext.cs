using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZY.WShop.Infrastructure.Data.Interceptors;

namespace XZY.WShop.Infrastructure.Extensions
{
    public static class ConfigureDbContext
    {
        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) where TContext : DbContext
        {
            services.AddDbContextPool<TContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                  .AddInterceptors(new AuditingSaveChangesInterceptor(httpContextAccessor));
            });

            return services;
        }
    }
}
