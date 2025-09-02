using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using XYZ.WShop.Domain.Enums;

namespace XYZ.WShop.Application.Dtos.Ticket
{
    public class CreateTicketDto
    {
        public Guid BusinessId { get; set; }
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        public TicketCategory Category { get; set; } = TicketCategory.General;

        public string? UserId { get; set; }

        [EmailAddress]
        public string? UserEmail { get; set; }

        public string? ImageUrl { get; set; }
    }
}
