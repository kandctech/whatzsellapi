using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Domain;

namespace XYZ.WShop.API.Controllers
{
    /// <summary>
    /// Represents a base controller class.
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase
    {
        // Returns the current authenticated account (null if not logged in)
        public ApplicationUser ApplicationUser => (ApplicationUser)HttpContext.Items["User"];
    }
}
