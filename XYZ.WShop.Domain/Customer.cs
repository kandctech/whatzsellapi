
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Customer: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}
