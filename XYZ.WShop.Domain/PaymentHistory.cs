using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Enums;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class PaymentHistory: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public PayType PaymentType { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
