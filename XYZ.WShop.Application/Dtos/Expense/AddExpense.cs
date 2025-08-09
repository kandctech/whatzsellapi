

namespace XYZ.WShop.Application.Dtos.Expense
{
    public class AddExpense
    {
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime ExpenseDate { get; set; }
    }

    public class UpdateExpense: AddExpense
    {
        public Guid Id { get; set; }
    }

    public class ExpenseResponse : AddExpense
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByFullName { get; set; }
    }
}
