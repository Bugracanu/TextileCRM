using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TextileCRM.WebUI.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        public IActionResult Sales()
        {
            return View();
        }
        
        public IActionResult Financial()
        {
            return View();
        }
        
        public IActionResult Customers()
        {
            return View();
        }
    }
}

