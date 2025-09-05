
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Subscription : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExpired => DateTime.UtcNow.AddDays(1) > EndDate;
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
