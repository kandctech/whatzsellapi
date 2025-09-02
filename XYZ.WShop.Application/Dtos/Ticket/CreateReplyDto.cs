
namespace XYZ.WShop.Application.Dtos.Ticket
{
    public class CreateReplyDto
    {
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
