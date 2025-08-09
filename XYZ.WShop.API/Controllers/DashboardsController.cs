using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace XYZ.WShop.API.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/dashboards")]
    public class DashboardsController : BaseController
    {
    }
}
