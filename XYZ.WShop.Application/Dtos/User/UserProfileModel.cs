
namespace XYZ.WShop.Application.Dtos.User
{
    public class UserProfileModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string? BusinessDescription { get; set; }
        public string? BusinessCategory { get; set; }
        public string BusinessAddress { get; set; }
        public string? Email { get; set; }
        public Guid BusinessId { get; set; }
        public string UserId { get; set; }
        public string? Token { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? Slug { get; set; }
    }
}
