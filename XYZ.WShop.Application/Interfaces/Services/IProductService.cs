using System;

using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Domain;

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

        Task<byte[]> ExportProductsToPdf(Guid businessId);
        Task<byte[]> ExportProductsToExcel(Guid businessId);
        Task<byte[]> ExportProductsToPdfQRCode(Guid businessId);
        Task<bool> LogOrderClick(OrderClickProduct orderClick, string deviceType, string clientIp);
    }
}
