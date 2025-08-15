using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<ResponseModel<CustomerResponse>> AddAsync(CreateCustomer createCustomer);

        Task<ResponseModel<CustomerResponse>> DeleteAsync(Guid id);

        Task<ResponseModel<PagedList<CustomerResponse>>> GetAllAsync(
            Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null);

        Task<ResponseModel<CustomerResponse>> GetByIdAsync(Guid id);

        Task<ResponseModel<CustomerResponse>> UpdateAsync(UpdateCustomer updateCustomer);

        Task<byte[]> ExportCustomersToPdf(Guid businessId);
        Task<byte[]> ExportCustomersToExcel(Guid businessId);
    }
}