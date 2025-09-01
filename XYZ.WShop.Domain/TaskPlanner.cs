
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class TaskPlanner: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime Time { get; set; } = DateTime.Now;
        public string Priority { get; set; } = "medium"; 
        public string Category { get; set; } = "work";
        public bool Completed { get; set; } = false;
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
