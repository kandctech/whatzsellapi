using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Dtos
{
    public record IdentityTokenResponse
    {
        public IdentityTokenResponse(ApplicationUser user,
                             string role,
                             string token
                            )
        {
            Id = user.Id;
            EmailAddress = user.Email;
            Token = token;
            Role = role;
        }

        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
