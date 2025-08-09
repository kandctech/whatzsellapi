using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Services.Identity;

namespace XZY.WShop.Infrastructure.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IdentityToken _token;

        public JwtMiddleware(RequestDelegate next, IdentityToken token)
        {
            _next = next;
            _token = token;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dataContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachAccountToContext(context, dataContext, token);
            await _next(context);
        }

        private async Task AttachAccountToContext(HttpContext context, ApplicationDbContext dataContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_token.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                context.Items["User"] = await dataContext.Users.FindAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                // Do nothing if jwt validation fails
            }
        }
    }
}
