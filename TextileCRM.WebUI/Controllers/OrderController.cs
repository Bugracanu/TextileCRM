using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCustomerDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                await _orderService.CreateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            await PopulateCustomerDropdown();
            return View(order);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            await PopulateCustomerDropdown();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            await PopulateCustomerDropdown();
            return View(order);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, TextileCRM.Domain.Entities.OrderStatus status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopulateCustomerDropdown()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "CompanyName");
        }

        public async Task<IActionResult> AddItem(int orderId)
        {
            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
                if (order == null)
                {
                    return NotFound();
                }

                order.OrderItems.Add(orderItem);
                await _orderService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
            }

            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.OrderId = orderItem.OrderId;
            return View(orderItem);
        }
    }
}