using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Enums;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Transaction:IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string? Narration { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public int DaysPayFor { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus Status { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
