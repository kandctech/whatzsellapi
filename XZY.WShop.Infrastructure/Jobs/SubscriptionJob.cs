using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Jobs
{
    public class SubscriptionJob: IInvocable
    {
        private readonly ILogger<SubscriptionJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionJob(ILogger<SubscriptionJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke()
        {
            await DeactivateExpireSubscriptionsAsync();
        }

        private async Task DeactivateExpireSubscriptionsAsync()
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var inactiveSubscriptions = await context.Subscriptions
                   .Where(s => s.EndDate > DateTime.UtcNow)
                   .OrderByDescending(s => s.EndDate)
                   .ToListAsync();

                foreach (var sub in inactiveSubscriptions)
                {
                    sub.IsActive = false;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
