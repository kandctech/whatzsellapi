using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain.Enums
{
    public enum PaymentType
    {
        None,
        Credit,
        Debit
    }

    public enum PaymentStatus
    {
        Paid,
        Pending,
        Expired
    }

    public enum PayType
    {
        None,
        Creditor,
        Debitor
    }
}
