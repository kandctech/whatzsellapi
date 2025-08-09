using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.User
{
    public class EditUserRequest
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }

    public class EditBusinessRequest
    {
        public Guid UserId { get; set; }
        public Guid BusinessId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessDescription { get; set; }
        public string? BusinessCategory { get; set; }
        public string? BusinessAddress { get; set; }
        public string? Logo { get; set; }
        public string PhoneNumber { get; set; }
    }
}
