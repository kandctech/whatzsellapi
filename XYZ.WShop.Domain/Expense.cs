
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Expense : IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
