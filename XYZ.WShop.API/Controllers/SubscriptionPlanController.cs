using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Dtos.Subscription;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/subscription-plan")]
    public class SubscriptionPlanController : BaseController
    {
        private readonly ILogger<SubscriptionPlanController> _logger;
        private readonly ISubscriptionPlanService _subPlanService;

        public SubscriptionPlanController(
            ILogger<SubscriptionPlanController> logger,
            ISubscriptionPlanService subPlanService)
        {
            _logger = logger;
            _subPlanService = subPlanService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _subPlanService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _subPlanService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SubscriptionPlanRequest subscriptionPlan)
        {
            return Ok(await _subPlanService.AddAsync(subscriptionPlan));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SubscriptionPlanUpdateRequest updateRequest)
        {
            return Ok(await _subPlanService.UpdateAsync(updateRequest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _subPlanService.DeleteAsync(id));
        }
    }
}