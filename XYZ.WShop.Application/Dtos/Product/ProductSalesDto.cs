using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Product
{
    public class ProductSalesDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantitySold { get; set; }
        public int RemainingStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RemainingSkuAmount { get; set; }
        public decimal TotalSales { get; set; }
    }
}
