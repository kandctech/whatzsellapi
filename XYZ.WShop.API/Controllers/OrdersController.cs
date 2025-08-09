using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/orders")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;

        public OrdersController(
            ILogger<OrdersController> logger,
            IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            return Ok(await _orderService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(
             [FromQuery] Guid businessId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            return Ok(await _orderService.GetAllAsync(businessId, page, pageSize, searchTerm));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrder createOrder)
        {
            return Ok(await _orderService.AddAsync(createOrder));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            return Ok(await _orderService.DeleteAsync(id));
        }
    }
}