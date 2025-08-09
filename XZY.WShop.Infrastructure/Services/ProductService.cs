using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        public ProductService(ApplicationDbContext applicationDbContext, IMapper mappper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mappper;
        }

        public async Task<ResponseModel<ProductResponse>> AddAsync(AddProduct addProduct)
        {
            var product = _mapper.Map<Product>(addProduct);
            await _applicationDbContext.Products.AddAsync(product);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(product);
            return ResponseModel<ProductResponse>.CreateResponse(result, "Product added successfully", true);
        }

        public async Task<ResponseModel<ProductResponse>> DeleteAsync(Guid id)
        {
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new EntityNotFoundException($"Product with ID {id} not found");
            }

            _applicationDbContext.Products.Remove(product);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(product);
            return ResponseModel<ProductResponse>.CreateResponse(result, "Product deleted successfully", true);
        }

        public async Task<ResponseModel<BusinessResponse>> GetAllBySlug(string deviceType, string clientIp, string slug, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var busness = await _applicationDbContext.Busineses.FirstOrDefaultAsync(b => b.Slug == slug);

            if (busness == null)
            {
                throw new EntityNotFoundException($"Business not found");
            }

            var query = _applicationDbContext
                .Products
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedDate)
                .Where(q => q.BusinessId == busness.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            var geo = await GetCityCountry(clientIp);
            var saleActivity = new SaleActivity
            {
                BusinessId = busness.Id,
                IsCatalogCliked = true,
                LinkedView = true,
                DeviceType = deviceType,
                CreatedDate = DateTime.UtcNow,
                Source = "Unknown",
                ProductId = Guid.Empty,
                OrderClicked = false,
                Country = geo.Country,
                City = geo.City,
                IsActive = true,

            };

            _applicationDbContext.SaleActivities.Add(saleActivity);
            await _applicationDbContext.SaveChangesAsync();

            var businessResponse = new BusinessResponse
            {
                BusinessId = busness.Id,
                Products = productResponses,
                Name = busness.Name,
                Description = busness.Description ?? string.Empty,
                Category = busness.Category,
                Slug = slug,

            };

            return ResponseModel<BusinessResponse>.CreateResponse(
                businessResponse,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }
        public async Task<ResponseModel<PagedList<ProductResponse>>> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _applicationDbContext.Products
                .Where(q => q.BusinessId == businessId)
                .OrderByDescending(q=> q.CreatedDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            var pagedList = new PagedList<ProductResponse>(productResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<ProductResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<ProductResponse>> GetByIdAsync(Guid id, string deviceType, string clientIp)
        {
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new EntityNotFoundException($"Product not found");
            }

            var result = _mapper.Map<ProductResponse>(product);

            var business = await _applicationDbContext.Busineses.FirstOrDefaultAsync(p => p.Id == product.BusinessId);

            result.PhoneNumber = business?.PhoneNumber;
            result.BusinessName = business?.Name;
            result.Slug = business?.Slug;

            var geo = await GetCityCountry(clientIp);
            var saleActivity = new SaleActivity
            {
                BusinessId = business.Id,
                IsCatalogCliked = false,
                LinkedView = true,
                DeviceType = deviceType,
                CreatedDate = DateTime.UtcNow,
                Source = "Unknown",
                ProductId = product.Id,
                OrderClicked = false,
                Country = geo.Country,
                City = geo.City,
                IsActive = true
            };

            _applicationDbContext.SaleActivities.Add(saleActivity);
            await _applicationDbContext.SaveChangesAsync();

            return ResponseModel<ProductResponse>.CreateResponse(result, ApplicationContants.Messages.RetrievedSuccessfully, true);
        }

        public async Task<ResponseModel<ProductResponse>> UpdateAsync(UpdateProductRequest updateProduct)
        {
            var existingProduct = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == updateProduct.Id);

            if (existingProduct == null)
            {
                throw new EntityNotFoundException($"Product with ID {updateProduct.Id} not found");
            }

            _mapper.Map(updateProduct, existingProduct);
            existingProduct.ImageUrls = string.Join(",", updateProduct.ImageUrls);

            _applicationDbContext.Products.Update(existingProduct);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(existingProduct);

            return ResponseModel<ProductResponse>.CreateResponse(result, "Product updated successfully", true);
        }
        public async Task<(string City, string Country)> GetCityCountry(string ip)
        {
            using var client = new HttpClient();
            var result = await client.GetStringAsync($"https://ipwho.is/{ip}");

            var json = JsonDocument.Parse(result);
            string city = json.RootElement.GetProperty("city").GetString();
            string country = json.RootElement.GetProperty("country").GetString();

            return (city, country);
        }

    }
}