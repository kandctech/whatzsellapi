using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Dtos.Notification;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ResponseModel<NotificationResponse>> AddAsync(NotificationRequest request);
        Task<ResponseModel<NotificationResponse>> DeleteAsync(Guid id);
        Task<ResponseModel<PagedList<NotificationResponse>>> GetAllAsync(
            Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<ResponseModel<NotificationResponse>> GetByIdAsync(Guid id);

        Task<ResponseModel<int>> UnReadCount(Guid businessId);
        Task<ResponseModel<bool>> MarkAsRead(Guid id);

        Task<ResponseModel<bool>> MarkAllRead(Guid businessId);
    }
}
