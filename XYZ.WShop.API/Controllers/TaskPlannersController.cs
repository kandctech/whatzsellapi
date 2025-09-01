
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Task;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/task-planners")]
    public class TaskPlannersController : BaseController
    {
        private readonly ILogger<TaskPlannersController> _logger;
        private readonly ITaskService _taskService;

        public TaskPlannersController(
            ILogger<TaskPlannersController> logger,
            ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _taskService.GetTaskByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
           [FromQuery] Guid businessId,
           [FromQuery] string? viewMode = "daily",
           [FromQuery] DateTime? selectedDate = null)
        {
            try
            {
                DateTime targetDate = selectedDate ?? DateTime.Today;
              ResponseModel<PagedList<TaskRequestResponse>> tasks;

                if (viewMode?.ToLower() == "daily")
                {
                    var startUtc = targetDate.Date.ToUniversalTime();
                    var endUtc = targetDate.AddDays(1).AddTicks(-1).ToUniversalTime();
                    tasks = await _taskService.GetTasksByDateRangeAsync(startUtc, endUtc, businessId);
                }
                else // weekly view
                {
                    var startOfWeek = targetDate.AddDays(-(int)targetDate.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(6);
                    tasks = await _taskService.GetTasksByDateRangeAsync(startOfWeek, endOfWeek, businessId);
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTaskRequest createTaskRequest)
        {
            return Ok(await _taskService.AddTaskAsync(createTaskRequest));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTaskRequest updateTaskRequest)
        {
            return Ok(await _taskService.UpdateTaskAsync(updateTaskRequest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _taskService.DeleteTaskAsync(id));
        }
    }
}