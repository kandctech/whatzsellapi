
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Dtos.Dashboard;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos.Reports;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XYZ.WShop.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReportService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DashboardResponse> GetReportDashbordAsync(Guid businessId)
        {
            var currentDate = DateTime.UtcNow;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            var todayUtc = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, DateTimeKind.Utc);
            var tomorrowStartUtc = todayUtc.AddDays(1);

            DashboardResponse dashboardResponse = new DashboardResponse();

            dashboardResponse.TodayTotalSale = _context.Orders
                .Where(t => t.BusinessId == businessId
                    && t.PaymentDate >= todayUtc
                    && t.PaymentDate < tomorrowStartUtc)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            // This week's sales (from Monday to today)
            var startOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = startOfWeek.AddDays(-7);
            }
            startOfWeek = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);

            dashboardResponse.ThisWkTotalSale = _context.Orders
                .Where(t => t.BusinessId == businessId && t.PaymentDate >= startOfWeek && t.PaymentDate <= currentDate)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            // This month's sales
            var firstDayOfMonth = new DateTime(currentYear, currentMonth, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            lastDayOfMonth = DateTime.SpecifyKind(lastDayOfMonth, DateTimeKind.Utc);

            dashboardResponse.ThisMthTotalSale = _context.Orders
                .Where(t => t.BusinessId == businessId && t.PaymentDate >= firstDayOfMonth && t.PaymentDate <= lastDayOfMonth)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            // This year's sales
            var firstDayOfYear = new DateTime(currentYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfYear = new DateTime(currentYear, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            dashboardResponse.ThisYrTotalSale = _context.Orders
                .Where(t => t.BusinessId == businessId && t.PaymentDate >= firstDayOfYear && t.PaymentDate <= lastDayOfYear)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            // Expense queries
            dashboardResponse.TodayTotalExpense = _context.Expenses
                .Where(t => t.BusinessId == businessId && t.ExpenseDate.Date == todayUtc)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            dashboardResponse.ThisWkTotalExpense = _context.Expenses
                .Where(t => t.BusinessId == businessId && t.ExpenseDate >= startOfWeek && t.ExpenseDate <= currentDate)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            dashboardResponse.ThisMthTotalExpense = _context.Expenses
                .Where(t => t.BusinessId == businessId && t.ExpenseDate >= firstDayOfMonth && t.ExpenseDate <= lastDayOfMonth)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            dashboardResponse.ThisYrTotalExpense = _context.Expenses
                .Where(t => t.BusinessId == businessId && t.ExpenseDate >= firstDayOfYear && t.ExpenseDate <= lastDayOfYear)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var recentOrders = await _context.Orders
                .Where(t => t.BusinessId == businessId)
                .OrderByDescending(d => d.CreatedDate)
                .Take(3)
                .ToListAsync();

            if (recentOrders.Any())
            {
                dashboardResponse.RecentOrders = _mapper.Map<List<OrderResponse>>(recentOrders);
            }

            // Best selling products query
            var productSales = await _context.OrderItems
                .Where(oi => oi.Order.BusinessId == businessId &&
                             oi.Order.PaymentDate >= firstDayOfMonth &&
                             oi.Order.PaymentDate <= lastDayOfMonth)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    ProductName = g.First().ProductName,
                    TotalSales = g.Sum(oi => oi.Qty * oi.Amount),
                    TotalQuantity = g.Sum(oi => oi.Qty)
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            // Get total revenue for the month
            var totalMonthlyRevenue = productSales.Sum(x => x.TotalSales);



            // Get best selling product if any exists
            if (productSales.Any())
            {
                var product = await _context.Products.FirstOrDefaultAsync(p=> p.Id == productSales.First().ProductId);
                var bestSeller = productSales.First();
                dashboardResponse.BestSellingProductName = bestSeller.ProductName;
                dashboardResponse.BestSellingProductUrl = product?.ImageUrls?.Split(",")[0];
                dashboardResponse.BestSellerQuantityThisMonth = bestSeller.TotalQuantity;
                dashboardResponse.BestSellerRevenueThisMonth = bestSeller.TotalSales;
                dashboardResponse.BestSellerRevenuePercentage =
                    totalMonthlyRevenue > 0 ? (bestSeller.TotalSales / totalMonthlyRevenue) * 100 : 0;
            }
            else
            {
                dashboardResponse.BestSellingProductName = "No sales this month";
                dashboardResponse.BestSellerRevenuePercentage = 0;
                dashboardResponse.BestSellerQuantityThisMonth = 0;
                dashboardResponse.BestSellerRevenueThisMonth = 0;
            }

            return dashboardResponse;
        }
        public async Task<ReportResponseDto> GetReportDataAsync(Guid businessId, ReportRequestDto request)
        {
            return request.ReportType switch
            {
                "sales" => await GetSalesReportAsync(businessId, request.TimeRange),
                "inventory" => await GetInventoryReportAsync(businessId),
                "customers" => await GetCustomersReportAsync(businessId),
                _ => throw new ArgumentException("Invalid report type")
            };
        }

        private async Task<ReportResponseDto> GetSalesReportAsync(Guid businessId, string timeRange)
        {
            DateTime startDate, endDate = DateTime.UtcNow;

            if (timeRange == "week")
            {
                // Calculate start date for the current week (Monday)
                int daysSinceMonday = (int)endDate.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysSinceMonday < 0) daysSinceMonday += 7; // Adjust for Sunday
                startDate = endDate.AddDays(-daysSinceMonday).Date;

                var dailySales = await _context.Orders
                    .Where(o => o.BusinessId == businessId && o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                    .GroupBy(o => o.CreatedDate.Date)
                    .Select(g => new { Date = g.Key, Total = g.Sum(o => o.Amount) })
                    .ToListAsync();

                var labels = Enumerable.Range(0, 7)
                    .Select(i => startDate.AddDays(i).ToString("ddd"))
                    .ToArray();

                var data = Enumerable.Range(0, 7)
                   .Select(i => dailySales.FirstOrDefault(d =>
                        d.Date.Date == startDate.AddDays(i).Date)?.Total ?? 0)
                   .ToArray();

                // Calculate previous week (Monday to Sunday)
                DateTime previousWeekStart = startDate.AddDays(-7);
                DateTime previousWeekEnd = startDate.AddDays(-1);
                var previousPeriodTotal = await GetPreviousPeriodSalesTotal(businessId, previousWeekStart, previousWeekEnd);
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
                startDate = new DateTime(endDate.Year, endDate.Month, 1).ToUniversalTime();
                var weeklySales = await _context.Orders
                    .Where(o => o.BusinessId == businessId && o.CreatedDate >= startDate && o.CreatedDate <= endDate)
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

                var previousPeriodTotal = await GetPreviousPeriodSalesTotal(businessId, startDate.AddMonths(-1), startDate);
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

        private async Task<ReportResponseDto> GetInventoryReportAsync(Guid businessId)
        {
            var inventoryStatus = await _context.Products.Where(p=> p.BusinessId == businessId)
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

        private async Task<ReportResponseDto> GetCustomersReportAsync(Guid businessId)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var customerStatus = await _context.Customers.Where(c=> c.BusinessId == businessId) 
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

        private async Task<decimal> GetPreviousPeriodSalesTotal(Guid businessId, DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.BusinessId == businessId && o.CreatedDate >= startDate && o.CreatedDate < endDate)
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