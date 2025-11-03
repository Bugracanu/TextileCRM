using System;
using System.Diagnostics;
using System.Linq;
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

    [HttpGet]
    public async Task<IActionResult> GlobalSearch(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
        {
            return Json(new
            {
                customers = new object[] { },
                orders = new object[] { },
                products = new object[] { }
            });
        }

        // Müşterilerde ara
        var customers = await _customerService.GetAllCustomersAsync();
        var customerResults = customers
            .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.ContactName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .Select(c => new
            {
                id = c.Id,
                name = c.Name,
                email = c.Email
            })
            .ToList();

        // Siparişlerde ara
        var orders = await _orderService.GetAllOrdersAsync();
        var orderResults = orders
            .Where(o => o.Id.ToString().Contains(searchTerm) ||
                       (o.Customer != null && o.Customer.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                       o.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .Select(o => new
            {
                id = o.Id,
                customerName = o.Customer != null ? o.Customer.Name : $"Müşteri #{o.CustomerId}",
                status = o.Status.ToString(),
                totalAmount = o.TotalAmount,
                orderDate = o.OrderDate.ToShortDateString()
            })
            .ToList();

        // Ürünlerde ara
        var products = await _productService.GetAllProductsAsync();
        var productResults = products
            .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                code = p.Code,
                unitPrice = p.UnitPrice,
                stockQuantity = p.StockQuantity
            })
            .ToList();

        return Json(new
        {
            customers = customerResults,
            orders = orderResults,
            products = productResults
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
