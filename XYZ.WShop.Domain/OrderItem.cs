using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class OrderItem: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Qty { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = String.Empty;
        public string ProductDescription { get; set; } = String.Empty;
        public decimal Amount { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
