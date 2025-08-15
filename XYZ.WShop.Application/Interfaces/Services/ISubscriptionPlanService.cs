using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Dtos.Subscription;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ISubscriptionPlanService
    {
        Task<ResponseModel<SubscriptionPlanResponse>> AddAsync(SubscriptionPlanRequest request);

        Task<ResponseModel<SubscriptionPlanResponse>> DeleteAsync(Guid id);

        Task<ResponseModel<List<SubscriptionPlanResponse>>> GetAllAsync();

        Task<ResponseModel<SubscriptionPlanResponse>> GetByIdAsync(Guid id);

        Task<ResponseModel<SubscriptionPlanResponse>> UpdateAsync(SubscriptionPlanUpdateRequest updateRequest);
    }
}
