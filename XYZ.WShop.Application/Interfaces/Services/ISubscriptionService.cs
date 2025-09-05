


using XYZ.WShop.Application.Dtos.Subscription;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<bool> HasActiveSubscriptionAsync(Guid businessId);
        Task<DateTime?> GetSubscriptionExpiryAsync(Guid BusinessId);
        Task<IEnumerable<SubscriptionNotificationDto>> GetExpiringSubscriptionsAsync();
        Task<IEnumerable<SubscriptionNotificationDto>> GetExpiredSubscriptionsAsync();
    }
}
