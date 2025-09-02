
using System.Threading.Tasks;
using XYZ.WShop.Domain.Enums;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Ticket: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public TicketCategory Category { get; set; } = TicketCategory.General;
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public DateTime? ResolvedAt { get; set; }

        // Navigation property
        public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
        public DateTime ResolvedDate { get; set; }
    }
}
