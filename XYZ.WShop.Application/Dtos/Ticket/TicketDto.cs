

namespace XYZ.WShop.Application.Dtos.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<ReplyDto> Replies { get; set; }
    }
}
