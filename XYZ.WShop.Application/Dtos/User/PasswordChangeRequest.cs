
using System.ComponentModel.DataAnnotations;

namespace XYZ.WShop.Application.Dtos.User
{
    public class PasswordChangeRequest
    {
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
