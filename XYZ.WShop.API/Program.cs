
using Coravel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using XYZ.WShop.API.Extensions;
using XYZ.WShop.API.SeedData;
using XYZ.WShop.Application.Extensions;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Extensions;
using XZY.WShop.Infrastructure.Jobs;

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
            builder.Services.AddInfrastructureServices<ApplicationDbContext>(builder.Configuration);
            builder.Services.ApiServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            DbInitializer.SeedData(app.Services, builder.Configuration);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
