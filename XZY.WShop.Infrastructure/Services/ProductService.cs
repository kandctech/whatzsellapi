using AutoMapper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using OfficeOpenXml;
using iTextSharp.text.pdf.draw;
using QRCoder;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace XZY.WShop.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public ProductService(ApplicationDbContext applicationDbContext, IMapper mappper, IConfiguration config)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mappper;
            _config = config;
        }

        public async Task<ResponseModel<ProductResponse>> AddAsync(AddProduct addProduct)
        {
            var product = _mapper.Map<Product>(addProduct);
            await _applicationDbContext.Products.AddAsync(product);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(product);
            return ResponseModel<ProductResponse>.CreateResponse(result, "Product added successfully", true);
        }

        public async Task<ResponseModel<ProductResponse>> DeleteAsync(Guid id)
        {
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new EntityNotFoundException($"Product with ID {id} not found");
            }

            _applicationDbContext.Products.Remove(product);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(product);
            return ResponseModel<ProductResponse>.CreateResponse(result, "Product deleted successfully", true);
        }

        public async Task<ResponseModel<BusinessResponse>> GetAllBySlug(string deviceType, string clientIp, string slug, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var busness = await _applicationDbContext.Busineses.FirstOrDefaultAsync(b => b.Slug == slug);

            if (busness == null)
            {
                throw new EntityNotFoundException($"Business not found");
            }

            var query = _applicationDbContext
                .Products
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedDate)
                .Where(q => q.BusinessId == busness.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            var geo = await GetCityCountry(clientIp);
            var saleActivity = new SaleActivity
            {
                BusinessId = busness.Id,
                IsCatalogCliked = true,
                LinkedView = true,
                DeviceType = deviceType,
                CreatedDate = DateTime.UtcNow,
                Source = "Unknown",
                ProductId = Guid.Empty,
                OrderClicked = false,
                Country = geo.Country,
                City = geo.City,
                IsActive = true,

            };

            _applicationDbContext.SaleActivities.Add(saleActivity);
            await _applicationDbContext.SaveChangesAsync();

            var businessResponse = new BusinessResponse
            {
                BusinessId = busness.Id,
                Products = productResponses,
                Name = busness.Name,
                PhoneNumber = busness.PhoneNumber,
                Description = busness.Description ?? string.Empty,
                Category = busness.Category,
                Slug = slug,

            };

            return ResponseModel<BusinessResponse>.CreateResponse(
                businessResponse,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }
        public async Task<ResponseModel<PagedList<ProductResponse>>> GetAllAsync(Guid businessId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _applicationDbContext.Products
                .Where(q => q.BusinessId == businessId)
                .OrderByDescending(q=> q.CreatedDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Description.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            var pagedList = new PagedList<ProductResponse>(productResponses, totalCount, page, pageSize);

            return ResponseModel<PagedList<ProductResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true, metaData: new
                {
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<ProductResponse>> GetByIdAsync(Guid id, string deviceType, string clientIp)
        {
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new EntityNotFoundException($"Product not found");
            }

            var result = _mapper.Map<ProductResponse>(product);

            var business = await _applicationDbContext.Busineses.FirstOrDefaultAsync(p => p.Id == product.BusinessId);

            result.PhoneNumber = business?.PhoneNumber;
            result.BusinessName = business?.Name;
            result.Slug = business?.Slug;

            var geo = await GetCityCountry(clientIp);
            var saleActivity = new SaleActivity
            {
                BusinessId = business.Id,
                IsCatalogCliked = false,
                LinkedView = true,
                DeviceType = deviceType,
                CreatedDate = DateTime.UtcNow,
                Source = "Unknown",
                ProductId = product.Id,
                OrderClicked = false,
                Country = geo.Country,
                City = geo.City,
                IsActive = true
            };

            _applicationDbContext.SaleActivities.Add(saleActivity);
            await _applicationDbContext.SaveChangesAsync();

            return ResponseModel<ProductResponse>.CreateResponse(result, ApplicationContants.Messages.RetrievedSuccessfully, true);
        }

        public async Task<ResponseModel<ProductResponse>> UpdateAsync(UpdateProductRequest updateProduct)
        {
            var existingProduct = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == updateProduct.Id);

            if (existingProduct == null)
            {
                throw new EntityNotFoundException($"Product with ID {updateProduct.Id} not found");
            }

            _mapper.Map(updateProduct, existingProduct);
            existingProduct.ImageUrls = string.Join(",", updateProduct.ImageUrls);

            _applicationDbContext.Products.Update(existingProduct);
            await _applicationDbContext.SaveChangesAsync();

            var result = _mapper.Map<ProductResponse>(existingProduct);

            return ResponseModel<ProductResponse>.CreateResponse(result, "Product updated successfully", true);
        }
        public async Task<(string City, string Country)> GetCityCountry(string ip)
        {
            using var client = new HttpClient();
            var result = await client.GetStringAsync($"https://ipwho.is/{ip}");

            var json = JsonDocument.Parse(result);
            string city = json.RootElement.GetProperty("city").GetString();
            string country = json.RootElement.GetProperty("country").GetString();

            return (city, country);
        }


        public async Task<byte[]> ExportProductsToPdf(Guid businessId)
        {
            var products = await _applicationDbContext.Products
               .Where(p => p.BusinessId == businessId)
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
                Paragraph title = new Paragraph("Product List", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Create table
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 3, 2, 2, 2, 3 });

                // Table headers
                AddTableHeader(table, "Name");
                AddTableHeader(table, "Description");
                AddTableHeader(table, "Unit Price");
                AddTableHeader(table, "Quantity");
                AddTableHeader(table, "Cost Price");
                AddTableHeader(table, "Images");

                string currencySymbol = business?.Currency ?? "$";

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font currencyFont = new Font(baseFont, 10);

                // Table data
                foreach (var product in products)
                {
                    AddTableCell(table, product.Name);
                    AddTableCell(table, product.Description ?? string.Empty);

                    // Unit Price with proper formatting
                    if (product.UnitPrice != null)
                    {
                        PdfPCell priceCell = new PdfPCell(new Phrase($"{currencySymbol}{product.UnitPrice.Value.ToString("N2")}", currencyFont));
                        priceCell.Padding = 5f;
                        table.AddCell(priceCell);
                    }
                    else
                    {
                        AddTableCell(table, "N/A");
                    }

                    AddTableCell(table, product.QuantityInStock?.ToString() ?? "N/A");

                    // Cost Price with proper formatting
                    PdfPCell costCell = new PdfPCell(new Phrase($"{currencySymbol}{product.CostPrice.ToString("N2")}", currencyFont));
                    costCell.Padding = 5f;
                    table.AddCell(costCell);

                    // Handle images
                    if (!string.IsNullOrEmpty(product.ImageUrls))
                    {
                        var imageUrls = product.ImageUrls.Split(',');
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(imageUrls[0]))
                            {
                                Image image = Image.GetInstance(imageUrls[0].Trim());
                                image.ScaleToFit(60f, 60f);
                                PdfPCell imageCell = new PdfPCell(image);
                                imageCell.Padding = 5f;
                                table.AddCell(imageCell);
                            }
                        }
                        catch
                        {
                            AddTableCell(table, "Image not available");
                        }
                    }
                    else
                    {
                        AddTableCell(table, "No images");
                    }
                }

                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }
        public async Task<byte[]> ExportProductsToExcel(Guid businessId)
        {
            var products = await _applicationDbContext.Products
                .Where(p=> p.BusinessId == businessId)
                .ToListAsync();

            var business = await _applicationDbContext.Busineses.FindAsync(businessId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Set column headers
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Unit Price";
                worksheet.Cells[1, 4].Value = "Quantity";
                worksheet.Cells[1, 5].Value = "Cost Price";
                worksheet.Cells[1, 6].Value = "Images";

                // Format headers
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Name;
                    worksheet.Cells[row, 2].Value = product.Description;
                    worksheet.Cells[row, 3].Value = $"{business?.Currency}{product.UnitPrice}";
                    worksheet.Cells[row, 4].Value = product.QuantityInStock;
                    worksheet.Cells[row, 5].Value = $"{business?.Currency}{product.CostPrice}";

                    // Handle images
                    if (!string.IsNullOrEmpty(product.ImageUrls))
                    {
                        var imageUrls = product.ImageUrls.Split(',');
                        int imageCol = 6;
                       
                            if (!string.IsNullOrWhiteSpace(imageUrls[0]))
                            {
                                try
                                {
                                    var image = worksheet.Drawings.AddPicture($"img_{row}_{imageCol}", new FileInfo(imageUrls[0].Trim()));
                                    image.SetPosition(row - 1, 0, imageCol - 1, 0);
                                    image.SetSize(60, 60);
                                }
                                catch
                                {
                                    worksheet.Cells[row, imageCol].Value = "Image error";
                                }
                            }
                         
                    }

                    row++;
                }

                // Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }


        public async Task<byte[]> ExportProductsToPdfQRCode(Guid businessId)
        {
            var products = await _applicationDbContext.Products
               .Where(p => p.BusinessId == businessId)
               .ToListAsync();

            var business = await _applicationDbContext.Busineses.FindAsync(businessId);

            // Register encoding provider
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create A4 document with proper margins
                Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add document metadata
                document.AddTitle($"{business?.Name} Product Catalog");
                document.AddSubject("Product catalog with QR codes");
                document.AddKeywords("product catalog, QR code, ordering");
                document.AddCreator("WhatSell");
                document.AddAuthor(business?.Name ?? "Seller");

                // ==================== HEADER SECTION ====================
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

                // Seller information
                PdfPCell sellerInfoCell = new PdfPCell();
                sellerInfoCell.Border = Rectangle.NO_BORDER;
                sellerInfoCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                Font sellerNameFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font sellerContactFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font addressFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                // Create contact information paragraph
                Paragraph contactInfo = new Paragraph();
                contactInfo.Add(new Chunk(business?.Name ?? "Seller Name", sellerNameFont));
                contactInfo.Add(Chunk.Newline);
                contactInfo.Add(new Chunk(business?.Email ?? "contact@whatsell.com", sellerContactFont));
                contactInfo.Add(Chunk.Newline);
                contactInfo.Add(new Chunk(business?.PhoneNumber ?? "+123 456 7890", sellerContactFont));

                // Add address if available
                if (!string.IsNullOrEmpty(business?.Address))
                {
                    contactInfo.Add(Chunk.Newline);
                    contactInfo.Add(new Chunk(business.Address, addressFont));
                }

                sellerInfoCell.AddElement(contactInfo);
                headerTable.AddCell(sellerInfoCell);
                document.Add(headerTable);

                // Add divider line
                document.Add(new Chunk(new LineSeparator(0.5f, 100f, new BaseColor(7, 94, 84), Element.ALIGN_CENTER, -1)));

                // ==================== CATALOG QR SECTION ====================
                document.Add(new Paragraph(" "));

                var cBaseUrl = _config.GetValue<string>("CatalogWebBaseUrl");
                string catalogUrl = $"{cBaseUrl}/?slug={business?.Slug}";
                AddMasterQrCode(document, catalogUrl, business?.Name);

                // ==================== PRODUCT TABLE SECTION ====================
                document.Add(new Paragraph(" "));

                Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(7, 94, 84));
                Paragraph productSection = new Paragraph($"Product Catalog({business.Currency})", sectionFont);
                productSection.SpacingBefore = 15f;
                productSection.SpacingAfter = 10f;
                document.Add(productSection);

                // Adjusted to 5 columns after removing cost and stock
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 3, 3, 2, 3 }); // Adjusted column widths
                table.SpacingBefore = 10f;

                // Updated table headers
                AddTableHeader(table, "Image");
                AddTableHeader(table, "Name");
                AddTableHeader(table, "Description");
                AddTableHeader(table, "Price");
                AddTableHeader(table, "Order QR Code");

                string currencySymbol = business?.Currency ?? "$";
                var productBaseUrl = _config.GetValue<string>("ProductWebBaseUrl");
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font currencyFont = new Font(baseFont, 10);
                Font productNameFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                foreach (var product in products)
                {
                    // Product Image
                    AddProductImageCell(table, product);

                    // Product Name
                    PdfPCell nameCell = new PdfPCell(new Phrase(product.Name, productNameFont));
                    nameCell.Padding = 5f;
                    nameCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(nameCell);

                    // Product Description
                    AddTableCell(table, TruncateDescription(product.Description), Element.ALIGN_LEFT);

                    // Unit Price
                    AddCurrencyCell(table, product.UnitPrice, currencySymbol, currencyFont);

                    // QR Code
                    string productUrl = $"{productBaseUrl}?productId={product.Id}";
                    AddProductQrCodeCell(table, qrGenerator, productUrl);
                }

                document.Add(table);

                // ==================== FOOTER SECTION ====================
                AddFooter(document, business);

                document.Close();
                return memoryStream.ToArray();
            }
        }
        // ==================== HELPER METHODS ====================

        private Image CreateWhatSellLogo()
        {
            // Create a simple logo programmatically
            int width = 150;
            int height = 50;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);

            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                // Background
                g.Clear(System.Drawing.Color.White);

                // Draw logo elements
                var logoColor = System.Drawing.Color.FromArgb(7, 94, 84);
                var font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);

                // Draw logo text
                g.DrawString("WhatSell", font, new System.Drawing.SolidBrush(logoColor), 10, 10);

                // Draw a simple graphic element
                g.FillRectangle(new System.Drawing.SolidBrush(logoColor), 0, height - 5, width, 5);
            }

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Image.GetInstance(ms.ToArray());
            }
        }

        private void AddMasterQrCode(Document document, string catalogUrl, string businessName)
        {
            PdfPTable qrTable = new PdfPTable(2);
            qrTable.WidthPercentage = 80;
            qrTable.HorizontalAlignment = Element.ALIGN_CENTER;
            qrTable.SpacingAfter = 20f;

            // QR Code Cell
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(catalogUrl, QRCodeGenerator.ECCLevel.H))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                var qrCodeBytes = qrCode.GetGraphic(15, new byte[] { 7, 94, 84 }, new byte[] { 255, 255, 255 }); // Using brand color

                using (var qrStream = new MemoryStream(qrCodeBytes))
                {
                    Image qrImage = Image.GetInstance(qrStream.ToArray());
                    qrImage.ScaleToFit(150f, 150f);

                    PdfPCell qrCell = new PdfPCell(qrImage);
                    qrCell.Border = Rectangle.NO_BORDER;
                    qrCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    qrCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    qrTable.AddCell(qrCell);
                }
            }

            // Text Cell
            PdfPCell textCell = new PdfPCell();
            textCell.Border = Rectangle.NO_BORDER;
            textCell.Padding = 10f;
            textCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(7, 94, 84));
            Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Font urlFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.Blue);

            textCell.AddElement(new Paragraph("Digital Product Catalog", headerFont));
            textCell.AddElement(new Paragraph(" "));
            textCell.AddElement(new Paragraph($"Scan this QR code to access the complete {businessName ?? "product"} catalog online at any time.", textFont));
            textCell.AddElement(new Paragraph(" "));
            textCell.AddElement(new Paragraph("Or visit:", textFont));
            textCell.AddElement(new Paragraph(catalogUrl, urlFont));

            qrTable.AddCell(textCell);
            document.Add(qrTable);
        }

        private void AddProductImageCell(PdfPTable table, Product product)
        {
            PdfPCell imageCell;

            if (!string.IsNullOrEmpty(product.ImageUrls))
            {
                var imageUrls = product.ImageUrls.Split(',');
                try
                {
                    if (!string.IsNullOrWhiteSpace(imageUrls[0]))
                    {
                        Image image = Image.GetInstance(imageUrls[0].Trim());
                        image.ScaleToFit(60f, 60f);
                        imageCell = new PdfPCell(image);
                        imageCell.Padding = 5f;
                        imageCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(imageCell);
                        return;
                    }
                }
                catch
                {
                    // Fall through to default
                }
            }

            // Default image placeholder
            imageCell = new PdfPCell(new Phrase("No image", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8)));
            imageCell.Padding = 5f;
            imageCell.HorizontalAlignment = Element.ALIGN_CENTER;
            imageCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(imageCell);
        }

        private string TruncateDescription(string description, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(description)) return "-";
            return description.Length > maxLength ? description.Substring(0, maxLength) + "..." : description;
        }

        private void AddCurrencyCell(PdfPTable table, decimal? value, string currencySymbol, Font font)
        {
            string text = value != null ? $"{currencySymbol}{value.Value.ToString("N2")}" : "N/A";
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Padding = 5f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);
        }

        private void AddStockCell(PdfPTable table, int? quantity)
        {
            string text = quantity != null ? quantity.Value.ToString() : "N/A";
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.Padding = 5f;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Add color coding for stock levels
            if (quantity.HasValue)
            {
                if (quantity <= 0) cell.BackgroundColor = new BaseColor(255, 200, 200); // Light red for out of stock
                else if (quantity <= 5) cell.BackgroundColor = new BaseColor(255, 255, 200); // Light yellow for low stock
            }

            table.AddCell(cell);
        }

        private void AddProductQrCodeCell(PdfPTable table, QRCodeGenerator qrGenerator, string productUrl)
        {
            using (var qrCodeData = qrGenerator.CreateQrCode(productUrl, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                var qrCodeBytes = qrCode.GetGraphic(5, new byte[] { 7, 94, 84 }, new byte[] { 255, 255, 255 }); // Brand color QR

                using (var qrStream = new MemoryStream(qrCodeBytes))
                {
                    Image qrImage = Image.GetInstance(qrStream.ToArray());
                    qrImage.ScaleToFit(80f, 80f);

                    PdfPCell qrCell = new PdfPCell();
                    qrCell.Padding = 5f;
                    qrCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    qrCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    // Add QR code with instruction
                    qrCell.AddElement(qrImage);
                    qrCell.AddElement(new Phrase("Scan to order",
                        FontFactory.GetFont(FontFactory.HELVETICA, 7, new BaseColor(7, 94, 84))));

                    table.AddCell(qrCell);
                }
            }
        }

        private void AddTableHeader(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.White)));
            cell.BackgroundColor = new BaseColor(7, 94, 84); // WhatSell brand color
            cell.Padding = 5f;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        private void AddFooter(Document document, Business business)
        {
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.WidthPercentage = 100;
            footerTable.SpacingBefore = 20f;

            // Add divider line
            footerTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.TOP_BORDER, FixedHeight = 15f });

            // Contact information
            PdfPCell contactCell = new PdfPCell();
            contactCell.Border = Rectangle.NO_BORDER;
            contactCell.HorizontalAlignment = Element.ALIGN_CENTER;

            Font contactFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            contactCell.AddElement(new Paragraph("For orders or inquiries:", contactFont));
            contactCell.AddElement(new Paragraph(business?.Email ?? "contact@example.com", contactFont));
            contactCell.AddElement(new Paragraph(business?.PhoneNumber ?? "+123 456 7890", contactFont));

            footerTable.AddCell(contactCell);

            // Generation info
            PdfPCell infoCell = new PdfPCell(new Phrase($"Generated on {DateTime.Now.ToString("yyyy-MM-dd")}",
                FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8)));
            infoCell.Border = Rectangle.NO_BORDER;
            infoCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            footerTable.AddCell(infoCell);

            document.Add(footerTable);
        }

        // Enhanced AddTableCell method with alignment option
        private void AddTableCell(PdfPTable table, string text, int alignment = Element.ALIGN_LEFT)
        {
            // Create cell with truncated text if it's a description
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "-"));

            // Styling
            cell.Padding = 5f;
            cell.PaddingTop = 7f;
            cell.PaddingBottom = 7f;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;

            // Alternate row coloring for better readability
            if (table.Rows.Count % 2 == 1)
            {
                cell.BackgroundColor = new BaseColor(245, 245, 245);
            }

            table.AddCell(cell);
        }

        public async Task<bool> LogOrderClick(OrderClickProduct orderClick, string deviceType, string clientIp)
        {
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.Id == orderClick.Id);

            if (product == null)
            {
                throw new EntityNotFoundException($"Product not found");
            }

            var result = _mapper.Map<ProductResponse>(product);

            var business = await _applicationDbContext.Busineses.FirstOrDefaultAsync(p => p.Id == product.BusinessId);

            result.PhoneNumber = business?.PhoneNumber;
            result.BusinessName = business?.Name;
            result.Slug = business?.Slug;

            var geo = await GetCityCountry(clientIp);
            var saleActivity = new SaleActivity
            {
                BusinessId = business.Id,
                IsCatalogCliked = false,
                LinkedView = true,
                DeviceType = deviceType,
                CreatedDate = DateTime.UtcNow,
                Source = "Unknown",
                ProductId = product.Id,
                OrderClicked = true,
                Country = geo.Country,
                City = geo.City,
                IsActive = true
            };

            _applicationDbContext.SaleActivities.Add(saleActivity);

           return await _applicationDbContext.SaveChangesAsync() > 0;
        }

       

    }
}