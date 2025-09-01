using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Data;
using XYZ.WShop.Application.Interfaces.Services.Identity;
using XZY.WShop.Infrastructure.Data.Repositories;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Exceptions;
using XZY.WShop.Infrastructure.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using XZY.WShop.Infrastructure.Services;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Infrastructure.Services;

namespace XZY.WShop.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.AddHttpContextAccessor();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            var serviceProvider = services.BuildServiceProvider();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            services.AddDbContext<ApplicationDbContext>(configuration, httpContextAccessor);
            services.AddIdentity();
            services.Configure<IdentityToken>(configuration.GetSection(nameof(IdentityToken.Token)));
            services.AddSingleton<IdentityToken>(sp => sp.GetRequiredService<IOptions<IdentityToken>>().Value);

            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILinkAnalyticService, LinkAnalyticService>();
            services.AddScoped<IFollowUpService, FollowUpService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();



            // Register Hosted services
            //services.AddHostedService<SMSHostedService>();



            return services;

        }
    }
}
