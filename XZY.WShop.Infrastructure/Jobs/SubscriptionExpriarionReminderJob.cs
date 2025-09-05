using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Jobs
{
    public class SubscriptionExpriarionReminderJob : IInvocable
    {
        private readonly ILogger<SubscriptionExpriarionReminderJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionExpriarionReminderJob(ILogger<SubscriptionExpriarionReminderJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke()
        {
            await SendSubscriptionExpirationReminder();
        }

        private async Task SendSubscriptionExpirationReminder()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<ISubscriptionEmailService>();

                    // Get users with expiring subscriptions
                    var expiringSubscriptions = await subscriptionService.GetExpiringSubscriptionsAsync();

                    foreach (var subscription in expiringSubscriptions)
                    {
                        var daysUntilExpiry = (subscription.ExpiryDate - DateTime.UtcNow).Days;

                        if (daysUntilExpiry == 1 || daysUntilExpiry == 3 || daysUntilExpiry == 7)
                        {
                            await emailService.SendSubscriptionExpiryWarningAsync(
                                subscription.UserEmail,
                                subscription.UserName,
                                subscription.ExpiryDate,
                                daysUntilExpiry
                            );
                        }
                    }

                    // Get expired subscriptions
                    var expiredSubscriptions = await subscriptionService.GetExpiredSubscriptionsAsync();

                    foreach (var subscription in expiredSubscriptions)
                    {
                        await emailService.SendSubscriptionExpiredAsync(
                            subscription.UserEmail,
                            subscription.UserName,
                            subscription.ExpiryDate
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in subscription notification service");
            }
        }
    }
}
