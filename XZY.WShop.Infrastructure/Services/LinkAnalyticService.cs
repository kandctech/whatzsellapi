using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Dtos.Marketing;
using XYZ.WShop.Application.Interfaces.Services;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class LinkAnalyticService : ILinkAnalyticService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LinkAnalyticService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ResponseModel<LinkAnalyticsResponse> GetLinkPerformance(Guid businessId, DateTime startDate, DateTime endDate)
        {
            var activities = _context.SaleActivities
                .Where(a => a.BusinessId == businessId &&
                           a.CreatedDate >= startDate.ToUniversalTime() &&
                           a.CreatedDate <= endDate.AddDays(1).AddTicks(-1).ToUniversalTime())
                .ToList();

            // Calculate summary metrics
            var summary = new SummaryMetrics
            {
                TotalViews = activities.Count(t=> !t.IsCatalogCliked),
                TotalCatalogViews = activities.Count(t => t.IsCatalogCliked),
                WhatsAppClicks = activities.Count(t=> t.OrderClicked),
                CTR = CalculateCTR(activities.Count, activities.Count(t => t.OrderClicked))
            };

            // Get all products for this business
            var products = _context.Products
                .Where(p => p.BusinessId == businessId)
                .ToList(); // Execute the query first

            // Now calculate product metrics in memory
            var productMetrics = products
                .Select(p => new ProductMetrics
                {
                    Name = p.Name,
                    Views = activities.Count(a => a.ProductId == p.Id && !a.IsCatalogCliked),
                    WhatsAppClicks = activities.Count(a => a.ProductId == p.Id && a.OrderClicked),
                    CTR = CalculateCTR(
                        activities.Count(a => a.ProductId == p.Id && !a.IsCatalogCliked),
                        activities.Count(a => a.ProductId == p.Id && a.OrderClicked))
                })
                .Where(p => p.Views > 0)
                .OrderByDescending(p => p.Views)
                .Take(10)
                .ToList();

            // Get country metrics
            var countryMetrics = activities
                .Where(a => !string.IsNullOrEmpty(a.Country))
                .GroupBy(a => a.Country)
                .Select(g => new CountryMetrics
                {
                    Name = g.Key,
                    Views = g.Count(),
                    Clicks = g.Where(t=> t.OrderClicked).Count()
                })
                .OrderByDescending(c => c.Views)
                .Take(10)
                .ToList();

            var result = new LinkAnalyticsResponse
            {
                Summary = summary,
                Products = productMetrics,
                Countries = countryMetrics,
                DateRange = $"{startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}"
            };

            return ResponseModel<LinkAnalyticsResponse>.CreateResponse(
              result,
              "Retrived successfullY",
              true);
        }

        private string CalculateCTR(int views, int clicks)
        {
            if (views == 0) return "0%";
            var ctr = (clicks * 100.0 / views);
            return $"{ctr:0.0}%";
        }
    }
}
