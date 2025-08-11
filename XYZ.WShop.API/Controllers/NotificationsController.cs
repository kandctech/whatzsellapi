using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Notification;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Application.Interfaces.Services.Identity;

namespace XYZ.WShop.API.Controllers
{
    [Route("api/v{version:apiVersion}/notifications")]
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [ProducesResponseType(typeof(ResponseModel<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> AddAsync(NotificationRequest request)
        {
            return Ok(await _notificationService.AddAsync(request));
        }

        [ProducesResponseType(typeof(ResponseModel<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public IActionResult DeleteAsync(Guid id)
        {

          var response = _notificationService.DeleteAsync(id);

            return Ok(response);

        }

        [ProducesResponseType(typeof(ResponseModel<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = await _notificationService.GetAllAsync(businessId,page, pageSize, searchTerm, startDate, endDate);
            return Ok(result);

        }
    
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<NotificationResponse>>> GetByIdAsync(Guid id)
        {
            var result = await _notificationService.GetByIdAsync(id);
            return Ok(result);

        }

        [HttpPut("mark-as-read")]
        public async Task<ActionResult<ResponseModel<bool>>> MarkAsRead(Guid id)
        {
            var result = await _notificationService.MarkAsRead(id);
            return Ok(result);

        }

        [HttpPut("mark-all-read")]
        public async Task<ActionResult<ResponseModel<bool>>> MarkAllRead(Guid businessId)
        {
            var result = await _notificationService.MarkAllRead(businessId);
            return Ok(result);

        }

        [HttpGet("unread")]
        public async Task<ActionResult<ResponseModel<int>>> GetUnreadCount(Guid businessId)
        {
            var result = await _notificationService.UnReadCount(businessId);
            return Ok(result);

        }
    }
}
