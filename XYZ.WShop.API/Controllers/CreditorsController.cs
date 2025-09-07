
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Creditor;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/creditors")]
    [ApiController]
    public class CreditorsController : BaseController
    {
        private readonly ILogger<CreditorsController> _logger;
        private readonly ICreditorService _creditorService;

        public CreditorsController(
            ILogger<CreditorsController> logger,
            ICreditorService creditorService)
        {
            _logger = logger;
            _creditorService = creditorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCreditor([FromBody] CreateCreditRequest createCreditRequest)
        {
            return Ok(await _creditorService.CreateCreditAsync(createCreditRequest));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCreditor(Guid id)
        {
            return Ok(await _creditorService.GetCreditByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetCreditors(
            [FromQuery] Guid businessId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
               [FromQuery] string status = "unpaid",
              [FromQuery] DateTime? date = null,
            [FromQuery] string? search = null)
        {
            return Ok(await _creditorService.GetCreditsAsync(
                businessId,
                page,
                pageSize,
                date,
                status,
                search));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCreditor(Guid id, [FromBody] UpdateCreditRequest updateCreditRequest)
        {
            if (id != updateCreditRequest.Id)
            {
                return BadRequest(ResponseModel<object>.CreateResponse(
                    null,
                    "ID in the route does not match ID in the request body",
                    false));
            }

            return Ok(await _creditorService.UpdateCreditAsync(updateCreditRequest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCreditor(Guid id)
        {
            return Ok(await _creditorService.DeleteCreditAsync(id));
        }

        [HttpPost("{creditId}/payments")]
        public async Task<IActionResult> AddPayment(Guid creditId, [FromBody] CreditorPayment creditorPayment)
        {
            return Ok(await _creditorService.AddPaymentAsync(creditId, creditorPayment));
        }
    }
}