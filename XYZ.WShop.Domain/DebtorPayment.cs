using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain
{
    public class DebtorPayment
    {
        public Guid Id { get; set; }
        public Guid DebtorRecordId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
