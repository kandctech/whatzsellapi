using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class DebtorRecord: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductName { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RecordDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
