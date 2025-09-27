using XYZ.WShop.Domain.Enums;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class Order: IBaseEntity, IAuditableEntity, IDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BusinessId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? OrderNumber { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string? CustomerAddrss { get; set; }
        public string? Note { get; set; }
        public string? PaymentLink { get; set; }
        public DateTime? DatePaymentLinkGenerated { get; set; }
        public bool? PaymentLinkExpired { get; set; }
        public Guid PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CancelDate { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal PaidAmount { get; set; }

        public OrderStatus Status { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
