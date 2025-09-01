
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Task;
using XYZ.WShop.Application.Helpers;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<ResponseModel<PagedList<TaskRequestResponse>>> GetAllTasksAsync(Guid businessId, int page= 1, int pageSize = 10);
        Task<ResponseModel<PagedList<TaskRequestResponse>>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate, Guid businessId);
        Task<ResponseModel<TaskRequestResponse>> GetTaskByIdAsync(Guid id);
        Task<ResponseModel<TaskRequestResponse>> AddTaskAsync(CreateTaskRequest task);
        Task<ResponseModel<TaskRequestResponse>> UpdateTaskAsync(UpdateTaskRequest task);
        Task<ResponseModel<TaskRequestResponse>> DeleteTaskAsync(Guid id);
    }
}
