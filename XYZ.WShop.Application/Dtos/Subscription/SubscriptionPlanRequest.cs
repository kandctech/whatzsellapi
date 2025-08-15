

namespace XYZ.WShop.Application.Dtos.Subscription
{
    public class SubscriptionPlanRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class SubscriptionPlanUpdateRequest: SubscriptionPlanRequest
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class SubscriptionPlanResponse : SubscriptionPlanUpdateRequest
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
