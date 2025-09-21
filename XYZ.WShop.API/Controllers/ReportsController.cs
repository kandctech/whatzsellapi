using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Reports;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/reports")]
    public class ReportsController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] Guid businessId, [FromQuery] string reportType = "sales", [FromQuery] string timeRange = "week")
        {
            var request = new ReportRequestDto
            {
                ReportType = reportType,
                TimeRange = timeRange
            };

            var result = await _reportService.GetReportDataAsync(businessId, request);
            return Ok(result);
        }

        [HttpGet("dashboard/{businessId}")]
        public async Task<IActionResult> GetDashboardReport(Guid businessId)
        {

            var result = await _reportService.GetReportDashbordAsync(businessId);
            return Ok(result);
        }

        [HttpGet("product-sales")]
        public async Task<IActionResult> DownloadProductSalesReport([FromQuery] DateTime startDate,[FromQuery] DateTime endDate, [FromQuery] Guid businessId)
        {
            try
            {
                var fileContent = await _reportService.GenerateProductSalesReportAsync(startDate, endDate, businessId);

                return File(fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ProductSalesReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating report: {ex.Message}");
            }
        }
    }
}