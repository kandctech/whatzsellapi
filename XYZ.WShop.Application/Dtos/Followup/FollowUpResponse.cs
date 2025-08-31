

namespace XYZ.WShop.Application.Dtos.Followup
{
    public class FollowUpResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Type { get; set; } 
        public DateTime Date { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // pending, completed
        public bool Reminder { get; set; } = false;
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
