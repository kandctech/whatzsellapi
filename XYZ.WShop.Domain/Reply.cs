
using System.ComponentModel.DataAnnotations.Schema;

namespace XYZ.WShop.Domain
{
    public class Reply
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Guid TicketId { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }
    }
}
