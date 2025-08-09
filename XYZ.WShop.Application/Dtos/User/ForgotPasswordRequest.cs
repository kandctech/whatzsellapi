using System.ComponentModel.DataAnnotations;

namespace XYZ.WShop.Application.Dtos.User
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
