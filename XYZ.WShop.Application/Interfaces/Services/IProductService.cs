using System;

using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Helpers;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ResponseModel<PagedList<ProductResponse>>> GetAllAsync(Guid businessId, int page = 10, int pageSize = 10, string? searchTerm = null);

        Task<ResponseModel<ProductResponse>> AddAsync(AddProduct addProduct);
        Task<ResponseModel<ProductResponse>> UpdateAsync(UpdateProductRequest updateProduct);
        Task<ResponseModel<ProductResponse>> GetByIdAsync(Guid id, string deviceType, string clientIp);
        Task<ResponseModel<ProductResponse>> DeleteAsync(Guid id);
        Task<ResponseModel<BusinessResponse>> GetAllBySlug(string deviceType, string clientIp,string businessName, int page, int pageSize, string? searchTerm);
    }
}
