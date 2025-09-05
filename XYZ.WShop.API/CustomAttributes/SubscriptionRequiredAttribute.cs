using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace XYZ.WShop.API.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SubscriptionRequiredAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get subscription status from claims or user service
            var subscriptionExpiryClaim = user.Claims.FirstOrDefault(c => c.Type == "subscription_expiry");

            if (subscriptionExpiryClaim == null || !DateTime.TryParse(subscriptionExpiryClaim.Value, out var expiryDate))
            {
                context.Result = new ObjectResult("Subscription required")
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Check if subscription is expired
            if (expiryDate < DateTime.UtcNow)
            {
                context.Result = new ObjectResult("Subscription expired")
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
