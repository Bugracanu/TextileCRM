using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InvoicesApiController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public InvoicesApiController(
            IInvoiceService invoiceService,
            IPaymentService paymentService,
            IEmailService emailService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        /// <summary>
        /// Tüm faturaları listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Invoice>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// ID'ye göre fatura getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> GetById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }
            return Ok(invoice);
        }

        /// <summary>
        /// Fatura numarasına göre fatura getirir
        /// </summary>
        [HttpGet("by-number/{invoiceNumber}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> GetByNumber(string invoiceNumber)
        {
            var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }
            return Ok(invoice);
        }

        /// <summary>
        /// Müşteriye ait faturaları listeler
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<Invoice>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetByCustomer(int customerId)
        {
            var invoices = await _invoiceService.GetInvoicesByCustomerIdAsync(customerId);
            return Ok(invoices);
        }

        /// <summary>
        /// Siparişe ait faturaları listeler
        /// </summary>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(IEnumerable<Invoice>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetByOrder(int orderId)
        {
            var invoices = await _invoiceService.GetInvoicesByOrderIdAsync(orderId);
            return Ok(invoices);
        }

        /// <summary>
        /// Duruma göre faturaları filtreler
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Invoice>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetByStatus(InvoiceStatus status)
        {
            var invoices = await _invoiceService.GetInvoicesByStatusAsync(status);
            return Ok(invoices);
        }

        /// <summary>
        /// Yeni fatura oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Invoice>> Create([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
        }

        /// <summary>
        /// Fatura bilgilerini günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingInvoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (existingInvoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }

            await _invoiceService.UpdateInvoiceAsync(invoice);
            return NoContent();
        }

        /// <summary>
        /// Fatura siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }

            await _invoiceService.DeleteInvoiceAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Fatura durumunu günceller
        /// </summary>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> UpdateStatus(int id, [FromBody] InvoiceStatus status)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }

            await _invoiceService.UpdateInvoiceStatusAsync(id, status);
            var updatedInvoice = await _invoiceService.GetInvoiceByIdAsync(id);
            return Ok(updatedInvoice);
        }

        /// <summary>
        /// Faturanın ödeme durumunu getirir
        /// </summary>
        [HttpGet("{id}/payment-status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetPaymentStatus(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }

            var paidAmount = await _invoiceService.GetTotalPaidAmountAsync(id);
            var remaining = await _invoiceService.GetRemainingAmountAsync(id);

            return Ok(new
            {
                invoiceId = id,
                invoiceNumber = invoice.InvoiceNumber,
                totalAmount = invoice.TotalAmount,
                paidAmount,
                remainingAmount = remaining,
                status = invoice.Status,
                isFullyPaid = remaining <= 0
            });
        }

        /// <summary>
        /// Faturayı email olarak gönderir
        /// </summary>
        [HttpPost("{id}/send-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendEmail(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Fatura bulunamadı" });
            }

            await _emailService.SendInvoiceAsync(id);
            return Ok(new { message = "Fatura başarıyla gönderildi" });
        }

        /// <summary>
        /// Yeni fatura numarası üretir
        /// </summary>
        [HttpGet("generate-number")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GenerateNumber()
        {
            var invoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync();
            return Ok(new { invoiceNumber });
        }
    }
}

