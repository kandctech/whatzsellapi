
using XYZ.WShop.Application.Dtos.Dashboard;
using XYZ.WShop.Application.Dtos.Reports;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<ReportResponseDto> GetReportDataAsync(Guid businessId, ReportRequestDto request);
        Task<DashboardResponse> GetReportDashbordAsync(Guid businessId);
    }
}