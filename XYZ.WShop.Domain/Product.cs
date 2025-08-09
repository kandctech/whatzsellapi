using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Product : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityInStock { get; set; }
        public decimal CostPrice { get; set; }
        public string? ImageUrls { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
