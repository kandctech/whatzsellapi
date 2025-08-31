
namespace XYZ.WShop.Application.Dtos.Followup
{
    public class UpdateFollowUpRequest
    {
        public Guid BusinessId { get; set; }
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public DateTime? Date { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public bool? Reminder { get; set; }
    }
}
