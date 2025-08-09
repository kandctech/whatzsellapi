using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Reports;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/reports")]
    public class ReportsController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] string reportType = "sales", [FromQuery] string timeRange = "week")
        {
            var request = new ReportRequestDto
            {
                ReportType = reportType,
                TimeRange = timeRange
            };

            var result = await _reportService.GetReportDataAsync(request);
            return Ok(result);
        }
    }
}