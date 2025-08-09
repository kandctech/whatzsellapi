using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.User;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IUserManagerService
    {
        Task<ResponseModel<ApplicationUser>> FindByIdAsync(string userId);
        Task<ResponseModel<ApplicationUser>> FindByEmailAsync(string userId);
        Task<ResponseModel<ApplicationUser>> FindByPhoneNumberAsync(string phoneNumber);
        Task<ResponseModel<ApplicationUser>> FindByNameAsync(string userId);
        Task<ResponseModel<IdentityResult>> UpdateAsync(ApplicationUser user);
        Task<ResponseModel<UserProfileModel>> CreateAdminAsync(CreateAccount user);

        Task<ResponseModel<bool>> RemoveUser(string id);
        Task<ResponseModel<IdentityResult>> AddToRoleAsync(ApplicationUser user, string role);
        Task<ResponseModel<IList<string>>> GetRolesAsync(ApplicationUser user);
        Task<ResponseModel<IdentityResult>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<ResponseModel<string>> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ResponseModel<string>> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<ResponseModel<IdentityResult>> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<ResponseModel<IdentityResult>> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<ResponseModel<bool>> CheckPasswordAsync(ApplicationUser user, string password);
        Task<ResponseModel<IdentityResult>> CreateUserAsync(CreateUserAccount user);
        Task<bool> SendPasswordResetCodeAsync(string email);
        Task<ResponseModel<UserProfileModel>> ChangePassword(PasswordChangeRequest model);
        Task<bool> ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<PaginatedResponseModel<UserProfileModel>> GetAllUsers(Guid businessId, int page = 1, int pageSize = 10);

        Task<ResponseModel<UserProfileModel>> UpdateUser(EditUserRequest model);
        Task<ResponseModel<bool>> DeleteUser(string userId);
        Task<ResponseModel<UserProfileModel>> UpdateBusiness(EditBusinessRequest editBusiness);
    }
}
