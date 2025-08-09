
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Dtos.Reports;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XYZ.WShop.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportResponseDto> GetReportDataAsync(ReportRequestDto request)
        {
            return request.ReportType switch
            {
                "sales" => await GetSalesReportAsync(request.TimeRange),
                "inventory" => await GetInventoryReportAsync(),
                "customers" => await GetCustomersReportAsync(),
                _ => throw new ArgumentException("Invalid report type")
            };
        }

        private async Task<ReportResponseDto> GetSalesReportAsync(string timeRange)
        {
            DateTime startDate, endDate = DateTime.UtcNow;

            if (timeRange == "week")
            {
                startDate = endDate.AddDays(-7);
                var dailySales = await _context.Orders
                    .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                    .GroupBy(o => o.CreatedDate.Date)
                    .Select(g => new { Date = g.Key, Total = g.Sum(o => o.Amount) })
                    .ToListAsync();

                var labels = Enumerable.Range(0, 7)
                    .Select(i => startDate.AddDays(i).ToString("ddd"))
                    .ToArray();

                var data = Enumerable.Range(0, 7)
                    .Select(i => dailySales.FirstOrDefault(d => d.Date == startDate.AddDays(i))?.Total ?? 0)
                    .ToArray();

                var previousPeriodTotal = await GetPreviousPeriodSalesTotal(startDate.AddDays(-7), startDate);
                var currentTotal = data.Sum();
                var change = CalculatePercentageChange(previousPeriodTotal, currentTotal);

                return new ReportResponseDto
                {
                    Title = "Sales Report",
                    Labels = labels,
                    Data = data,
                    Total = currentTotal.ToString("C0"),
                    Change = change
                };
            }
            else // month
            {
                startDate = new DateTime(endDate.Year, endDate.Month, 1);
                var weeklySales = await _context.Orders
                    .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                    .GroupBy(o => new { Week = (o.CreatedDate.Day - 1) / 7 + 1 })
                    .Select(g => new { Week = g.Key.Week, Total = g.Sum(o => o.Amount) })
                    .ToListAsync();

                var weeksInMonth = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
                var labels = Enumerable.Range(1, weeksInMonth)
                    .Select(i => $"Week {i}")
                    .ToArray();

                var data = Enumerable.Range(1, weeksInMonth)
                    .Select(i => weeklySales.FirstOrDefault(w => w.Week == i)?.Total ?? 0)
                    .ToArray();

                var previousPeriodTotal = await GetPreviousPeriodSalesTotal(startDate.AddMonths(-1), startDate);
                var currentTotal = data.Sum();
                var change = CalculatePercentageChange(previousPeriodTotal, currentTotal);

                return new ReportResponseDto
                {
                    Title = "Sales Report",
                    Labels = labels,
                    Data = data,
                    Total = currentTotal.ToString("C0"),
                    Change = change
                };
            }
        }

        private async Task<ReportResponseDto> GetInventoryReportAsync()
        {
            var inventoryStatus = await _context.Products
                .GroupBy(p => p.QuantityInStock > 10 ? "In Stock" :
                             p.QuantityInStock > 0 ? "Low Stock" : "Out of Stock")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var labels = new[] { "In Stock", "Low Stock", "Out of Stock" };
            var data = labels.Select(label =>
                (decimal?)inventoryStatus.FirstOrDefault(i => i.Status == label)?.Count ?? 0M)
                .ToArray();

            // For demo purposes - in real app you'd compare with previous period
            var change = new Random().Next(0, 2) == 0 ? $"+{new Random().Next(1, 20)}%" : $"-{new Random().Next(1, 10)}%";

            return new ReportResponseDto
            {
                Title = "Inventory Report",
                Labels = labels,
                Data = data,
                Total = $"{inventoryStatus.Sum(i => i.Count)} Products",
                Change = change
            };
        }

        private async Task<ReportResponseDto> GetCustomersReportAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var customerStatus = await _context.Customers
                .GroupBy(c => c.LastOrderDate == null ? "Inactive" :
                             c.LastOrderDate >= thirtyDaysAgo ? "Returning" : "New")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var labels = new[] { "New", "Returning", "Inactive" };
            var data = labels.Select(label =>
                (decimal?)customerStatus.FirstOrDefault(c => c.Status == label)?.Count ?? 0M)
                .ToArray();

            // For demo purposes - in real app you'd compare with previous period
            var change = $"+{new Random().Next(5, 25)}%";

            return new ReportResponseDto
            {
                Title = "Customers Report",
                Labels = labels,
                Data = data,
                Total = $"{customerStatus.Sum(c => c.Count)} Customers",
                Change = change
            };
        }

        private async Task<decimal> GetPreviousPeriodSalesTotal(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .SumAsync(o => o.Amount);
        }

        private string CalculatePercentageChange(decimal previousTotal, decimal currentTotal)
        {
            if (previousTotal == 0) return currentTotal == 0 ? "0%" : "+100%";

            var change = ((currentTotal - previousTotal) / previousTotal) * 100;
            return $"{(change >= 0 ? "+" : "")}{change:0}%";
        }
    }
}