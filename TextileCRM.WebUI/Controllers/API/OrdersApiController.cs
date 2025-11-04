using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)] // Geçici olarak Swagger'dan gizle
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public OrdersApiController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }

        /// <summary>
        /// Tüm siparişleri listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// ID'ye göre sipariş getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı" });
            }
            return Ok(order);
        }

        /// <summary>
        /// Yeni sipariş oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Müşteri kontrolü
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Geçersiz müşteri ID" });
            }

            order.OrderDate = DateTime.Now;
            order.CreatedDate = DateTime.Now;
            order.Status = OrderStatus.New;
            
            await _orderService.CreateOrderAsync(order);
            
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        /// <summary>
        /// Sipariş bilgilerini günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Order order)
        {
            if (id != order.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = await _orderService.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı" });
            }

            await _orderService.UpdateOrderAsync(order);
            return NoContent();
        }

        /// <summary>
        /// Sipariş siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı" });
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Sipariş durumunu günceller
        /// </summary>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> UpdateStatus(int id, [FromBody] OrderStatus status)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı" });
            }

            await _orderService.UpdateOrderStatusAsync(id, status);
            
            var updatedOrder = await _orderService.GetOrderByIdAsync(id);
            return Ok(updatedOrder);
        }

        /// <summary>
        /// Müşteriye ait siparişleri listeler
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetByCustomer(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound(new { message = "Müşteri bulunamadı" });
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// Duruma göre siparişleri filtreler
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetByStatus(OrderStatus status)
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            var filteredOrders = allOrders.Where(o => o.Status == status);
            
            return Ok(filteredOrders);
        }

        /// <summary>
        /// Tarih aralığına göre siparişleri filtreler
        /// </summary>
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            var filteredOrders = allOrders.Where(o => 
                o.OrderDate >= startDate && o.OrderDate <= endDate);
            
            return Ok(filteredOrders);
        }
    }
}

