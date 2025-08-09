using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Dtos.User;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XYZ.WShop.Infrastructure.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/expenses")]
    [ApiController]
    public class ExpensesController : BaseController
    {
        private readonly ILogger<ExpensesController> _logger;
        private readonly IExpenseService _expenseService;

        public ExpensesController(
            ILogger<ExpensesController> logger,
            IExpenseService expenseService)
        {
            _logger = logger;
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] AddExpense addExpense)
        {
            return Ok(await _expenseService.AddAsync(addExpense));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(Guid id)
        {
            return Ok(await _expenseService.GetByIdAsync(id));
        }

        [HttpGet("expense-categories")]
        public async Task<IActionResult> GetExpenseCategories()
        {
            return Ok(ResponseModel<List<ExpenseCategory>>.CreateResponse(ExpenseCategoryData.GetCategories(), "Expense categories retrieved successfully", true));
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses(
            Guid businessId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return Ok(await _expenseService.GetAllAsync(
               businessId,
                page,
                pageSize,
                searchTerm,
                category,
                startDate,
                endDate));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpense updateExpense)
        {
            return Ok(await _expenseService.UpdateAsync(updateExpense));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            return Ok(await _expenseService.DeleteAsync(id));
        }

        [HttpGet("excel")]
        public async Task<IActionResult> ExportToExcel(
            Guid businessId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
        

            var fileContent = await _expenseService.ExportToExcel(businessId, searchTerm, category, startDate, endDate);

            return File(fileContent,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "Expenses.xlsx");
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportToPdf(
        Guid businessId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
        {


            var fileContent = await _expenseService.ExportToPdf(businessId, searchTerm, category, startDate, endDate);

            return File(fileContent, "application/pdf", "Expenses.pdf");
        }
    }
}