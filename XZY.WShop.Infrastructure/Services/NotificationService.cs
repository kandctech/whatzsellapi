using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Notification;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public NotificationService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResponseModel<NotificationResponse>> AddAsync(NotificationRequest request)
        {
            var notification = _mapper.Map<Notification>(request);
            notification.CreatedDate = DateTime.UtcNow;

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<NotificationResponse>(notification);
            return ResponseModel<NotificationResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Notification"),
                true);
        }

        public async Task<ResponseModel<NotificationResponse>> DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(e => e.Id == id);

            if (notification == null)
            {
                throw new EntityNotFoundException($"Notification with ID {id} not found");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<NotificationResponse>(notification);
            return ResponseModel<NotificationResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Expense"),
                true);
        }

        public async Task<ResponseModel<PagedList<NotificationResponse>>> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Notifications
                .Where(e => e.BusinessId == businessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.Message.Contains(searchTerm));

            }

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                query = query.Where(e => e.CreatedDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.AddDays(1).AddTicks(-1).ToUniversalTime();
                query = query.Where(e => e.CreatedDate <= endUtc);
            }

            var totalCount = await query.CountAsync();

            var notifications = await query
               .OrderByDescending(c => c.CreatedDate)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            var notificationResponses = _mapper.Map<List<NotificationResponse>>(notifications);

            var pagedList = new PagedList<NotificationResponse>(notificationResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<NotificationResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true, metaData: new
                {
                    HasMore = pagedList.HasNext
                } );

        }

        public async Task<ResponseModel<NotificationResponse>> GetByIdAsync(Guid id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(e => e.Id == id);

            if (notification == null)
            {
                throw new EntityNotFoundException($"Notification with ID {id} not found");
            }

            var result = _mapper.Map<NotificationResponse>(notification);
            return ResponseModel<NotificationResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<bool>> MarkAllRead(Guid businessId)
        {
            var records = await _context.Notifications
                .Where(b => b.Id == businessId && !b.IsRead)
                .ToListAsync();

            foreach (var notification in records)
            {
                {
                    notification.IsRead = true;
                }
            }


                var isSaved = await _context.SaveChangesAsync() > 0;

                var result = ResponseModel<bool>.CreateResponse(
                   isSaved,
                   "Updated Successfully",
                   true);

                return result;
        }
        public async Task<ResponseModel<bool>> MarkAsRead(Guid id)
        {
            var record = await _context.Notifications
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsRead);

                if(record != null)
            {
                record.IsRead = true;
            }
            

          var isSaved = await _context.SaveChangesAsync() > 0;

            var result = ResponseModel<bool>.CreateResponse(
               isSaved,
               "Updated Successfully",
               true);

            return result;
        }

        public  async Task<ResponseModel<int>> UnReadCount(Guid businessId)
        {
           var unReadCount = await _context.Notifications
                .Where(b=> b.Id == businessId && !b.IsRead)
                .CountAsync();

            var result = ResponseModel<int>.CreateResponse(
               unReadCount,
               ApplicationContants.Messages.RetrievedSuccessfully,
               true);

            return result;
        }
    }
}
