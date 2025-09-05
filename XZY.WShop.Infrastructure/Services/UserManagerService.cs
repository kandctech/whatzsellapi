using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Slugify;
using System.Security.Cryptography;
using System.Text;
using Twilio.Exceptions;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Application.User;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Services.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace XZY.WShop.Infrastructure.Services
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _config;

        public UserManagerService(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IConfiguration config)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _config = config;
        }
        public async Task<ResponseModel<IdentityResult>> AddToRoleAsync(ApplicationUser user, string role)
        {
            var userResponse = await _userManager.AddToRoleAsync(user!, role);

            return ResponseModel<IdentityResult>.CreateResponse(userResponse, string.Format(ApplicationContants.Messages.CreatedSuccessfully, "Role"), true);
        }

        public async Task<ResponseModel<IdentityResult>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return ResponseModel<IdentityResult>.CreateResponse(await _userManager.ChangePasswordAsync(user!, currentPassword, newPassword), string.Format(ApplicationContants.Messages.ChangedSuccessfulMessage, "Password"), true);
            ;
        }

        public async Task<ResponseModel<bool>> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return ResponseModel<bool>.CreateResponse(await _userManager.CheckPasswordAsync(user!, password), string.Format(ApplicationContants.Messages.SuccessRetrieval, "Password"), true);
        }

        public async Task<ResponseModel<IdentityResult>> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return ResponseModel<IdentityResult>.CreateResponse(await _userManager.ConfirmEmailAsync(user!, token), string.Format(ApplicationContants.Messages.SuccessRetrieval, "Email confirm"), true);
        }

        public async Task<ResponseModel<UserProfileModel>> CreateAdminAsync(CreateAccount user)
        {
          
            var dbUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

            if (dbUser != null)
            {
                throw new BadRequestException("User with same email already exist!");
            }

            SlugHelper helper = new SlugHelper();
            var slug = helper.GenerateSlug(user.BusinessName);

            var isBusinessExist = await _applicationDbContext.Busineses.CountAsync(b=> b.Slug == slug);

            if (isBusinessExist > 0) {
                throw new BadRequestException("Business name is already in used");
            }

            var businessId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            await _applicationDbContext.Busineses.AddAsync(new Business
            {
                Address = user.BusinessAddress,
                Name = user.BusinessName,
                Slug = slug,
                IsActive = true,
                Email = user.Email,
                Currency = user.Currency,
                PhoneNumber = user.PhoneNumber,
                WalletBalance = 0M,
                Id = businessId,
                CreatedBy = Guid.Parse(userId),
            });

            await _applicationDbContext.SaveChangesAsync();


            var business = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Email,
                Id = userId,
                DeviceToken = user.DeviceToken,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BusinessId = businessId,
                EmailConfirmed = true,
                Role = "Admin"

            };

            var result = await _userManager.CreateAsync(business!, user.Password);
            var profileModel = new UserProfileModel();

            Subscription subscription = new Subscription
            {
                BusinessId = businessId,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(ApplicationContants.TrialDay),
                CreatedDate = DateTime.UtcNow,
                Email = business.Email,
                ModifiedDate = DateTime.UtcNow,
                FullName = $"{user.FirstName} {user.LastName}" ,
            };

            await _applicationDbContext.Subscriptions.AddAsync(subscription);
            await _applicationDbContext.SaveChangesAsync();

            var jwtToken = TokenHelper.GenerateJwtToken(business, _config);
            profileModel.Token = jwtToken;
            profileModel.Email = user.Email;
            profileModel.FirstName = user.FirstName;
            profileModel.LastName = user.LastName;
            profileModel.BusinessName = user.BusinessName;
            profileModel.Currency = user.Currency;
            profileModel.Slug = slug;
            profileModel.BusinessId = businessId;
            profileModel.BusinessAddress = user.BusinessAddress;
            profileModel.PhoneNumber = user.PhoneNumber;
            profileModel.LastPaymentDate = subscription.ModifiedDate;
            profileModel.NextPaymentDate = subscription.EndDate;
            profileModel.Role = "Admin";

            var resp = ResponseModel<UserProfileModel>.CreateResponse(profileModel, string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Admin"), true);

            return resp;
        }

        public async Task<ResponseModel<IdentityResult>> CreateUserAsync(CreateUserAccount user)
        {
            var business = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName= user.LastName,
                Email = user.Email,
                UserName = user.Email,
                PhoneNumber = user.PhoneNumber,
                BusinessId = user.BusinessId,
                EmailConfirmed = true,
                Role = "User"

            };

            return ResponseModel<IdentityResult>.CreateResponse(await _userManager.CreateAsync(business!, user.Password), string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "User"), true);
        }

        public async Task<PaginatedResponseModel<UserProfileModel>> GetAllUsers(Guid businessId, int page = 1, int pageSize = 10)
        {
            var query = _applicationDbContext
                 .Users
                 .Where(q => q.BusinessId == businessId)
                 .OrderBy(q=> q.FirstName);

            var count = await query.CountAsync();

            var skip = (page - 1) * pageSize;

            var users = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            var userModels = users.Select(x => new UserProfileModel
            {
                BusinessId = businessId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Role = x.Role,
                Id = Guid.Parse(x.Id)
            }).ToList();

            return new PaginatedResponseModel<UserProfileModel>
            {
                Data = userModels,
                Success = true,
                Message = "Users retrieved successfully",
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = count,
                TotalPages = totalPages
            };
        }

        public async Task<ResponseModel<ApplicationUser>> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ResponseModel<ApplicationUser>.CreateResponse(user, string.Format(ApplicationContants.Messages.NotFound, "User"), false);
            }
            return ResponseModel<ApplicationUser>.CreateResponse(user, string.Format(ApplicationContants.Messages.SuccessRetrieval, "User"), true);
        }

        public async Task<ResponseModel<ApplicationUser>> FindByIdAsync(string userId)
        {
            return ResponseModel<ApplicationUser>.CreateResponse(await _userManager.FindByIdAsync(userId), string.Format(ApplicationContants.Messages.SuccessRetrieval, "User"), true);
        }

        public async Task<ResponseModel<ApplicationUser>> FindByNameAsync(string userName)
        {
            return ResponseModel<ApplicationUser>.CreateResponse(await _userManager.FindByNameAsync(userName), string.Format(ApplicationContants.Messages.SuccessRetrieval, "User"), true);
        }

        public async Task<ResponseModel<ApplicationUser>> FindByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _userManager.Users
         .FirstOrDefaultAsync(usr => usr.PhoneNumber != null &&
                                      usr.PhoneNumber.Substring(usr.PhoneNumber.Length - 10).Equals(phoneNumber.Substring(phoneNumber.Length - 10)));

            if (user == null)
            {
                return ResponseModel<ApplicationUser>.CreateResponse(user, string.Format(ApplicationContants.Messages.NotFound, "User"), false);
            }
            return ResponseModel<ApplicationUser>.CreateResponse(user, string.Format(ApplicationContants.Messages.SuccessRetrieval, "User"), true);
        }

        public async Task<ResponseModel<string>> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return ResponseModel<string>.CreateResponse(await _userManager.GenerateEmailConfirmationTokenAsync(user), string.Format(ApplicationContants.Messages.SuccessRetrieval, "Email Confirm"), true);
        }

        public async Task<ResponseModel<string>> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return ResponseModel<string>.CreateResponse(await _userManager.GenerateEmailConfirmationTokenAsync(user), string.Format(ApplicationContants.Messages.SuccessRetrieval, "Password token"), true);
        }

        public async Task<ResponseModel<IList<string>>> GetRolesAsync(ApplicationUser user)
        {
            return ResponseModel<IList<string>>.CreateResponse(await _userManager.GetRolesAsync(user!), string.Format(ApplicationContants.Messages.SuccessRetrieval, "Roles"), true);
        }

        public async Task<ResponseModel<bool>> RemoveUser(string id)
        {
           var user = await _userManager.Users.FirstOrDefaultAsync(u=> u.Id == id);

            if (user == null) {
               await _userManager.DeleteAsync(user);
            }

            return ResponseModel<bool>.CreateResponse(true, string.Format(ApplicationContants.Messages.PasswordResetMessage), true);

        }

        public async Task<ResponseModel<IdentityResult>> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return ResponseModel<IdentityResult>.CreateResponse(await _userManager.ResetPasswordAsync(user!, token, newPassword), string.Format(ApplicationContants.Messages.PasswordResetMessage), true);
        }

        public async Task<ResponseModel<IdentityResult>> UpdateAsync(ApplicationUser user)
        {
            return ResponseModel<IdentityResult>.CreateResponse(await _userManager.UpdateAsync(user!), string.Format(ApplicationContants.Messages.PasswordResetMessage), true);
        }

        public async Task<ResponseModel<UserProfileModel>> ChangePassword(PasswordChangeRequest model)
        {

            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new BadRequestException("New password and confirm pasword MUST match.");
            }

            var userToChange = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower()) ?? throw new BadRequestException("Account not found.");

            if (userToChange.PasswordResetCode == null || userToChange.PasswordResetCodeExpiryDate == null)
            {
                throw new BadRequestException("Invalid code.");
            }


            if (userToChange.PasswordResetCode != model.Code)
            {
                throw new BadRequestException("Invalid code.");
            }

            if (userToChange.PasswordResetCodeExpiryDate < DateTime.UtcNow)
            {
                throw new BadRequestException("Code has expired.");
            }

           var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(userToChange);

           await _userManager.ResetPasswordAsync(userToChange, passwordResetToken, model.NewPassword);

            userToChange.PasswordResetCode = null;
            userToChange.PasswordResetCodeExpiryDate = null;

            _applicationDbContext.Entry(userToChange).State = EntityState.Modified;

            await _applicationDbContext.SaveChangesAsync();


            UserProfileModel profileModel = new UserProfileModel();
            var business = await _applicationDbContext.Busineses.FirstOrDefaultAsync();

            var jwtToken = TokenHelper.GenerateJwtToken(userToChange, _config);

            profileModel.Token = jwtToken;
            profileModel.Email = userToChange.Email;
            profileModel.FirstName = userToChange.FirstName;
            profileModel.LastName = userToChange.LastName;
            profileModel.BusinessName = business.Currency;
            profileModel.BusinessName = business.Name;
            profileModel.BusinessId = business.Id;
            profileModel.BusinessAddress = business.Address;
            profileModel.PhoneNumber = userToChange.PhoneNumber;


            return ResponseModel<UserProfileModel>.CreateResponse(profileModel, "Success", true);
        }

        public async Task<bool> SendPasswordResetCodeAsync(string email)
        {
            var userToChange = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower()) ?? throw new BadRequestException("Account not found.");

            var code = GenerateRandomPassword(6);
            userToChange.PasswordResetCodeExpiryDate = DateTime.UtcNow.AddHours(24);
            userToChange.PasswordResetCode = code;

            _applicationDbContext.Entry(userToChange).State = EntityState.Modified;

            await SendPasswordResetCodeHelper(userToChange, code);

            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var user = _applicationDbContext.Users.SingleOrDefault(x => x.Email == model.Email);

            if (user == null)
            {
                throw new BadRequestException("Please check your email for password reset instructions.");
            }

            var newPassword = GenerateRandomPassword(8);

            user.PasswordHash = BC.HashPassword(newPassword);

            _applicationDbContext.Users.Update(user);

            // send email
            await SendPasswordResetEmail(user, newPassword);
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        #region Helpers

        private async Task SendPasswordResetEmail(ApplicationUser user, string newPassword)
        {

            string message = $@"
                            <p>Dear User,</p>

                            <p>Please find below your temporary password: <strong style='font-size:20px'>{newPassword}</strong>.</p>

                            <p>You may use this password to log in to your account. For security purposes, we recommend resetting your password immediately after logging in through your profile settings.</p>

                            <p>Best regards,<br>
                            <strong>WShop Team</strong></p>

                            <p style=""color:#888888;font-size:12px;"">
                            WakaWithUs Customer Support<br>
                            support@wshop.com<br>
                            {ApplicationContants.SupportNumber}
                            </p>";

            await _applicationDbContext.EmailTemplates.AddAsync(new EmailTemplate
            {
                Id = Guid.NewGuid(),
                Subject = $"{ApplicationContants.AppName}: Forgot Password",
                ToEmail = user.Email,
                Body = $@"<h4>Forgot Password Email</h4>
                         {message}"

            });
        }

        private async Task SendPasswordResetCodeHelper(ApplicationUser user, string code)
        {
            string message = $@"
                            <p>Dear User,</p>

                            <p>Your password reset verification code is:  
                            <strong style='font-size:20px; letter-spacing: 2px;'>{code}</strong>  
                            </p>

                            <p>Please use this code to complete your password reset process.  
                            <strong>This code will expire in 24 hours</strong> for security reasons.</p>  

                            <p>If you did not request this reset, please ignore this email or contact our support team immediately.</p>  

                            <p>Best regards,<br>  
                            <strong>WShop Security Team</strong></p>  

                            <p style='color: #777777; font-size: 12px;'>  
                            For your security, never share this code with anyone.  
                            </p>";

            await _applicationDbContext.EmailTemplates.AddAsync(new EmailTemplate
            {
                Id = Guid.NewGuid(),
                Subject = $"Wshop: Password Reset Code",
                ToEmail = user.Email,
                Body = $@"<h4>Password reset code</h4>
                         {message}"

            });
        }

        private static string GenerateRandomPassword(int length)
        {
            const string validChars = "1234567890";
            var randomBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            var result = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                result.Append(validChars[b % validChars.Length]);
            }
            return result.ToString();
        }

        public async Task<ResponseModel<UserProfileModel>> UpdateUser(EditUserRequest model)
        {
            var userToEdit = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (userToEdit == null)
            {
                throw new BadRequestException("User not found");
            }

            userToEdit.Email = model.Email;
            userToEdit.FirstName = model.FirstName;
            userToEdit.LastName = model.LastName;

            await _userManager.UpdateAsync(userToEdit);

            var useModel = new UserProfileModel
            {
                BusinessId = userToEdit.BusinessId,
                FirstName = userToEdit.FirstName,
                LastName = userToEdit.LastName,
                Email = userToEdit.Email,
                PhoneNumber = userToEdit.PhoneNumber,
                Role = userToEdit.Role,
                Id = Guid.Parse(userToEdit.Id)
            };

            return ResponseModel<UserProfileModel>.CreateResponse(useModel, "User updated", true);
        }

        public async Task<ResponseModel<bool>> DeleteUser(string userId)
        {
            var userToDelete = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userToDelete == null)
            {
                throw new BadRequestException("User not found");
            }
           await _userManager.DeleteAsync(userToDelete);

            return ResponseModel<bool>.CreateResponse(true, "User deleted", true);
        }

        public async Task<ResponseModel<UserProfileModel>> UpdateBusiness(EditBusinessRequest editBusiness)
        {
            var userToEdit = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == editBusiness.UserId.ToString());

            if (userToEdit == null)
            {
                throw new BadRequestException("User not found");
            }

            userToEdit.PhoneNumber = editBusiness.PhoneNumber;
            userToEdit.FirstName = editBusiness.FirstName;
            userToEdit.LastName = editBusiness.LastName;
            userToEdit.ProfileImageUrl = editBusiness.ProfileImageUrl;
            

            await _userManager.UpdateAsync(userToEdit);

            var business = await _applicationDbContext.Busineses.FirstOrDefaultAsync(b => b.Id == editBusiness.BusinessId);

            if (business != null)
            {
                business.Category = editBusiness.BusinessCategory;
                business.Name = editBusiness?.BusinessName ?? string.Empty;
                business.Description = editBusiness?.BusinessDescription;
                business.Logo = editBusiness?.Logo ?? string.Empty;
                business.Address = editBusiness?.BusinessAddress ?? string.Empty;
            }

           await _applicationDbContext.SaveChangesAsync();

            var useModel = new UserProfileModel
            {
                BusinessId = userToEdit.BusinessId,
                FirstName = userToEdit.FirstName,
                LastName = userToEdit.LastName,
                Email = userToEdit.Email,
                PhoneNumber = userToEdit.PhoneNumber,
                Role = userToEdit.Role,
                Id = Guid.Parse(userToEdit.Id),
                BusinessAddress = business?.Address ?? string.Empty,
                BusinessName = business?.Name ?? string.Empty,
                Currency = business?.Currency ?? string.Empty,
                BusinessCategory = business?.Category,
                BusinessDescription = business?.Description ?? string.Empty,
                Logo = business?.Logo ?? string.Empty,
                ProfileImageUrl = userToEdit?.ProfileImageUrl ?? string.Empty,
                Slug = business?.Slug ?? string.Empty,
                UserId = userToEdit.Id
                
            };

            return ResponseModel<UserProfileModel>.CreateResponse(useModel, "User updated", true);
        }
        #endregion
    }
}
