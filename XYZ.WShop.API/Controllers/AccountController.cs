
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Application.User;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/accounts")]
    public class AccountController : BaseController
    {
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger<AuthsController> _logger;

        public AccountController(IUserManagerService userManagerService,
ILogger<AuthsController> logger)
        {
            _userManagerService = userManagerService;
            _logger = logger;
        }

        [ProducesResponseType(typeof(ResponseModel<IdentityTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost("business")]
        public async Task<IActionResult> CreateBusinessAsync(CreateAccount createAccount)
        {
            return Ok(await _userManagerService.CreateAdminAsync(createAccount));
        }

        [ProducesResponseType(typeof(ResponseModel<IdentityTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost("add-user")]
        public async Task<IActionResult> CreateUserAsync(CreateUserAccount createAccount)
        {
            return Ok(await _userManagerService.CreateUserAsync(createAccount));
        }

        [ProducesResponseType(typeof(PaginatedResponseModel<UserProfileModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsersAsync(Guid businessId, int page = 1, int pageSize = 10)
        {
            return Ok(await _userManagerService.GetAllUsers(businessId, page, pageSize));
        }


        [ProducesResponseType(typeof(ResponseModel<UserProfileModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUsersAsync(EditUserRequest editUser)
        {
            return Ok(await _userManagerService.UpdateUser(editUser));
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUsersAsync(string userId)
        {
            return Ok(await _userManagerService.DeleteUser(userId));
        }


        [ProducesResponseType(typeof(ResponseModel<UserProfileModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPut("update-business")]
        public async Task<IActionResult> UpdateProfileAsync(EditBusinessRequest editBusiness)
        {
            return Ok(await _userManagerService.UpdateBusiness(editBusiness));
        }

        [HttpPut]
        [Route("change-password")]
        public async Task<ActionResult<ResponseModel<UserProfileModel>>> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _userManagerService.ChangePasswordLoginUser(request);
            return Ok(result);

        }

    }
}
