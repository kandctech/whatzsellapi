
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Ticket;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Domain.Enums;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<ResponseModel<PagedList<TicketDto>>> GetAllTicketsAsync(Guid businessId, int page= 1, int pageSize = 10);
        Task<ResponseModel<TicketDto>> GetTicketByIdAsync(Guid id, Guid businessId);
        Task<ResponseModel<TicketDto>> CreateTicketAsync(CreateTicketDto createTicketDto);
        Task<ResponseModel<TicketDto>> UpdateTicketStatusAsync(Guid id, TicketStatus status);
        Task<ResponseModel<TicketDto>> DeleteTicketAsync(Guid id);
        Task<ResponseModel<TicketDto>> AddReplyToTicketAsync(Guid ticketId, CreateReplyDto createReplyDto);
    }
}
