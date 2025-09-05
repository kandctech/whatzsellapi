using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Payment
{
    public class FlutterwaveWebhookRequest
    {
        public string Event { get; set; } // Event type (e.g., "charge.completed", "transfer.completed")
        public FlutterwaveWebhookData Data { get; set; }
    }

    public class FlutterwaveWebhookData
    {
        public string id { get; set; } // Transaction or transfer ID
        public string status { get; set; } // Status (e.g., "successful")
        public decimal amount { get; set; } // Amount in NGN
        public string CustomerEmail { get; set; } // Customer email
        public string tx_ref { get; set; }
    }
}
