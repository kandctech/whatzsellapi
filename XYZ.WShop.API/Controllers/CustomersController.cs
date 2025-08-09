using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/customers")]
    public class CustomersController : BaseController
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly ICustomerService _customerService;

        public CustomersController(
            ILogger<CustomersController> logger,
            ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _customerService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null)
        {
            return Ok(await _customerService.GetAllAsync(businessId, page, pageSize, searchTerm));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomer createCustomer)
        {
            return Ok(await _customerService.AddAsync(createCustomer));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCustomer updateCustomer)
        {
            return Ok(await _customerService.UpdateAsync(updateCustomer));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _customerService.DeleteAsync(id));
        }
    }
}