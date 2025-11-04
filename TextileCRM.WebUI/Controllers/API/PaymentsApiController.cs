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
    public class PaymentsApiController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public PaymentsApiController(IPaymentService paymentService, IEmailService emailService)
        {
            _paymentService = paymentService;
            _emailService = emailService;
        }

        /// <summary>
        /// Tüm ödemeleri listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        /// <summary>
        /// ID'ye göre ödeme getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Payment>> GetById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Ödeme bulunamadı" });
            }
            return Ok(payment);
        }

        /// <summary>
        /// Faturaya ait ödemeleri listeler
        /// </summary>
        [HttpGet("invoice/{invoiceId}")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByInvoice(int invoiceId)
        {
            var payments = await _paymentService.GetPaymentsByInvoiceIdAsync(invoiceId);
            return Ok(payments);
        }

        /// <summary>
        /// Ödeme yöntemine göre ödemeleri filtreler
        /// </summary>
        [HttpGet("method/{method}")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByMethod(PaymentMethod method)
        {
            var payments = await _paymentService.GetPaymentsByMethodAsync(method);
            return Ok(payments);
        }

        /// <summary>
        /// Duruma göre ödemeleri filtreler
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByStatus(PaymentStatus status)
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(payments);
        }

        /// <summary>
        /// Yeni ödeme kaydı oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Payment>> Create([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
        }

        /// <summary>
        /// Ödeme bilgilerini günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPayment = await _paymentService.GetPaymentByIdAsync(id);
            if (existingPayment == null)
            {
                return NotFound(new { message = "Ödeme bulunamadı" });
            }

            await _paymentService.UpdatePaymentAsync(payment);
            return NoContent();
        }

        /// <summary>
        /// Ödeme kaydını siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Ödeme bulunamadı" });
            }

            await _paymentService.DeletePaymentAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Ödemeyi işleme alır
        /// </summary>
        [HttpPost("{id}/process")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Payment>> ProcessPayment(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Ödeme bulunamadı" });
            }

            await _paymentService.ProcessPaymentAsync(id);
            var processedPayment = await _paymentService.GetPaymentByIdAsync(id);
            
            // Send confirmation email
            try
            {
                await _emailService.SendPaymentConfirmationAsync(id);
            }
            catch
            {
                // Log error but continue
            }

            return Ok(processedPayment);
        }

        /// <summary>
        /// Yeni ödeme referans numarası üretir
        /// </summary>
        [HttpGet("generate-reference")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GenerateReference()
        {
            var reference = await _paymentService.GeneratePaymentReferenceAsync();
            return Ok(new { paymentReference = reference });
        }
    }
}

