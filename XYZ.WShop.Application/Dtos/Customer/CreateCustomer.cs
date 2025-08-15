

namespace XYZ.WShop.Application.Dtos.Customer
{
    public class CreateCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public Guid BusinessId { get; set; }
        public string[]? Tags { get; set; }
    }

    public class UpdateCustomer : CreateCustomer
    {
        public Guid Id { get; set; }
    }

        public class CustomerResponse: CreateCustomer
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
