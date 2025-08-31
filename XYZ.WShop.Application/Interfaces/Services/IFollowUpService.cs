
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Followup;
using XYZ.WShop.Application.Helpers;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IFollowUpService
    {
        Task<ResponseModel<PagedList<FollowUpResponse>>> GetFollowUpsAsync(Guid businessId, string? status = null, int page = 1, int pageSize = 10);
        Task<ResponseModel<FollowUpResponse>> GetFollowUpByIdAsync(Guid id);
        Task<ResponseModel<FollowUpResponse>> CreateFollowUpAsync(Guid userId, CreateFollowUpRequest request);
        Task<ResponseModel<FollowUpResponse>> UpdateFollowUpAsync(Guid id, UpdateFollowUpRequest request);
        Task<ResponseModel<FollowUpResponse>> DeleteFollowUpAsync(Guid id);
        Task<ResponseModel<FollowUpResponse>> ToggleFollowUpStatusAsync(Guid id);
    }
}
