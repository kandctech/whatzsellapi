
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class FollowUp : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid CustomerId { get; set; } 
        public string Type { get; set; } = "Call"; // Call, Email, Meeting, Message
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // pending, completed
        public bool Reminder { get; set; } = false;
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
