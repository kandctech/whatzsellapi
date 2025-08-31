

namespace XYZ.WShop.Application.Dtos.Followup
{
    public class CreateFollowUpRequest
    {
        public string CustomerId { get; set; }
        public Guid BusinessId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Type { get; set; } = "Call";
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
        public bool Reminder { get; set; } = false;
    }
}
