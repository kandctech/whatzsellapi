
using System.ComponentModel.DataAnnotations;

namespace XYZ.WShop.Application.Dtos.User
{
    public class SendCodeRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
