
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Ticket;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain.Enums;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/tickets")]
    public class TicketsController : BaseController
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly ITicketService _ticketService;

        public TicketsController(
            ILogger<TicketsController> logger,
            ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, Guid businessId)
        {
            return Ok(await _ticketService.GetTicketByIdAsync(id, businessId));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            Guid businessId,
            int page = 1,
            int pageSize = 10)
        {
            return Ok(await _ticketService.GetAllTicketsAsync(businessId, page, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTicketDto createTicket)
        {
            return Ok(await _ticketService.CreateTicketAsync(createTicket));
        }

        [HttpPost("{ticketId}/replies")]
        public async Task<IActionResult> AddReply(Guid ticketId, [FromBody] CreateReplyDto createReply)
        {
            return Ok(await _ticketService.AddReplyToTicketAsync(ticketId, createReply));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TicketStatus status)
        {
            return Ok(await _ticketService.UpdateTicketStatusAsync(id, status));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _ticketService.DeleteTicketAsync(id));
        }
    }
}