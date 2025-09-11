using AutoMapper;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OfficeOpenXml;
using XYZ.WShop.Application.Dtos.Product;

namespace XZY.WShop.Infrastructure.Services
    {
        public class OrderService : IOrderService
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly IProductService _productService;

            public OrderService(
                ApplicationDbContext context,
                IMapper mapper,
                IProductService productService)
            {
                _context = context;
                _mapper = mapper;
                _productService = productService;
            }

            public async Task<ResponseModel<OrderResponse>> AddAsync(CreateOrder createOrder)
            {
            var helper = new SubscriptionHelper();
            await helper.ValidateSubscriptionAsync(createOrder.BusinessId, _context);
            // Validate products exist and have sufficient stock
            await ValidateOrderItems(createOrder.OrderItems);

                var order = _mapper.Map<Order>(createOrder);
                order.CreatedDate = DateTime.UtcNow;

                // Calculate total amount
                order.Amount = createOrder.OrderItems.Sum(item => item.Price * item.Qty);

            string last10;

            // Check if string is at least 10 characters long
            if (order.CustomerPhoneNumber.Length >= 10)
            {
                last10 = order.CustomerPhoneNumber.Substring(order.CustomerPhoneNumber.Length - 10);
            }
            else
            {
                last10 = order.CustomerPhoneNumber; 
            }

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c=> c.PhoneNumber.Contains(last10) && c.BusinessId == createOrder.BusinessId);


            var customerId = Guid.NewGuid();

            if (existingCustomer == null)
            {
                var names = createOrder.CustomerName.Split(new char[] {' ',',', ';'});
                    var newCustomer = new Customer
                    {
                        Id = customerId,
                        Address = createOrder.CustomerAddress,
                        FirstName = names[0],
                        LastName = names.Length > 1 ? names[1] : names[0],
                        PhoneNumber = createOrder.CustomerPhoneNumber,
                        BusinessId = createOrder.BusinessId,
                        LastOrderDate = DateTime.UtcNow,
                        
                    };
                    _context.Customers.Add(newCustomer);

            }

            if(existingCustomer != null)
            {
                existingCustomer.LastOrderDate = DateTime.UtcNow;   
            }

            var orderId = Guid.NewGuid();
            order.Id = orderId;
            order.CustomerId = existingCustomer != null? existingCustomer.Id : customerId;
            order.PaymentDate = createOrder.PaymentDate.ToUniversalTime();
            await _context.Orders.AddAsync(order);

            var count = await _context.Orders.Where(o=> o.BusinessId == order.BusinessId).CountAsync();

            order.OrderNumber = count.ToString("D6");    

            await _context.SaveChangesAsync();

            var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Order"),
                    true);
            }

            public async Task<ResponseModel<OrderResponse>> DeleteAsync(Guid id)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    throw new EntityNotFoundException($"Order with ID {id} not found");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Order"),
                    true);
            }

            public async Task<ResponseModel<PagedList<OrderResponse>>> GetAllAsync(
                Guid businessId,
                int page = 1,
                int pageSize = 10,
                string? searchTerm = null,
                DateTime? startDate = null,
                DateTime? endDate = null)
            {
                var query = _context.Orders
                    .Where(o => o.BusinessId == businessId)
                    .OrderByDescending(q => q.CreatedDate)
                    .Include(o => o.OrderItems)
                    .AsQueryable();

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                query = query.Where(e => e.PaymentDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.AddDays(1).AddTicks(-1).ToUniversalTime();
                query = query.Where(e => e.PaymentDate <= endUtc);
            }

            if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(o =>
                        o.CustomerName.ToLower().Contains(searchTerm.ToLower()) ||
                        o.CustomerEmail.ToLower().Contains(searchTerm.ToLower()) ||
                         o.OrderNumber.ToLower().Contains(searchTerm.ToLower()) ||
                          o.OrderItems.Any(item=> item.ProductName.ToLower().Contains(searchTerm.ToLower())) ||
                       o.OrderItems.Any(item => item.ProductDescription.ToLower().Contains(searchTerm.ToLower())) ||
                        o.CustomerPhoneNumber.Contains(searchTerm));
                }

                var totalCount = await query.CountAsync();
            decimal totalSales = query.Sum(o => o.OrderItems.Sum(oi => oi.Amount * oi.Qty));

            // Calculate average order value
            decimal averageOrderValue = totalCount > 0 ? totalSales / totalCount : 0;
            var orders = await query
                    .OrderByDescending(o => o.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
                var pagedList = new PagedList<OrderResponse>(orderResponses, totalCount, page, pageSize);

                return ResponseModel<PagedList<OrderResponse>>.CreateResponse(
                    pagedList,
                    ApplicationContants.Messages.RetrievedSuccessfully,
                    true, metaData: new
                    {
                        HasNext = pagedList.HasNext,
                        TotalCount = totalCount,
                        TotalSales = totalSales,
                        AverageOrderValue = averageOrderValue,
                    });
            }

            public async Task<ResponseModel<OrderResponse>> GetByIdAsync(Guid id)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    throw new EntityNotFoundException($"Order with ID {id} not found");
                }

                var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    ApplicationContants.Messages.RetrievedSuccessfully,
                    true);
            }

        public async Task<ResponseModel<PagedList<OrderResponse>>> GetOrderByCustomerIdAsync(Guid businessId, Guid customerId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _context.Orders
                .Where(o => o.BusinessId == businessId && o.CustomerId == customerId)
                .OrderByDescending(q => q.CreatedDate)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(o =>
                    o.CustomerName.Contains(searchTerm) ||
                    o.CustomerEmail.Contains(searchTerm) ||
                     o.OrderNumber.Contains(searchTerm) ||
                      o.OrderItems.Any(item => item.ProductName.Contains(searchTerm)) ||
                   o.OrderItems.Any(item => item.ProductDescription.Contains(searchTerm)) ||
                    o.CustomerPhoneNumber.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            // Calculate total spent
            decimal totalSpent = query.Sum(o => o.OrderItems.Sum(oi => oi.Amount * oi.Qty));

            // Calculate average order value
            decimal averageOrderValue = totalCount > 0 ? totalSpent / totalCount : 0;

            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            var pagedList = new PagedList<OrderResponse>(orderResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<OrderResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true, metaData: new
                {
                   HasNext = pagedList.HasNext,
                   TotalSpent = totalSpent,
                   AverageOrderValue = averageOrderValue,
                });
        }

        public async Task<byte[]> ExportOrdersToPdf(Guid businessId, DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.BusinessId == businessId)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                query = query.Where(e => e.PaymentDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.AddDays(1).AddTicks(-1).ToUniversalTime();
                query = query.Where(e => e.PaymentDate <= endUtc);
            }

            query = query.OrderByDescending(o => o.PaymentDate);

            var orders = await query.ToListAsync();

            var business = await _context.Busineses.FindAsync(businessId);

            // Register encoding provider
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create A4 document with proper margins
                Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add document metadata
                document.AddTitle($"{business?.Name} Order Report");
                document.AddSubject("Order history report");
                document.AddKeywords("orders, report, sales");
                document.AddCreator("WhatSell");
                document.AddAuthor(business?.Name ?? "Seller");

                // ==================== HEADER SECTION ====================
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 30, 70 });

                // Add WhatSell logo
                try
                {
                    var logo = CreateWhatSellLogo();
                    PdfPCell logoCell = new PdfPCell(logo);
                    logoCell.Border = Rectangle.NO_BORDER;
                    logoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    headerTable.AddCell(logoCell);
                }
                catch
                {
                    PdfPCell textLogoCell = new PdfPCell(new Phrase("WhatSell",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(7, 94, 84))));
                    textLogoCell.Border = Rectangle.NO_BORDER;
                    headerTable.AddCell(textLogoCell);
                }

                // Business information
                PdfPCell businessInfoCell = new PdfPCell();
                businessInfoCell.Border = Rectangle.NO_BORDER;
                businessInfoCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                Font businessNameFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font businessContactFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font addressFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                // Create business information paragraph
                Paragraph contactInfo = new Paragraph();
                contactInfo.Add(new Chunk(business?.Name ?? "Business Name", businessNameFont));
                contactInfo.Add(Chunk.Newline);
                contactInfo.Add(new Chunk(business?.Email ?? "contact@business.com", businessContactFont));
                contactInfo.Add(Chunk.Newline);
                contactInfo.Add(new Chunk(business?.PhoneNumber ?? "+123 456 7890", businessContactFont));

                // Add address if available
                if (!string.IsNullOrEmpty(business?.Address))
                {
                    contactInfo.Add(Chunk.Newline);
                    contactInfo.Add(new Chunk(business.Address, addressFont));
                }

                businessInfoCell.AddElement(contactInfo);
                headerTable.AddCell(businessInfoCell);
                document.Add(headerTable);

                // Add divider line
                document.Add(new Chunk(new LineSeparator(0.5f, 100f, new BaseColor(7, 94, 84), Element.ALIGN_CENTER, -1)));

                // ==================== REPORT TITLE SECTION ====================
                document.Add(new Paragraph(" "));

                Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(7, 94, 84));
                Paragraph reportTitle = new Paragraph("Order History Report", sectionFont);
                reportTitle.SpacingBefore = 15f;
                reportTitle.SpacingAfter = 10f;
                reportTitle.Alignment = Element.ALIGN_CENTER;
                document.Add(reportTitle);

                // Add report date
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph reportDate = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}", dateFont);
                reportDate.Alignment = Element.ALIGN_CENTER;
                reportDate.SpacingAfter = 15f;
                document.Add(reportDate);

                // ==================== ORDERS SECTION ====================
                string currencySymbol = business?.Currency ?? "$";
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font currencyFont = new Font(baseFont, 10);

                foreach (var order in orders)
                {
                    // Add order header
                    Font orderHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    Paragraph orderHeader = new Paragraph($"Order #: {order.OrderNumber} | Date: {order.CreatedDate:yyyy-MM-dd HH:mm} | Customer: {order.CustomerName}", orderHeaderFont);
                    orderHeader.SpacingAfter = 5f;
                    document.Add(orderHeader);

                    // Add order details
                    Font orderDetailsFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    Paragraph orderDetails = new Paragraph();
                    orderDetails.Add(new Phrase($"Phone: {order.CustomerPhoneNumber ?? "N/A"} | ", orderDetailsFont));
                    orderDetails.Add(new Phrase($"Amount: {currencySymbol}{order.Amount.ToString("N2")} | ", orderDetailsFont));
                    orderDetails.Add(new Phrase($"Payment: {order.PaymentMethod}", orderDetailsFont));
                    orderDetails.SpacingAfter = 10f;
                    document.Add(orderDetails);

                    // Create items table
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 4, 2, 2 });

                    // Table headers
                    AddTableHeader(table, "Product");
                    AddTableHeader(table, "Price");
                    AddTableHeader(table, "Qty");

                    // Table data
                    foreach (var item in order.OrderItems)
                    {
                        AddTableCell(table, item.ProductName);

                        PdfPCell priceCell = new PdfPCell(new Phrase($"{currencySymbol}{item.Amount.ToString("N2")}", currencyFont));
                        priceCell.Padding = 5f;
                        table.AddCell(priceCell);

                        AddTableCell(table, item.Qty.ToString());
                    }

                    document.Add(table);

                    // Add spacer between orders
                    document.Add(new Paragraph(" "));
                    document.Add(new Chunk(LineSeparator));
                    document.Add(new Paragraph(" "));
                }

                // ==================== FOOTER SECTION ====================
                AddFooter(document, business);

                document.Close();
                return memoryStream.ToArray();
            }
        }

        // Helper method to create WhatSell logo
        private Image CreateWhatSellLogo()
        {
            // Create a simple text-based logo
            Chunk chunk = new Chunk("WhatSell",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(255, 255, 255)));

            // Create a colored background
            PdfPTable table = new PdfPTable(1);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.DefaultCell.BackgroundColor = new BaseColor(7, 94, 84);
            table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.DefaultCell.FixedHeight = 40f;
            table.TotalWidth = 100f;
            table.LockedWidth = true;

            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(7, 94, 84);
            table.AddCell(cell);

            // Convert table to image
            return PdfImageFromTable(table);
        }

        private Image PdfImageFromTable(PdfPTable table)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document tempDoc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(tempDoc, ms);
                tempDoc.Open();
                tempDoc.Add(table);
                tempDoc.Close();

                return Image.GetInstance(ms.ToArray());
            }
        }

        // Helper method to add footer
        private void AddFooter(Document document, Business business)
        {
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.WidthPercentage = 100;
            footerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            Paragraph footer = new Paragraph();
            footer.Add(new Chunk("Generated by WhatSell - Your Sales Management Solution", footerFont));
            footer.Add(Chunk.Newline);
            footer.Add(new Chunk($"© {DateTime.Now.Year} WhatSell. All rights reserved.", footerFont));
            footer.Add(Chunk.Newline);
            footer.Add(new Chunk(business?.Name ?? "Your Business", footerFont));

            PdfPCell footerCell = new PdfPCell(footer);
            footerCell.Border = Rectangle.NO_BORDER;
            footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTable.AddCell(footerCell);

            document.Add(footerTable);
        }

        public async Task<byte[]> ExportOrdersToExcel(Guid businessId, DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.BusinessId == businessId)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                query = query.Where(e => e.PaymentDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.AddDays(1).AddTicks(-1).ToUniversalTime();
                query = query.Where(e => e.PaymentDate <= endUtc);
            }

            query = query.OrderByDescending(o => o.PaymentDate);

            var orders = await query.ToListAsync();

            var business = await _context.Busineses.FindAsync(businessId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");

                // Set column headers
                worksheet.Cells[1, 1].Value = "Order #";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "Customer";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Amount";
                worksheet.Cells[1, 6].Value = "Payment Method";
                worksheet.Cells[1, 7].Value = "Items";

                // Format headers
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.OrderNumber;
                    worksheet.Cells[row, 2].Value = order.CreatedDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 3].Value = order.CustomerName;
                    worksheet.Cells[row, 4].Value = order.CustomerPhoneNumber;
                    worksheet.Cells[row, 5].Value = $"{business?.Currency}{order.Amount.ToString("N2")}";
                    worksheet.Cells[row, 6].Value = order.PaymentMethod;

                    // Combine order items into a single cell
                    var itemsText = string.Join(Environment.NewLine,
                        order.OrderItems.Select(i => $"{i.ProductName} - {i.Qty} x {business?.Currency}{i.Amount.ToString("N2")} = {business?.Currency}{(i.Qty * i.Amount).ToString("N2")}"));
                    worksheet.Cells[row, 7].Value = itemsText;
                    worksheet.Cells[row, 7].Style.WrapText = true;

                    row++;
                }

                // Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        // Helper methods
        private void AddTableHeader(PdfPTable table, string headerText)
        {
            PdfPCell cell = new PdfPCell(new Phrase(headerText));
            cell.BackgroundColor = new BaseColor(240, 240, 240);
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        private void AddTableCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        private LineSeparator LineSeparator
        {
            get
            {
                LineSeparator line = new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_CENTER, -1);
                return line;
            }
        }

        private async Task ValidateOrderItems(List<OrderItemDto> orderItems)
            {
                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                    {
                        throw new EntityNotFoundException($"Product with ID {item.ProductId} not found");
                    }

                    if(product.QuantityInStock < item.Qty)
                    {
                    throw new BadRequestException($"Product {item.Name} has only {product.QuantityInStock} left. Please update the product quantity before placing creating this sale.");
                    }

                    product.QuantityInStock -= item.Qty;
                   
                }
            }
        }
    }
