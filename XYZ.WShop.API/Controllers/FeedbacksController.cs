using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos.Creditor;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using XZY.WShop.Infrastructure.Services;

namespace XYZ.WShop.API.Controllers
{
    [Route("api/v{version:apiVersion}/feedbacks")]
    public class FeedbacksController : BaseController
    {
        private readonly ILogger<FeedbacksController> _logger;
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ILogger<FeedbacksController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedBack feedBack)
        {
            feedBack.Timestamp = DateTime.UtcNow;
           await _context.FeedBacks.AddRangeAsync(feedBack);
           await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
