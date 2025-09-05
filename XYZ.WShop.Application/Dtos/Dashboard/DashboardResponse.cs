using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos.Product;

namespace XYZ.WShop.Application.Dtos.Dashboard
{
    public class DashboardResponse
    {
        public decimal TodayTotalSale { get; set; }
        public decimal ThisWkTotalSale { get; set; }
        public decimal ThisMthTotalSale { get; set; }
        public decimal ThisYrTotalSale { get; set; }

        public decimal TodayTotalExpense { get; set; }
        public decimal ThisWkTotalExpense { get; set; }
        public decimal ThisMthTotalExpense { get; set; }
        public decimal ThisYrTotalExpense { get; set; }

        public List<OrderResponse>? RecentOrders { get; set; }

        public string? BestSellingProductName { get; set; }
        public int BestSellerQuantityThisMonth { get; set; }
        public decimal BestSellerRevenueThisMonth { get; set; }
        public decimal BestSellerRevenuePercentage { get; set; }
        
        public int? BestSelleingPrPercentageRevenueContribtn { get; set; }
        public string? BestSellingProductUrl { get; set; }
    }
}
