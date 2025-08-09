using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Interfaces.Services.Identity
{
    public interface IIdentityToken
    {
        public string Secret { get; }
        public string Issuer { get; }
        public string Audience { get; }
        public int Expiry { get; }
        public int RefreshExpiry { get; }
    }
}
