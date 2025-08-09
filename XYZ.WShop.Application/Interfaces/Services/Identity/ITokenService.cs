
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services.Identity
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>The generated JWT token as a string.</returns>
        Task<string> GenerateJwtToken(ApplicationUser user);
    }
}
