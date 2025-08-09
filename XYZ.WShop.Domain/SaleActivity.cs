using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class SaleActivity: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public bool LinkedView { get; set; }
        public bool OrderClicked { get; set; }
        public Guid BusinessId { get; set; }
        public Guid ProductId { get; set; }
        public bool IsCatalogCliked { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? DeviceType { get; set; }
        public string? Source { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
