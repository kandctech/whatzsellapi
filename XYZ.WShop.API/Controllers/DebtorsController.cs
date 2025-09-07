
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Debtor;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/debtors")]
    [ApiController]
    public class DebtorsController : BaseController
    {
        private readonly ILogger<DebtorsController> _logger;
        private readonly IDebtorService _debtorService;

        public DebtorsController(
            ILogger<DebtorsController> logger,
            IDebtorService debtorService)
        {
            _logger = logger;
            _debtorService = debtorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDebtor([FromBody] CreateDebtRequest createDebtRequest)
        {
            return Ok(await _debtorService.CreateDebtorAsync(createDebtRequest));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDebtor(Guid id)
        {
            return Ok(await _debtorService.GetDebtorByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetDebtors(
            [FromQuery] Guid businessId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string status = "unpaid",
            [FromQuery] DateTime? date = null,
            [FromQuery] string? search = null)
        {
             return Ok(await _debtorService.GetDebtorAsync(
                businessId,
                page,
                pageSize,
                date,
                status,
                search));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDebtor(Guid id, [FromBody] UpdateDebtRequest updateDebtRequest)
        {
            if (id != updateDebtRequest.Id)
            {
                return BadRequest(ResponseModel<object>.CreateResponse(
                    null,
                    "ID in the route does not match ID in the request body",
                    false));
            }

            return Ok(await _debtorService.UpdateDebtorAsync(updateDebtRequest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDebtor(Guid id)
        {
            return Ok(await _debtorService.DeleteDebtorAsync(id));
        }

        [HttpPost("{debtorId}/payments")]
        public async Task<IActionResult> AddPayment(Guid debtorId, [FromBody] DebtorPayment debtorPayment)
        {
            return Ok(await _debtorService.AddPaymentAsync(debtorId, debtorPayment));
        }
    }
}