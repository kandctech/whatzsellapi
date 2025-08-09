using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Services.Identity;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityToken _token;

        public TokenService(
            UserManager<ApplicationUser> userManager,
            IdentityToken token,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _token = token;
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>The JWT token as a string.</returns>
        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            string role = (await _userManager.GetRolesAsync(user))[0];
            byte[] secret = Encoding.ASCII.GetBytes(_token.Secret);

            JwtSecurityTokenHandler handler = new();
            SecurityTokenDescriptor descriptor = new()
            {
                Issuer = _token.Issuer,
                Audience = _token.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                new("UserId", user.Id.ToString()),
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_token.Expiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
