
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
        public DateTime LastPaymentDate { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public string? Role { get; set; }
        public string? Slug { get; set; }
        private string? _currency;
        private decimal _subscriptionAmountPerDay;

        public string? Currency
        {
            get => _currency;
            set
            {
                _currency = value;
                UpdateSubscriptionAmount();
            }
        }

        public decimal SubScriptionAmountPerDay
        {
            get => _subscriptionAmountPerDay;
            set => _subscriptionAmountPerDay = value;
        }

        private void UpdateSubscriptionAmount()
        {
            _subscriptionAmountPerDay = _currency?.ToUpper() switch
            {
                "₦" => 100m,
                "R" => 5m,
                "₵" => 2m,
                "KSh" => 30m,
                "$" => 0.20m,
                "£" => 0.15m,
                _ => _subscriptionAmountPerDay // Keep current value if currency not recognized
            };
        }

    }
}
