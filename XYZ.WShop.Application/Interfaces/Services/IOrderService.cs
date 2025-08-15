using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Dtos.Orders;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ResponseModel<OrderResponse>> AddAsync(CreateOrder createOrder);
        Task<ResponseModel<OrderResponse>> DeleteAsync(Guid id);
        Task<ResponseModel<PagedList<OrderResponse>>> GetAllAsync(
            Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<ResponseModel<PagedList<OrderResponse>>> GetOrderByCustomerIdAsync(
       Guid businessId,
         Guid customerId,
       int page = 1,
       int pageSize = 10,
       string? searchTerm = null);
        Task<ResponseModel<OrderResponse>> GetByIdAsync(Guid id);
        Task<byte[]> ExportOrdersToPdf(Guid businessId, DateTime? startDate = null, DateTime? endDate = null);
        Task<byte[]> ExportOrdersToExcel(Guid businessId, DateTime? startDate = null,
                DateTime? endDate = null);
    }
}
