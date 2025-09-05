

namespace XYZ.WShop.Application.Dtos.Transaction
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public int DaysPayFor { get; set; }
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? Currency { get; set; }
    }

    public class UpdateTransactionDto
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
    }
}
