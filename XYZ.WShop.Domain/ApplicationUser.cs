using Microsoft.AspNetCore.Identity;

namespace XYZ.WShop.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? Role { get; set; }
        public Guid BusinessId { get; set; } 


        public DateTime? PasswordResetCodeExpiryDate { get; set; }
        public string? PasswordResetCode { get; set; }
    }
}
