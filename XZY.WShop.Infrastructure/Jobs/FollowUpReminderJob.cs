using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Jobs
{
    public class FollowUpReminderJob : IInvocable
    {
        private readonly ILogger<FollowUpReminderJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPushNotificationService _pushNotificationService;

        public FollowUpReminderJob(ILogger<FollowUpReminderJob> logger, IPushNotificationService pushNotificationService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _pushNotificationService = pushNotificationService;
        }

        public async Task Invoke()
        {
            try
            {
                await SendReminderReminderAsync();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private async Task SendReminderReminderAsync()
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var pushNotificationService = scope.ServiceProvider.GetService<IPushNotificationService>();
                var now = DateTime.UtcNow;
                var dueFollowUps = context.FollowUps
                    .Where(f => f.Reminder &&
                               f.Status == "pending" &&
                               f.Date <= now.AddMinutes(15) && // Due within next 15 mins
                               f.Date >= now)
                    .ToList();

                foreach (var followUp in dueFollowUps)
                {
                    var deviceToken = await context.Users
                        .Where(d => d.Id == followUp.CreatedBy.ToString() &&
                                   d.BusinessId == followUp.BusinessId)
                        .Select(d => d.DeviceToken)
                        .FirstOrDefaultAsync();

                    var customer = await context.Customers.FindAsync(followUp.CustomerId);

                    if (!string.IsNullOrEmpty(deviceToken))
                    {
                        {
                            await pushNotificationService.SendPushNotification(new List<string> { deviceToken }, "Follow-Up Reminder", $"{followUp.Type} with {customer?.FirstName} {customer?.LastName} at {followUp.Date:g}");
                        }

                        followUp.Reminder = false;
                        context.Update(followUp);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
