using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Orders
{
    public class CreateOrder
    {
        public Guid BusinessId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }

    public class OrderResponse: CreateOrder
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
