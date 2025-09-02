using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Ticket;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XYZ.WShop.Domain.Enums;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public TicketService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<TicketDto>> AddReplyToTicketAsync(Guid ticketId, CreateReplyDto createReplyDto)
        {
            var ticket = await _applicationDbContext.Tickets
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with ID {ticketId} not found");
            }

            var reply = _mapper.Map<Reply>(createReplyDto);
            reply.Id = Guid.NewGuid();
            reply.TicketId = ticketId;
            reply.CreatedDate = DateTime.UtcNow;

            // Update ticket status if admin is replying
            if (createReplyDto.IsAdmin && ticket.Status == TicketStatus.Open)
            {
                ticket.Status = TicketStatus.InProgress;
                ticket.ModifiedDate = DateTime.UtcNow;
            }

            await _applicationDbContext.Replies.AddAsync(reply);
            await _applicationDbContext.SaveChangesAsync();

            var result = await GetTicketByIdAsync(ticketId, ticket.BusinessId);

            return result;
        }

        public async Task<ResponseModel<TicketDto>> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            var ticket = _mapper.Map<Ticket>(createTicketDto);
            ticket.Id = Guid.NewGuid();
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.Status = TicketStatus.Open;

            await _applicationDbContext.Tickets.AddAsync(ticket);

            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TicketDto>(ticket);

            return ResponseModel<TicketDto>.CreateResponse(
                result,
                "Ticket created successfully",
                true);
        }

        public async Task<ResponseModel<TicketDto>> DeleteTicketAsync(Guid id)
        {
            var ticket = await _applicationDbContext.Tickets
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with ID {id} not found");
            }

            // Remove all replies first to avoid foreign key constraints
            _applicationDbContext.Replies.RemoveRange(ticket.Replies);
            _applicationDbContext.Tickets.Remove(ticket);

            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TicketDto>(ticket);

            return ResponseModel<TicketDto>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Ticket"),
                true);
        }

        public async Task<ResponseModel<PagedList<TicketDto>>> GetAllTicketsAsync(Guid businessId, int page = 1, int pageSize = 10)
        {
            var query = _applicationDbContext.Tickets
                .Include(t => t.Replies)
                .Where(t => t.BusinessId == businessId)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var tickets = await query
                .OrderByDescending(t => t.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);

            var pagedList = new PagedList<TicketDto>(ticketDtos, totalCount, page, pageSize);

            return ResponseModel<PagedList<TicketDto>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true,
                metaData: new
                {
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<TicketDto>> GetTicketByIdAsync(Guid id, Guid businessId)
        {
            var ticket = await _applicationDbContext.Tickets
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == id && t.BusinessId == businessId);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with ID {id} not found");
            }

            var result = _mapper.Map<TicketDto>(ticket);

            return ResponseModel<TicketDto>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<TicketDto>> UpdateTicketStatusAsync(Guid id, TicketStatus status)
        {
            var ticket = await _applicationDbContext.Tickets.FindAsync(id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with ID {id} not found");
            }

            ticket.Status = status;
            ticket.ModifiedDate = DateTime.UtcNow;

            if (status == TicketStatus.Resolved || status == TicketStatus.Closed)
            {
                ticket.ResolvedDate = DateTime.UtcNow;
            }

            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<TicketDto>(ticket);

            return ResponseModel<TicketDto>.CreateResponse(
                result,
                "Ticket status updated successfully",
                true);
        }
    }
}