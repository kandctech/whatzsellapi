using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ISubscriptionEmailService
    {
        Task SendSubscriptionExpiryWarningAsync(string userEmail, string userName, DateTime expiryDate, int daysUntilExpiry);
        Task SendSubscriptionExpiredAsync(string userEmail, string userName, DateTime expiryDate);
    }
}
