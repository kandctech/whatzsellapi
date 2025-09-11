using Microsoft.EntityFrameworkCore;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class SubscriptionHelper
    {
        public async Task ValidateSubscriptionAsync(Guid businessId, ApplicationDbContext dbContext)
        {
            var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(s => s.BusinessId == businessId);

            if (subscription == null)
            {
                throw new XYZ.WShop.Application.Exceptions.BadRequestException("Sunscription not found! Please subscribe to continue.");
            }

            if (DateTime.UtcNow > subscription.EndDate)
            {
                throw new XYZ.WShop.Application.Exceptions.BadRequestException("Your subscription has expired. To maintain access, please renew your subscription.");
            }
        }
    }
}