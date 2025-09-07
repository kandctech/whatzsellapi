using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Dtos.Task;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public TaskService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<TaskRequestResponse>> AddTaskAsync(CreateTaskRequest task)
        {
            var taskPlanner = _mapper.Map<TaskPlanner>(task);
            taskPlanner.CreatedDate = DateTime.UtcNow;

            await _applicationDbContext.Tasks.AddAsync(taskPlanner);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TaskRequestResponse>(taskPlanner);
            return ResponseModel<TaskRequestResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Task"),
                true);
        }

        public async Task<ResponseModel<TaskRequestResponse>> DeleteTaskAsync(Guid id)
        {
            var task = await _applicationDbContext.Tasks.FirstOrDefaultAsync(e => e.Id == id);

            if (task == null)
            {
                throw new EntityNotFoundException($"Expense with ID {id} not found");
            }

            _applicationDbContext.Tasks.Remove(task);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TaskRequestResponse>(task);
            return ResponseModel<TaskRequestResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Task"),
                true);
        }

        public async Task<ResponseModel<PagedList<TaskRequestResponse>>> GetAllTasksAsync(Guid businessId, int page = 1, int pageSize = 10)
        {
            var query = _applicationDbContext.Tasks.Where(t => t.BusinessId == businessId);

            var totalCount = await query.CountAsync();

            var tasks  = await query
            .OrderBy(t => t.Date).ThenBy(t => t.Time)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var tasksResponses = _mapper.Map<List<TaskRequestResponse>>(tasks);
            var pagedList = new PagedList<TaskRequestResponse>(tasksResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<TaskRequestResponse>>.CreateResponse(
             pagedList,
             ApplicationContants.Messages.RetrievedSuccessfully,
             true, metaData: new
             {
                 Total = totalCount,
                 pagedList.HasNext,
                 TotalCount = totalCount
             });

        }

        public async Task<ResponseModel<TaskRequestResponse>> GetTaskByIdAsync(Guid id)
        {
            var task = await _applicationDbContext.Tasks.FirstOrDefaultAsync(e => e.Id == id);

            if (task == null)
            {
                throw new EntityNotFoundException($"Task with ID {id} not found");
            }

            var result = _mapper.Map<TaskRequestResponse>(task);
            return ResponseModel<TaskRequestResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<PagedList<TaskRequestResponse>>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate, Guid businessId)
        {
            var query = _applicationDbContext.Tasks
                .Where(t => t.BusinessId == businessId && t.Date >= startDate && t.Date <= endDate);

            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderBy(t => t.Date).ThenBy(t => t.Time)
                .ToListAsync();

            var tasksResponses = _mapper.Map<List<TaskRequestResponse>>(tasks);
            var pagedList = new PagedList<TaskRequestResponse>(tasksResponses, totalCount, 1, 1000);

            return ResponseModel<PagedList<TaskRequestResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true,
                metaData: new
                {
                    Total = totalCount,
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<TaskRequestResponse>> UpdateTaskAsync(UpdateTaskRequest task)
        {
            var existingTask = await _applicationDbContext.Tasks.FirstOrDefaultAsync(e => e.Id == task.Id);

            if (existingTask == null)
            {
                throw new EntityNotFoundException($"Task with ID {task.Id} not found");
            }

            _mapper.Map(task, existingTask);
            _applicationDbContext.Tasks.Update(existingTask);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TaskRequestResponse>(existingTask);
            return ResponseModel<TaskRequestResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Task"),
                true);
        }
    }
}
