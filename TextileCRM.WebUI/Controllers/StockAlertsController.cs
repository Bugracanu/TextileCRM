using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TextileCRM.WebUI.Controllers
{
    [Authorize]
    public class StockAlertsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

