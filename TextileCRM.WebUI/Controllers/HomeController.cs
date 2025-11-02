using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.WebUI.Models;
using TextileCRM.Application.Interfaces;
using System.Threading.Tasks;

namespace TextileCRM.WebUI.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IEmployeeService _employeeService;
    private readonly IWorkLogService _workLogService;

    public HomeController(
        ILogger<HomeController> logger,
        ICustomerService customerService,
        IOrderService orderService,
        IProductService productService,
        IEmployeeService employeeService,
        IWorkLogService workLogService)
    {
        _logger = logger;
        _customerService = customerService;
        _orderService = orderService;
        _productService = productService;
        _employeeService = employeeService;
        _workLogService = workLogService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.CustomerCount = (await _customerService.GetAllCustomersAsync()).Count();
        ViewBag.OrderCount = (await _orderService.GetAllOrdersAsync()).Count();
        ViewBag.ProductCount = (await _productService.GetAllProductsAsync()).Count();
        ViewBag.EmployeeCount = (await _employeeService.GetAllEmployeesAsync()).Count();
        ViewBag.WorkLogCount = (await _workLogService.GetAllWorkLogsAsync()).Count();
        
        ViewBag.RecentOrders = (await _orderService.GetAllOrdersAsync()).OrderBy(o => o.Id).Take(5);
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
