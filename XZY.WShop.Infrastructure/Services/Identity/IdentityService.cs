using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio.Exceptions;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Interfaces.Services.Identity;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Services.Helpers;

namespace XZY.WShop.Infrastructure.Services.Identity
{
    /// <summary>
    /// Represents a service for managing user identities.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public IdentityService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }


        /// <summary>
        /// Gets the username of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The username of the user.</returns>
        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user?.UserName;
        }

        /// <summary>
        /// Checks if a user is in a specific role asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        /// <summary>
        /// Authenticates a user asynchronously.
        /// </summary>
        /// <param name="request">The login request object.</param>
        /// <param name="ipAddress">The IP address of the user.</param>
        /// <returns>An <see cref="ResponseModel<IdentityTokenResponse>"/> object containing the user, role, and JWT token.</returns>
        public async Task<ResponseModel<UserProfileModel>> AuthenticateAsync(AuthLoginRequest request, string ipAddress)
        {
            ApplicationUser user = await GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ResponseModel<UserProfileModel>.CreateResponse(null, "Invalid credentials.", false);
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

            UserProfileModel profileModel = null;

            if (signInResult.Succeeded)
            {
                if (request?.DeviceToken != null)
                {
                    request.DeviceToken = request.DeviceToken;
                }

                profileModel = new UserProfileModel();

                var business = await _context.Busineses
                    .FirstOrDefaultAsync(b=> b.Id == user.BusinessId);

                var jwtToken = TokenHelper.GenerateJwtToken(user, _config);
                profileModel.Token = jwtToken;
                profileModel.Email = user.Email;
                profileModel.FirstName = user.FirstName;
                profileModel.LastName = user.LastName;
                profileModel.BusinessName = business.Name;
                profileModel.Slug = business.Slug;
                profileModel.BusinessId = business.Id;
                profileModel.BusinessAddress = business.Address;
                profileModel.PhoneNumber = user.PhoneNumber;
                profileModel.Role = user.Role;
                profileModel.BusinessCategory = business.Category;
                profileModel.BusinessDescription = business.Description;
                profileModel.Logo = business.Logo;
                profileModel.ProfileImageUrl = user?.ProfileImageUrl;
                profileModel.Id = Guid.Parse(user.Id);



                return ResponseModel<UserProfileModel>.CreateResponse(profileModel, "Success", true);
            }
            else
            {
                return ResponseModel<UserProfileModel>.CreateResponse(null, "Invalid credentials.", false);
            }


        }

        private async Task<bool> IsValidUser(string username, string password)
        {
            ApplicationUser user = await GetUserByEmailAsync(username);

            if (user == null)
            {
                return false;
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, password, true, false);

            return signInResult.Succeeded;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private string generateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["AppSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
