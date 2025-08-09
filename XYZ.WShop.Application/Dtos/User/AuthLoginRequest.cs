using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.User
{
    public record AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? DeviceToken { get; set; }
    }
}
