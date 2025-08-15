
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Subscription : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndateDate { get; set; }
        public bool IsExpired => DateTime.UtcNow.AddDays(1) > EndateDate;
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
