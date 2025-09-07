using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class DebtorRecord: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BusinessId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Purpose { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RecordDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string Status { get; set; } = "unpaid"; // unpaid, paid
        public List<DebtorPayment> Payments { get; set; } = new List<DebtorPayment>();
    }
}
