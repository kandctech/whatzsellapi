using AutoMapper;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace XZY.WShop.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ExpenseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<ExpenseResponse>> AddAsync(AddExpense addExpense)
        {
            var expense = _mapper.Map<Expense>(addExpense);
            expense.CreatedDate = DateTime.UtcNow;
            expense.CreatedBy = addExpense.UserId;

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<ExpenseResponse>(expense);
            return ResponseModel<ExpenseResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Expense"),
                true);
        }

        public async Task<ResponseModel<ExpenseResponse>> DeleteAsync(Guid id)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
            {
                throw new EntityNotFoundException($"Expense with ID {id} not found");
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<ExpenseResponse>(expense);
            return ResponseModel<ExpenseResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Expense"),
                true);
        }

        public async Task<ResponseModel<PagedList<ExpenseResponse>>> GetAllAsync(Guid businessId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? category = null,
            DateTime? startDate = null,
            DateTime? endDate = null, bool isReport = false)
        {
            var query = _context.Expenses
                .Where(e => e.BusinessId == businessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.Description.Contains(searchTerm));

            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Category.ToLower() == category.ToLower());
            }

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime(); 
                query = query.Where(e => e.ExpenseDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.AddDays(1).AddTicks(-1).ToUniversalTime(); 
                query = query.Where(e => e.ExpenseDate <= endUtc);
            }

            var totalCount = await query.CountAsync();
            var total = query.Sum(exp => exp.Amount);

            List<Expense> expenses = new List<Expense>();

            if (!isReport)
            {
                 expenses = await query
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                expenses = await query
                                .OrderByDescending(e => e.ExpenseDate)
                                .ThenByDescending(e => e.CreatedDate)
                                .ToListAsync();
            }

            var expenseResponses = _mapper.Map<List<ExpenseResponse>>(expenses);
            var pagedList = new PagedList<ExpenseResponse>(expenseResponses, totalCount, page, pageSize);

            var usersIds = expenses.Select(x => x.CreatedBy.ToString()).ToList();

            var usersDic = _context.Users.Where(u => usersIds.Contains(u.Id)).ToDictionary(u => u.Id);

           foreach (var item in pagedList)
            {
                var user = usersDic.GetValueOrDefault(item.UserId.ToString());
               item.CreatedByFullName = $"{user?.FirstName} {user?.LastName}";
            }

            return ResponseModel<PagedList<ExpenseResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true, metaData: new { Total = total, 
                    pagedList.HasNext,
                TotalCount = totalCount});
        }

        public async Task<ResponseModel<ExpenseResponse>> GetByIdAsync(Guid id)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
            {
                throw new EntityNotFoundException($"Expense with ID {id} not found");
            }

            var result = _mapper.Map<ExpenseResponse>(expense);
            return ResponseModel<ExpenseResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<ExpenseResponse>> UpdateAsync(UpdateExpense updateExpense)
        {
            var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == updateExpense.Id);

            if (existingExpense == null)
            {
                throw new EntityNotFoundException($"Expense with ID {updateExpense.Id} not found");
            }

            _mapper.Map(updateExpense, existingExpense);
            _context.Expenses.Update(existingExpense);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<ExpenseResponse>(existingExpense);

            var user = await _context.Users.FindAsync(result.UserId.ToString());
            result.CreatedByFullName = $"{user?.FirstName} {user?.FirstName}";
            return ResponseModel<ExpenseResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Expense"),
                true);
        }

        public async Task<byte[]> ExportToExcel(Guid businessId,
            string? searchTerm = null,
            string? category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();

            var expensesResult = await GetAllAsync(businessId, searchTerm: searchTerm, category: category, startDate: startDate, endDate: endDate, isReport: true);

            if(expensesResult == null || expensesResult?.Data?.Count == 0)
            {
                throw new BadRequestException("No expense records to export");
            }

            if (expensesResult != null) {
                expenses = expensesResult?.Data?? new  List<ExpenseResponse>();
            }


            var catDic = ExpenseCategoryData.GetCategories().ToDictionary(e => e.Id);


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Expenses");

                // Add headers
                worksheet.Cell(1, 1).Value = "Business ID";
                worksheet.Cell(1, 2).Value = "Amount";
                worksheet.Cell(1, 3).Value = "Category";
                worksheet.Cell(1, 4).Value = "Created By";
                worksheet.Cell(1, 5).Value = "Description";
                worksheet.Cell(1, 6).Value = "Expense Date";

                // Style headers
                var headerRange = worksheet.Range("A1:G1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add data
                for (int i = 0; i < expenses.Count; i++)
                {
                    var expense = expenses[i];

                    var categoryItem = catDic.GetValueOrDefault(expense.Category);

                    worksheet.Cell(i + 2, 1).Value = expense.BusinessId.ToString();
                    worksheet.Cell(i + 2, 2).Value = expense.Amount;
                    worksheet.Cell(i + 2, 3).Value = categoryItem != null ? categoryItem.Label : "N/A";
                    worksheet.Cell(i + 2, 4).Value =  expense.CreatedByFullName ?? "N/A";
                    worksheet.Cell(i + 2, 5).Value = expense.Description;
                    worksheet.Cell(i + 2, 6).Value = expense.ExpenseDate;
                }

                // Auto fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> ExportToPdf(Guid businessId,
            string? searchTerm = null,
            string? category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();

            var expensesResult = await GetAllAsync(businessId, searchTerm: searchTerm, category: category, startDate: startDate, endDate: endDate, isReport: true);

            if (expensesResult == null || expensesResult?.Data?.Count == 0)
            {
                throw new BadRequestException("No expense records to export");
            }

            if (expensesResult != null)
            {
                expenses = expensesResult?.Data ?? new List<ExpenseResponse>();
            }

            var catDic = ExpenseCategoryData.GetCategories().ToDictionary(e => e.Id);

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("Expense Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph(" ")); // Add space

                // Create table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Add headers
                AddCell(table, "Amount", true);
                AddCell(table, "Category", true);
                AddCell(table, "Created By", true);
                AddCell(table, "Description", true);
                AddCell(table, "Expense Date", true);

                // Add data
                foreach (var expense in expenses)
                {
                    AddCell(table, expense.Amount.ToString("C"));
                    AddCell(table, expense.Category);
                    AddCell(table, expense.CreatedByFullName);
                    AddCell(table, expense.Description);
                    AddCell(table, expense.ExpenseDate.ToShortDateString());
                }

                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }

        private void AddCell(PdfPTable table, string text, bool isHeader = false)
        {
            var cell = new PdfPCell(new Phrase(text));
            if (isHeader)
            {
                cell.BackgroundColor = new BaseColor(200, 200, 200);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
            }
            table.AddCell(cell);
        }
    }
}