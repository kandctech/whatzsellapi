using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Services.Identity;

namespace XZY.WShop.Infrastructure.Services.Identity
{
    [JsonObject("token")]
    public class IdentityToken : IIdentityToken

    {
        public const string Token = "token";
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expiry { get; set; }
        public int RefreshExpiry { get; set; }
    }
}
