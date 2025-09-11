

namespace XYZ.WShop.Domain
{
    public class DebtorPayment
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid DebtorRecordId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
