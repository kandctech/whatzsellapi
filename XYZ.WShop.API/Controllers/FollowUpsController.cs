
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XYZ.WShop.Application.Dtos.Followup;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/follow-ups")]
    public class FollowUpsController : BaseController
    {
        private readonly ILogger<FollowUpsController> _logger;
        private readonly IFollowUpService _followUpService;

        public FollowUpsController(
            ILogger<FollowUpsController> logger,
            IFollowUpService followUpService)
        {
            _logger = logger;
            _followUpService = followUpService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _followUpService.GetFollowUpByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            Guid businessId,
            string status = null,
            int page = 1,
            int pageSize = 10
            )
        {
            return Ok(await _followUpService.GetFollowUpsAsync(businessId, status, page, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateFollowUpRequest createFollow)
        {
            return Ok(await _followUpService.CreateFollowUpAsync(Guid.NewGuid(), createFollow));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateFollowUpRequest updateFollowUp)
        {
            return Ok(await _followUpService.UpdateFollowUpAsync(id, updateFollowUp));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _followUpService.DeleteFollowUpAsync(id));
        }
    }
}