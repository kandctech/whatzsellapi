
namespace XYZ.WShop.Application.User
{
    public class CreateAccount
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateRsponse: CreateAccount
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

    }
    public class CreateUserAccount
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid BusinessId { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
