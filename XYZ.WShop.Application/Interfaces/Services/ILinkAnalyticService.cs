
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Marketing;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ILinkAnalyticService
    {
      ResponseModel<LinkAnalyticsResponse> GetLinkPerformance(Guid businessId, DateTime startDate, DateTime endDate);
    }
}
