

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Dtos.Subscription;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public SubscriptionPlanService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<SubscriptionPlanResponse>> AddAsync(SubscriptionPlanRequest request)
        {
            var subscriptionPlan = _mapper.Map<SubscriptionPlan>(request);

            await _applicationDbContext.SubscriptionPlans.AddAsync(subscriptionPlan);

            await _applicationDbContext.SaveChangesAsync();

            var subscriptionPlanRespnse = _mapper.Map<SubscriptionPlanResponse
                >(subscriptionPlan);

            return ResponseModel<SubscriptionPlanResponse>.CreateResponse(
                subscriptionPlanRespnse,
                string.Format(ApplicationContants.Messages.CreatedSuccessfully, "Subscription plan"),
                true);
        }

        public async Task<ResponseModel<SubscriptionPlanResponse>> DeleteAsync(Guid id)
        {
            var subscriptionPlan = await _applicationDbContext.SubscriptionPlans.FirstOrDefaultAsync(c => c.Id == id);

            if (subscriptionPlan == null)
            {
                throw new EntityNotFoundException($"Subscription Plan with ID {id} not found");
            }

            _applicationDbContext.SubscriptionPlans.Remove(subscriptionPlan);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<SubscriptionPlanResponse>(subscriptionPlan);

            return ResponseModel<SubscriptionPlanResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Subscription Plan"),
                true);
        }

        public async Task<ResponseModel<List<SubscriptionPlanResponse>>> GetAllAsync()
        {
            var subscriptionPlans = await _applicationDbContext.SubscriptionPlans.ToListAsync();

            var subscriptionPlansResponse = _mapper.Map<List<SubscriptionPlanResponse>>(subscriptionPlans);

            return ResponseModel<List<SubscriptionPlanResponse>>.CreateResponse(
                subscriptionPlansResponse,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Subscription Plan"),
                true);


        }

        public async Task<ResponseModel<SubscriptionPlanResponse>> GetByIdAsync(Guid id)
        {
            var subscriptionPlan = await _applicationDbContext.SubscriptionPlans.FirstOrDefaultAsync(c => c.Id == id);

            if (subscriptionPlan == null)
            {
                throw new EntityNotFoundException($"Subscription plan with ID {id} not found");
            }

            var result = _mapper.Map<SubscriptionPlanResponse>(subscriptionPlan);
            return ResponseModel<SubscriptionPlanResponse>.CreateResponse(
                result,
                "Retrieved successfully",
                true);
        }

        public async Task<ResponseModel<SubscriptionPlanResponse>> UpdateAsync(SubscriptionPlanUpdateRequest updateRequest)
        {
            var existingSubscriptionPlan = await _applicationDbContext.SubscriptionPlans.FirstOrDefaultAsync(c => c.Id == updateRequest.Id);

            if (existingSubscriptionPlan == null)
            {
                throw new EntityNotFoundException($"Subscription plan with ID {updateRequest.Id} not found");
            }

            _mapper.Map(updateRequest, existingSubscriptionPlan);
            _applicationDbContext.SubscriptionPlans.Update(existingSubscriptionPlan);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<SubscriptionPlanResponse>(existingSubscriptionPlan);
            return ResponseModel<SubscriptionPlanResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Subscription plan"),
                true);
        }
    }
}
