using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Dtos.Subscription;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasActiveSubscriptionAsync(Guid bunessId)
        {
            var subscription = await _context.Subscriptions
                .Where(s => s.BusinessId == bunessId && s.IsActive)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            return subscription != null && subscription.EndDate > DateTime.UtcNow;
        }

        public async Task<DateTime?> GetSubscriptionExpiryAsync(Guid businessId)
        {
            var subscription = await _context.Subscriptions
                .Where(s => s.BusinessId == businessId && s.IsActive)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            return subscription?.EndDate;
        }

        public async Task<IEnumerable<SubscriptionNotificationDto>> GetExpiringSubscriptionsAsync()
        {
            var warningPeriods = new[] { 1, 3, 7 }; // Days before expiry to send warnings
            var today = DateTime.UtcNow.Date;

            return await _context.Subscriptions
                .Where(s => s.IsActive && s.EndDate > today)
                .Where(s => warningPeriods.Contains((s.EndDate - today).Days))
                .Select(s => new SubscriptionNotificationDto
                {
                    UserEmail = s.Email,
                    UserName = s.FullName,
                    ExpiryDate = s.EndDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionNotificationDto>> GetExpiredSubscriptionsAsync()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            return await _context.Subscriptions
                .Where(s => !s.IsActive)
                .Select(s => new SubscriptionNotificationDto
                {
                    UserEmail = s.Email,
                    UserName = s.FullName,
                    ExpiryDate = s.EndDate
                })
                .ToListAsync();
        }
    }
}
