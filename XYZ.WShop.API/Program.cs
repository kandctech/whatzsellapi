
using Coravel;
using System.Net.Http.Headers;
using XYZ.WShop.API.Config;
using XYZ.WShop.API.Extensions;
using XYZ.WShop.API.SeedData;
using XYZ.WShop.Application.Extensions;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Extensions;
using XZY.WShop.Infrastructure.Jobs;
using Polly;
using Microsoft.EntityFrameworkCore;

namespace XYZ.WShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseSentry();


            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add services to the container.
            builder.Services.AddScheduler();
            builder.Services.AddQueue();
            builder.Services.AddTransient<FollowUpReminderJob>();
            builder.Services.AddTransient<SubscriptionJob>();
            builder.Services.AddTransient<SubscriptionExpriarionReminderJob>();
            builder.Services.AddInfrastructureServices<ApplicationDbContext>(builder.Configuration);
            builder.Services.ApiServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            var appSettingsSection = builder.Configuration.GetSection("AppSettings");
            builder.Services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();


            builder.Services.AddHttpClient("PayStack", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["PayoutAPI:Paystack:BaseUrl"]);
                var useLiveKey = builder.Configuration["PayoutAPI:Paystack:UseLive"];

                var apiKey = bool.Parse(useLiveKey) ? builder.Configuration["PayoutAPI:Paystack:LiveKeys:Secretkey"] : builder.Configuration["PayoutAPI:Paystack:TestKeys:Secretkey"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Check for pending migrations
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    dbContext.Database.Migrate(); // Applies all pending migrations
                    Console.WriteLine("Migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("No pending migrations found.");
                }
            }

            //DbInitializer.SeedData(app.Services, builder.Configuration);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            //  app.InitialiseDb().Wait();
            // app.SeedData();

            // app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            //app.UseRouting();
            //app.UseGetCurrentLoginUser();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/api/health");
            app.MapControllers();
            //schedule background jobs with coravel
            var provider = app.Services;
            Schedule.ScheduleJobs(provider);

            app.Run();
        }
    }
}
