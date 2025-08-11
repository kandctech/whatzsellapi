using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Application.Interfaces.Services.Identity;

namespace XYZ.WShop.API.Controllers
{
    [Route("api/v{version:apiVersion}/auths")]
    public class AuthsController : BaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger<AuthsController> _logger;

        public AuthsController(IIdentityService identityService, IUserManagerService userManagerService, ILogger<AuthsController> logger)
        {
            _identityService = identityService;
            _userManagerService = userManagerService;
            _logger = logger;
        }

        [ProducesResponseType(typeof(ResponseModel<IdentityTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(AuthLoginRequest loginRequest)
        {
            return Ok(await _identityService.AuthenticateAsync(loginRequest, null));
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {

           _userManagerService.ForgotPassword(model, Request.Headers["origin"]);

            return Ok(new ResponseModel<string>
            {
                Data = "Password reset successful.",
                Message = "Please check your email for password reset instructions.",
                IsSuccess = true
            });

        }

        [AllowAnonymous]
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode(SendCodeRequest sendCodeRequest)
        {
            var result = await _userManagerService.SendPasswordResetCodeAsync(sendCodeRequest.Email);
            return Ok(new ResponseModel<string>
            {
                Data = "Code has been sent to your email.",
                Message = "Please check your email and use the code to reset your password.",
                IsSuccess = true
            });

        }
        [AllowAnonymous]
        [HttpPut]
        [Route("password/change")]
        public async Task<ActionResult<ResponseModel<UserProfileModel>>> ChangePassword(PasswordChangeRequest request)
        {
            var result = await _userManagerService.ChangePassword(request);
            return Ok(result);

        }
    }
}
