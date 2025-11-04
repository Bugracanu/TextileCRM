using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;
using System.Security.Claims;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StockAlertsApiController : ControllerBase
    {
        private readonly IStockAlertService _stockAlertService;

        public StockAlertsApiController(IStockAlertService stockAlertService)
        {
            _stockAlertService = stockAlertService;
        }

        /// <summary>
        /// Tüm stok uyarılarını listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StockAlert>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockAlert>>> GetAll()
        {
            var alerts = await _stockAlertService.GetAllStockAlertsAsync();
            return Ok(alerts);
        }

        /// <summary>
        /// Aktif stok uyarılarını listeler
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<StockAlert>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockAlert>>> GetActive()
        {
            var alerts = await _stockAlertService.GetActiveAlertsAsync();
            return Ok(alerts);
        }

        /// <summary>
        /// ID'ye göre stok uyarısı getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StockAlert), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockAlert>> GetById(int id)
        {
            var alert = await _stockAlertService.GetStockAlertByIdAsync(id);
            if (alert == null)
            {
                return NotFound(new { message = "Stok uyarısı bulunamadı" });
            }
            return Ok(alert);
        }

        /// <summary>
        /// Ürüne ait stok uyarılarını listeler
        /// </summary>
        [HttpGet("product/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<StockAlert>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockAlert>>> GetByProduct(int productId)
        {
            var alerts = await _stockAlertService.GetAlertsByProductIdAsync(productId);
            return Ok(alerts);
        }

        /// <summary>
        /// Uyarı tipine göre filtreler
        /// </summary>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<StockAlert>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockAlert>>> GetByType(StockAlertType type)
        {
            var alerts = await _stockAlertService.GetAlertsByTypeAsync(type);
            return Ok(alerts);
        }

        /// <summary>
        /// Yeni stok uyarısı oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StockAlert), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StockAlert>> Create([FromBody] StockAlert alert)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAlert = await _stockAlertService.CreateStockAlertAsync(alert);
            return CreatedAtAction(nameof(GetById), new { id = createdAlert.Id }, createdAlert);
        }

        /// <summary>
        /// Stok uyarısını günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] StockAlert alert)
        {
            if (id != alert.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAlert = await _stockAlertService.GetStockAlertByIdAsync(id);
            if (existingAlert == null)
            {
                return NotFound(new { message = "Stok uyarısı bulunamadı" });
            }

            await _stockAlertService.UpdateStockAlertAsync(alert);
            return NoContent();
        }

        /// <summary>
        /// Stok uyarısını çözer
        /// </summary>
        [HttpPost("{id}/resolve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveAlertRequest? request = null)
        {
            var alert = await _stockAlertService.GetStockAlertByIdAsync(id);
            if (alert == null)
            {
                return NotFound(new { message = "Stok uyarısı bulunamadı" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int resolvedBy = int.TryParse(userIdClaim, out int userId) ? userId : 1;

            await _stockAlertService.ResolveAlertAsync(id, resolvedBy, request?.Notes);
            return Ok(new { message = "Stok uyarısı çözüldü" });
        }

        /// <summary>
        /// Stok uyarısını siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var alert = await _stockAlertService.GetStockAlertByIdAsync(id);
            if (alert == null)
            {
                return NotFound(new { message = "Stok uyarısı bulunamadı" });
            }

            await _stockAlertService.DeleteStockAlertAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Bir ürün için stok kontrolü yapar ve gerekirse uyarı oluşturur
        /// </summary>
        [HttpPost("check-product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckProduct(int productId)
        {
            await _stockAlertService.CheckAndCreateAlertsAsync(productId);
            return Ok(new { message = "Stok kontrolü tamamlandı" });
        }

        /// <summary>
        /// Tüm ürünler için stok kontrolü yapar
        /// </summary>
        [HttpPost("check-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAllProducts()
        {
            await _stockAlertService.CheckAllProductsAsync();
            return Ok(new { message = "Tüm ürünler için stok kontrolü tamamlandı" });
        }
    }

    public class ResolveAlertRequest
    {
        public string? Notes { get; set; }
    }
}

