using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Business : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Slug { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public decimal WalletBalance { get; set; } = 0;
        public string PhoneNumber { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
