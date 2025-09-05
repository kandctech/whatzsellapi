using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Subscription
{
    public class SubscriptionNotificationDto
    {
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
