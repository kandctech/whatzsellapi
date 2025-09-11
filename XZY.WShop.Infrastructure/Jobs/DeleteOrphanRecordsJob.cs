using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Jobs
{
    public class DeleteOrphanRecordsJob : IInvocable
    {
        private readonly ILogger<DeleteOrphanRecordsJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPushNotificationService _pushNotificationService;

        public DeleteOrphanRecordsJob(ILogger<DeleteOrphanRecordsJob> logger, IPushNotificationService pushNotificationService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _pushNotificationService = pushNotificationService;
        }

        public async Task Invoke()
        {
            try
            {
                await DeleteOrphanRecordsJobsAsync();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private async Task DeleteOrphanRecordsJobsAsync()
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var businesssNotInUsers = await context.Busineses
                .Where(p => !context.Users.Any(u => u.BusinessId == p.Id))
                .ToListAsync();

                if (businesssNotInUsers.Count > 0)
                {
                    context.Busineses.RemoveRange(businesssNotInUsers);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Removed {businesssNotInUsers.Count} from Business Table");
                }

                // List all your entity types that have BusinessId property
                var entitiesWithBusinessId = new (string tableName, IQueryable<object> query)[]
                {
                    ("Products", context.Products.Cast<object>()),
                    ("Orders", context.Orders.Cast<object>()),
                    ("Customers", context.Customers.Cast<object>()),

                    ("CreditorPayments", context.Products.Cast<object>()),
                    ("CreditorRecords", context.Orders.Cast<object>()),
                    ("DebtorPayments", context.Customers.Cast<object>()),

                    ("DebtorRecords", context.Products.Cast<object>()),
                    ("Expenses", context.Orders.Cast<object>()),
                    ("FollowUps", context.Customers.Cast<object>()),

                    ("Notifications", context.Products.Cast<object>()),
                    ("OrderItems", context.Orders.Cast<object>()),
                    ("Replies", context.Customers.Cast<object>()),

                    ("SaleActivities", context.Products.Cast<object>()),
                    ("Subscriptions", context.Orders.Cast<object>()),
                    ("Tasks", context.Customers.Cast<object>()),

                    ("Tickets", context.Products.Cast<object>()),
                    ("Transactions", context.Orders.Cast<object>())
                };

                foreach (var (tableName, query) in entitiesWithBusinessId)
                {
                    var orphanedRecords = await query
                        .Where(e => !context.Users.Any(u => u.BusinessId == EF.Property<Guid>(e, "BusinessId")))
                        .ToListAsync();

                    if (orphanedRecords.Any())
                    {
                        context.RemoveRange(orphanedRecords);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Removed {orphanedRecords.Count} from {tableName}");
                    }
                }
            }
        }
    }
}

