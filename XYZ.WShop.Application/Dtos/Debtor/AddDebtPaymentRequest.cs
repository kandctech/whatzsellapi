using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Debtor
{
    public class AddDebtPaymentRequest
    {
        public Guid DebtRecordId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
