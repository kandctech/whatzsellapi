
namespace XYZ.WShop.Application.Dtos.Notification
{
    public class NotificationRequest
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Message { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
