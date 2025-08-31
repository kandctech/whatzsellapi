using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Followup;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class FollowUpService : IFollowUpService
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public FollowUpService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<FollowUpResponse>> CreateFollowUpAsync(Guid userId, CreateFollowUpRequest request)
        {
            string last10;

            // Check if string is at least 10 characters long
            if (request.CustomerPhone.Length >= 10)
            {
                last10 = request.CustomerPhone.Substring(request.CustomerPhone.Length - 10);
            }
            else
            {
                last10 = request.CustomerPhone;
            }

            var existingCustomer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.PhoneNumber.Contains(last10) && c.BusinessId == request.BusinessId);


            var customerId = Guid.NewGuid();

            if (existingCustomer == null)
            {
                var names = request.CustomerName.Split(new char[] { ' ', ',', ';' });
                var newCustomer = new Customer
                {
                    Id = customerId,
                    Address = "Not Provided",
                    FirstName = names[0],
                    LastName = names.Length > 1 ? names[1] : names[0],
                    PhoneNumber = request.CustomerPhone,
                    BusinessId = request.BusinessId,

                };
                _applicationDbContext.Customers.Add(newCustomer);
                request.CustomerId = customerId.ToString();
            }
            else
            {
                request.CustomerId = existingCustomer.Id.ToString();
            }

            var followUp = _mapper.Map<FollowUp>(request);
            followUp.CreatedDate = DateTime.UtcNow;

            await _applicationDbContext.FollowUps.AddAsync(followUp);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<FollowUpResponse>(followUp);

            result.CustomerName = string.IsNullOrEmpty( request.CustomerName)? $"{existingCustomer?.FirstName} {existingCustomer?.LastName}" : $"{request.CustomerName}";

            result.CustomerPhone = string.IsNullOrEmpty( request.CustomerPhone) ? $"{existingCustomer?.PhoneNumber}" : $"{request.CustomerPhone}";

            result.CustomerEmail = string.IsNullOrEmpty( request.CustomerEmail)? $"{existingCustomer?.Email}" : $"{request.CustomerEmail}";

            return ResponseModel<FollowUpResponse>.CreateResponse(
                result,
                "FollowUp created successfullY",
                true);
        }

        public async Task<ResponseModel<FollowUpResponse>> DeleteFollowUpAsync(Guid id)
        {
            var followUp = await _applicationDbContext.FollowUps.FirstOrDefaultAsync(c => c.Id == id);

            if (followUp == null)
            {
                throw new EntityNotFoundException($"FollowUp with ID {id} not found");
            }

            _applicationDbContext.FollowUps.Remove(followUp);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<FollowUpResponse>(followUp);
            return ResponseModel<FollowUpResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "FollowUp"),
                true);
        }

        public async Task<ResponseModel<FollowUpResponse>> GetFollowUpByIdAsync(Guid id)
        {
            var followUp = await _applicationDbContext.FollowUps.FirstOrDefaultAsync(c => c.Id == id);

            if (followUp == null)
            {
                throw new EntityNotFoundException($"FollowUp with ID {id} not found");
            }

            var result = _mapper.Map<FollowUpResponse>(followUp);

            return ResponseModel<FollowUpResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<PagedList<FollowUpResponse>>> GetFollowUpsAsync(Guid businessId, string? status = null, int page = 1, int pageSize = 10)
        {
            var query = _applicationDbContext
                  .FollowUps
                  .Where(c => c.BusinessId == businessId)
                  .AsQueryable();


            if (status != "all")
            {
                query = query.Where(c =>
                    (c.Status == status));

            }

            var totalCount = await query.CountAsync();
            var followUps = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerIds = followUps.Select(c => c.CustomerId)
                .ToList();

            var customersDic = _applicationDbContext.Customers.Where(c=> customerIds.Contains(c.Id))
                .ToDictionary(c=> c.Id);

            var followUpResponses = _mapper.Map<List<FollowUpResponse>>(followUps);

            foreach (var item in followUpResponses)
            {
                if (customersDic.ContainsKey(item.CustomerId))
                {
                    var customer = customersDic.GetValueOrDefault(item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = $"{customer?.FirstName} {customer?.LastName}";
                        item.CustomerPhone = $"{customer?.PhoneNumber}";
                        item.CustomerEmail = $"{ customer?.Email?? "N/A"}";
                    }
                }
            }
            var pagedList = new PagedList<FollowUpResponse>(followUpResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<FollowUpResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true, metaData: new
                {
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<FollowUpResponse>> ToggleFollowUpStatusAsync(Guid id)
        {
            var followUp = await _applicationDbContext.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                throw new EntityNotFoundException("Follow up not found");
            }

            followUp.Status = followUp.Status == "completed" ? "pending" : "completed";
            followUp.ModifiedDate = DateTime.Now;

            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<FollowUpResponse>(followUp);

            return ResponseModel<FollowUpResponse>.CreateResponse(
                result,
                "Update successfully.",
                true);
        }

        public async Task<ResponseModel<FollowUpResponse>> UpdateFollowUpAsync(Guid id, UpdateFollowUpRequest request)
        {
            var followUp = await _applicationDbContext.FollowUps.FindAsync(id);
            if (followUp == null) return null;

            if (!string.IsNullOrEmpty(request.Type)) followUp.Type = request.Type;
            if (request.Date.HasValue) followUp.Date = request.Date.Value;
            if (!string.IsNullOrEmpty(request.Notes)) followUp.Notes = request.Notes;
            if (!string.IsNullOrEmpty(request.Status)) followUp.Status = request.Status;
            if (request.Reminder.HasValue) followUp.Reminder = request.Reminder.Value;

            followUp.ModifiedDate = DateTime.Now;

            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<FollowUpResponse>(followUp);

            return ResponseModel<FollowUpResponse>.CreateResponse(
               result,
               ApplicationContants.Messages.RetrievedSuccessfully,
               true);
        }
    }
}
