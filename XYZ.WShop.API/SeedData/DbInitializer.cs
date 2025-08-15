using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XYZ.WShop.API.SeedData
{
    public static class DbInitializer
    {
        public static async void SeedData(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                if (!context.SubscriptionPlans.Any())
                {
                    var subPlans = new List<SubscriptionPlan> {
                       new SubscriptionPlan{
                       Amount = 0M,
                       Name = "Free Plan",
                       NumberOfDays = 7,
                       Description = "7 days free plan",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        },
                       new SubscriptionPlan{
                       Amount = 350M,
                       Name = "7 Days Plan",
                       NumberOfDays = 7,
                       Description = "7 days plan",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        },
                       new SubscriptionPlan{
                       Amount = 1400M,
                       Name = "1 Month Plan",
                       NumberOfDays = 30,
                       Description = "1 month plan",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        },
                       new SubscriptionPlan{
                       Amount = 4200M,
                       Name = "3 Months Plan",
                       NumberOfDays = 90,
                       Description = "3 months plan",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        },
                       new SubscriptionPlan{
                       Amount = 8500,
                       Name = "6 Months Plan",
                       NumberOfDays = 180,
                       Description = "6 months paln",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        },
                         new SubscriptionPlan{
                       Amount = 17000M,
                       Name = "1 Year Plan",
                       NumberOfDays = 360,
                       Description = "1 year plan",
                       IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        }
                    };


                    context.SubscriptionPlans.AddRange(subPlans);
                }

                context.SaveChanges();
            }
        }
    }
}
