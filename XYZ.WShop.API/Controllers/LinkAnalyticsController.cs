using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    [Route("api/v{version:apiVersion}/link-analytics")]
    [ApiController]
    public class LinkAnalyticsController : BaseController
    {
        private readonly ILogger<LinkAnalyticsController> _logger;
        private readonly ILinkAnalyticService _linkAnalyticService;

        public LinkAnalyticsController(ILogger<LinkAnalyticsController> logger, ILinkAnalyticService linkAnalyticService)
        {
            _logger = logger;
            _linkAnalyticService = linkAnalyticService;
        }

        [HttpGet("link-performance")]
        public IActionResult GetLinkPerformance(
        [FromQuery] Guid businessId,
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = _linkAnalyticService.GetLinkPerformance(businessId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("link-performance/custom")]
        public IActionResult GetCustomLinkPerformance(
            [FromQuery] Guid businessId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest("Start date cannot be after end date");

                var result = _linkAnalyticService.GetLinkPerformance(businessId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private DateRange GetDateRange(string period)
        {
            var today = DateTime.Today;

            return period.ToLower() switch
            {
                "today" => new DateRange(today, today.AddDays(1).AddTicks(-1)),
                "week" => new DateRange(
                    today.AddDays(-(int)today.DayOfWeek),
                    today.AddDays(6 - (int)today.DayOfWeek).AddDays(1).AddTicks(-1)),
                "month" => new DateRange(
                    new DateTime(today.Year, today.Month, 1),
                    new DateTime(today.Year, today.Month, 1).AddMonths(1).AddTicks(-1)),
                _ => new DateRange(today.AddDays(-7), today.AddDays(1).AddTicks(-1)) // Default to week
            };
        }

        private record DateRange(DateTime StartDate, DateTime EndDate);
    }
}
