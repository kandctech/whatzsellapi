
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Helpers;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<ResponseModel<ExpenseResponse>> AddAsync(AddExpense addExpense);
        Task<ResponseModel<ExpenseResponse>> DeleteAsync(Guid id);
        Task<ResponseModel<PagedList<ExpenseResponse>>> GetAllAsync(
            Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? category = null,
            DateTime? startDate = null,
            DateTime? endDate = null, bool isReport = false);
        Task<ResponseModel<ExpenseResponse>> GetByIdAsync(Guid id);
        Task<ResponseModel<ExpenseResponse>> UpdateAsync(UpdateExpense updateExpense);

        Task<byte[]> ExportToExcel(Guid businessId,
                string? searchTerm = null,
                string? category = null,
                DateTime? startDate = null,
                DateTime? endDate = null);

        Task<byte[]> ExportToPdf(Guid businessId,
        string? searchTerm = null,
        string? category = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
    }
}
