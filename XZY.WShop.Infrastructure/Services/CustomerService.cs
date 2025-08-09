using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public CustomerService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<CustomerResponse>> AddAsync(CreateCustomer createCustomer)
        {
            var customer = _mapper.Map<Customer>(createCustomer);
            customer.CreatedDate = DateTime.UtcNow;

            var existingCustomer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c=> c.PhoneNumber == createCustomer.PhoneNumber);

            if (existingCustomer != null)
            {
                throw new BadRequestException("Customer already exist with that phone number");
            }

            await _applicationDbContext.Customers.AddAsync(customer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(customer);

            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                "Customer created successfullY",
                true);
        }

        public async Task<ResponseModel<CustomerResponse>> DeleteAsync(Guid id)
        {
            var customer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {id} not found");
            }

            _applicationDbContext.Customers.Remove(customer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(customer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Customer"),
                true);
        }

        public async Task<ResponseModel<PagedList<CustomerResponse>>> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _applicationDbContext
                .Customers
                .Where(c=> c.BusinessId == businessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(searchTerm) ||
                    c.LastName.Contains(searchTerm) ||
                    c.Email.Contains(searchTerm) ||
                    c.PhoneNumber.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerResponses = _mapper.Map<List<CustomerResponse>>(customers);
            var pagedList = new PagedList<CustomerResponse>(customerResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<CustomerResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<CustomerResponse>> GetByIdAsync(Guid id)
        {
            var customer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {id} not found");
            }

            var result = _mapper.Map<CustomerResponse>(customer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<CustomerResponse>> UpdateAsync(UpdateCustomer updateCustomer)
        {
            var existingCustomer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == updateCustomer.Id);

            if (existingCustomer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {updateCustomer.Id} not found");
            }

            _mapper.Map(updateCustomer, existingCustomer);
            _applicationDbContext.Customers.Update(existingCustomer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(existingCustomer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Customer"),
                true);
        }
    }
}