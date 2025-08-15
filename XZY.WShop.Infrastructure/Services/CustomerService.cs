using AutoMapper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using OfficeOpenXml;

namespace XZY.WShop.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public CustomerService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseModel<CustomerResponse>> AddAsync(CreateCustomer createCustomer)
        {
            var customer = _mapper.Map<Customer>(createCustomer);
            customer.CreatedDate = DateTime.UtcNow;

            var existingCustomer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c=> c.PhoneNumber == createCustomer.PhoneNumber);

            if (existingCustomer != null)
            {
                throw new BadRequestException("Customer already exist with that phone number");
            }

            await _applicationDbContext.Customers.AddAsync(customer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(customer);

            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                "Customer created successfullY",
                true);
        }

        public async Task<ResponseModel<CustomerResponse>> DeleteAsync(Guid id)
        {
            var customer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {id} not found");
            }

            _applicationDbContext.Customers.Remove(customer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(customer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Customer"),
                true);
        }
        public async Task<byte[]> ExportCustomersToPdf(Guid businessId)
        {
            var customers = await _applicationDbContext.Customers
                .Where(c => c.BusinessId == businessId)
                .ToListAsync();

            var business = await _applicationDbContext.Busineses.FindAsync(businessId);

            // Register encoding provider (important for symbols)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);

                Paragraph title = new Paragraph(business?.Name, titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 5f;
                document.Add(title);

                // Add business contact info
                Font infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var contactInfo = new List<string>();

                if (!string.IsNullOrEmpty(business?.PhoneNumber))
                    contactInfo.Add($"Phone: {business.PhoneNumber}");

                if (!string.IsNullOrEmpty(business?.Email))
                    contactInfo.Add($"Email: {business.Email}");

                if (!string.IsNullOrEmpty(business?.Address))
                    contactInfo.Add($"Address: {business.Address}");

                if (contactInfo.Any())
                {
                    Paragraph infoParagraph = new Paragraph(string.Join(" | ", contactInfo), infoFont);
                    infoParagraph.Alignment = Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 10f;
                    document.Add(infoParagraph);
                }

                // Add customer count
                Font countFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                Paragraph count = new Paragraph($"Total Customers: {customers.Count}", countFont);
                count.Alignment = Element.ALIGN_CENTER;
                count.SpacingAfter = 20f;
                document.Add(count);

                // Create table
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 3, 2, 3, 2 });

                // Table headers
                AddTableHeader(table, "First Name");
                AddTableHeader(table, "Last Name");
                AddTableHeader(table, "Email");
                AddTableHeader(table, "Phone");
                AddTableHeader(table, "Address");
                AddTableHeader(table, "Tags");

                // Table data
                foreach (var customer in customers)
                {
                    AddTableCell(table, customer.FirstName);
                    AddTableCell(table, customer.LastName);
                    AddTableCell(table, customer.Email ?? string.Empty);
                    AddTableCell(table, customer.PhoneNumber);
                    AddTableCell(table, customer.Address ?? string.Empty);
                    AddTableCell(table, customer.Tags ?? string.Empty);
                }

                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportCustomersToExcel(Guid businessId)
        {
            var customers = await _applicationDbContext.Customers
                .Where(c => c.BusinessId == businessId)
                .ToListAsync();

            var business = await _applicationDbContext.Busineses.FindAsync(businessId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Customers");

                // Add title
                worksheet.Cells["A1"].Value = business?.Name ?? "Chidimma Venture";
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add business contact info
                var contactInfo = new List<string>();

                if (!string.IsNullOrEmpty(business?.PhoneNumber))
                    contactInfo.Add($"Phone: {business.PhoneNumber}");

                if (!string.IsNullOrEmpty(business?.Email))
                    contactInfo.Add($"Email: {business.Email}");

                if (!string.IsNullOrEmpty(business?.Address))
                    contactInfo.Add($"Address: {business.Address}");

                if (contactInfo.Any())
                {
                    worksheet.Cells["A2"].Value = string.Join(" | ", contactInfo);
                    worksheet.Cells["A2:F2"].Merge = true;
                    worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A2"].Style.Font.Size = 10;
                }

                // Add customer count
                worksheet.Cells["A3"].Value = $"Total Customers: {customers.Count}";
                worksheet.Cells["A3:F3"].Merge = true;
                worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Set column headers
                worksheet.Cells[4, 1].Value = "First Name";
                worksheet.Cells[4, 2].Value = "Last Name";
                worksheet.Cells[4, 3].Value = "Email";
                worksheet.Cells[4, 4].Value = "Phone";
                worksheet.Cells[4, 5].Value = "Address";
                worksheet.Cells[4, 6].Value = "Tags";

                // Format headers
                using (var range = worksheet.Cells[4, 1, 4, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                int row = 5;
                foreach (var customer in customers)
                {
                    worksheet.Cells[row, 1].Value = customer.FirstName;
                    worksheet.Cells[row, 2].Value = customer.LastName;
                    worksheet.Cells[row, 3].Value = customer.Email;
                    worksheet.Cells[row, 4].Value = customer.PhoneNumber;
                    worksheet.Cells[row, 5].Value = customer.Address;
                    worksheet.Cells[row, 6].Value = customer.Tags;
                    row++;
                }

                // Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        private void AddTableHeader(PdfPTable table, string headerText)
        {
            PdfPCell cell = new PdfPCell(new Phrase(headerText));
            cell.BackgroundColor = new BaseColor(200, 200, 200);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        private void AddTableCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        public async Task<ResponseModel<PagedList<CustomerResponse>>> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _applicationDbContext
                .Customers
                .Where(c=> c.BusinessId == businessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    (c.FirstName != null && c.FirstName.Contains(searchTerm)) ||
                    (c.LastName != null && c.LastName.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Contains(searchTerm)) ||
                    (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm)) ||
                    (c.Tags != null &&  c.Tags.Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderByDescending(c => c.CreatedDate)
                .ThenBy(c => c.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerResponses = _mapper.Map<List<CustomerResponse>>(customers);
            var pagedList = new PagedList<CustomerResponse>(customerResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<CustomerResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true,metaData: new
                {
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<CustomerResponse>> GetByIdAsync(Guid id)
        {
            var customer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {id} not found");
            }

            var result = _mapper.Map<CustomerResponse>(customer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<CustomerResponse>> UpdateAsync(UpdateCustomer updateCustomer)
        {
            var existingCustomer = await _applicationDbContext.Customers.FirstOrDefaultAsync(c => c.Id == updateCustomer.Id && c.BusinessId == updateCustomer.BusinessId);

            if (existingCustomer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {updateCustomer.Id} not found");
            }

            _mapper.Map(updateCustomer, existingCustomer);
            _applicationDbContext.Customers.Update(existingCustomer);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<CustomerResponse>(existingCustomer);
            return ResponseModel<CustomerResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Customer"),
                true);
        }
    }
}