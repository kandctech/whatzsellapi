using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Payment
{
    public class PaystackWebhookRequest
    {
        public string Event { get; set; } // Event type (e.g., "charge.success", "transfer.success")
        public PaystackWebhookData Data { get; set; }

    }

    public class PaystackWebhookData
    {
        public string Reference { get; set; } // Transaction or transfer reference
        public string Status { get; set; } // Status (e.g., "success")
        public decimal Amount { get; set; } // Amount in kobo
        public string CustomerEmail { get; set; }
    }
}
