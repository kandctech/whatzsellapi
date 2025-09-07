

namespace XYZ.WShop.Application.Dtos.Creditor
{
    public class AddCreditPaymentRequest
    {
        public Guid CreditRecordId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
