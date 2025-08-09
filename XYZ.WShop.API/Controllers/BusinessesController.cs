using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/businesses")]
    public class BusinessesController : BaseController
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public BusinessesController(ILogger<ProductsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBySlug(string slug, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();

            string deviceType;
            if (userAgent.Contains("Android"))
                deviceType = "Android";
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                deviceType = "iOS";
            else if (userAgent.Contains("Windows") || userAgent.Contains("Macintosh"))
                deviceType = "Desktop";
            else
                deviceType = "Unknown";

            string clientIp = GetClientIp(HttpContext);

            return Ok(await _productService.GetAllBySlug(deviceType, clientIp, slug, page, pageSize, searchTerm));

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
