

namespace XYZ.WShop.Application.Dtos.Ticket
{
    public class ReplyDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
