using XYZ.WShop.Application.Dtos.Product;

namespace XYZ.WShop.Application.Dtos.Product
{
    public class AddProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string[] ImageUrls { get; set; }
        public Guid BusinessId { get; set; }
    }

    public class OrderClickProduct
    {
        public Guid Id { get; set; }
    }

    public class UpdateProductRequest : AddProduct
    {
        public Guid Id { get; set; }
    }

    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string[] ImageUrls { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? BusinessName { get; set; }
        public string Slug { get; set; }
    }

    public class BusinessResponse
    {
        public List<ProductResponse> Products { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PhoneNumber { get; set; }
        public Guid BusinessId { get; set; }
        public string Slug { get; set; }
    }
}

