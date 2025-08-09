using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsController : BaseController
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var clientIp = GetClientIp(HttpContext);
            string deviceType;
            if (userAgent.Contains("Android"))
                deviceType = "Android";
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                deviceType = "iOS";
            else if (userAgent.Contains("Windows") || userAgent.Contains("Macintosh"))
                deviceType = "Desktop";
            else
                deviceType = "Unknown";
  
            return Ok(await _productService.GetByIdAsync(id, deviceType, clientIp));

        }

        [HttpGet("all")]
        public async Task<IActionResult> Get(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            return Ok(await _productService.GetAllAsync(businessId, page, pageSize, searchTerm));

        }

        [HttpPost("add")]
        public async Task<IActionResult> Post(AddProduct addProduct)
        {
            return Ok(await _productService.AddAsync(addProduct));

        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateProductRequest updateProduct)
        {
            return Ok(await _productService.UpdateAsync(updateProduct));

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _productService.DeleteAsync(id));

        }

        private string GetClientIp(HttpContext context)
        {
            // First check for X-Forwarded-For (used when behind proxy, load balancer, CDN)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // In case of multiple IPs, the first one is the original client
                var ip = forwardedFor.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }
            }

            // Fallback to remote IP address
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
