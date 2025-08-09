
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services.Identity
{
    /// <summary>
    /// Services abstraction for managing Identity.
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// Gets the user name asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user name.</returns>
        Task<string> GetUserNameAsync(string userId);

        /// <summary>
        /// Gets the user asynchronously.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <returns>The user name.</returns>
        Task<ApplicationUser> GetUserByEmailAsync(string email);

        /// <summary>
        /// Checks if a user is in a specific role asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        Task<bool> IsInRoleAsync(string userId, string role);

        /// <summary>
        /// Authenticates a user asynchronously.
        /// </summary>
        /// <param name="request">The login request.</param>
        /// <param name="ipAddress">The IP address of the user.</param>
        /// <returns>The IdentityTokenResponse after authentication.</returns>
        Task<ResponseModel<UserProfileModel>> AuthenticateAsync(AuthLoginRequest request, string ipAddress);

    }
}
