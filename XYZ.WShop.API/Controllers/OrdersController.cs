using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Services;

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
            [FromQuery] string? searchTerm = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return Ok(await _orderService.GetAllAsync(businessId, page, pageSize, searchTerm, startDate, endDate));
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

        [HttpGet("orders-by-customer-id")]
        public async Task<IActionResult> GetOrdersByCustomerId(
           [FromQuery] Guid businessId,
            [FromQuery] Guid customerId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 10,
          [FromQuery] string? searchTerm = null)
        {
            return Ok(await _orderService.GetOrderByCustomerIdAsync(businessId, customerId, page, pageSize, searchTerm));
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel(Guid businessId, [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var excelBytes = await _orderService.ExportOrdersToExcel(businessId, startDate, endDate);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }

        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportToPdf(Guid businessId, [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var pdfBytes = await _orderService.ExportOrdersToPdf(businessId, startDate, endDate);
            return File(pdfBytes, "application/pdf", $"Products{DateTime.UtcNow.Ticks}.pdf");
        }
    }
}