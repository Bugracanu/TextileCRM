using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;

namespace TextileCRM.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;

        public EmailService(
            IOrderService orderService,
            ICustomerService customerService,
            IInvoiceService invoiceService,
            IPaymentService paymentService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _productService = productService;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            // Mock implementation - In production, integrate with SendGrid, MailGun, or SMTP
            await Task.Run(() =>
            {
                Console.WriteLine($"[EMAIL] To: {to}");
                Console.WriteLine($"[EMAIL] Subject: {subject}");
                Console.WriteLine($"[EMAIL] Body: {body}");
                Console.WriteLine($"[EMAIL] IsHtml: {isHtml}");
                Console.WriteLine("-------------------------------------");
            });
        }

        public async Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true)
        {
            foreach (var email in to)
            {
                await SendEmailAsync(email, subject, body, isHtml);
            }
        }

        public async Task SendOrderConfirmationAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null) return;

            var subject = $"Sipariş Onayı - #{orderId}";
            var body = $@"
                <h2>Sayın {customer.Name},</h2>
                <p>Siparişiniz başarıyla alınmıştır.</p>
                <p><strong>Sipariş No:</strong> {orderId}</p>
                <p><strong>Sipariş Tarihi:</strong> {order.OrderDate:dd/MM/yyyy}</p>
                <p><strong>Toplam Tutar:</strong> {order.TotalAmount:C}</p>
                <p><strong>Durum:</strong> {order.Status}</p>
                <br/>
                <p>Teşekkür ederiz.</p>
            ";

            await SendEmailAsync(customer.Email, subject, body);
        }

        public async Task SendInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return;

            var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId);
            if (customer == null) return;

            var subject = $"Fatura - {invoice.InvoiceNumber}";
            var body = $@"
                <h2>Sayın {customer.Name},</h2>
                <p>Faturanız hazırlanmıştır.</p>
                <p><strong>Fatura No:</strong> {invoice.InvoiceNumber}</p>
                <p><strong>Fatura Tarihi:</strong> {invoice.InvoiceDate:dd/MM/yyyy}</p>
                <p><strong>Vade Tarihi:</strong> {invoice.DueDate:dd/MM/yyyy}</p>
                <p><strong>Toplam Tutar:</strong> {invoice.TotalAmount:C}</p>
                <br/>
                <p>Ödemenizi zamanında yapmanızı rica ederiz.</p>
            ";

            await SendEmailAsync(customer.Email, subject, body);
        }

        public async Task SendPaymentConfirmationAsync(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null) return;

            var invoice = await _invoiceService.GetInvoiceByIdAsync(payment.InvoiceId);
            if (invoice == null) return;

            var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId);
            if (customer == null) return;

            var subject = $"Ödeme Onayı - {payment.PaymentReference}";
            var body = $@"
                <h2>Sayın {customer.Name},</h2>
                <p>Ödemeniz başarıyla alınmıştır.</p>
                <p><strong>Ödeme Referans No:</strong> {payment.PaymentReference}</p>
                <p><strong>Fatura No:</strong> {invoice.InvoiceNumber}</p>
                <p><strong>Ödeme Tarihi:</strong> {payment.PaymentDate:dd/MM/yyyy}</p>
                <p><strong>Tutar:</strong> {payment.Amount:C}</p>
                <p><strong>Ödeme Yöntemi:</strong> {payment.PaymentMethod}</p>
                <br/>
                <p>Teşekkür ederiz.</p>
            ";

            await SendEmailAsync(customer.Email, subject, body);
        }

        public async Task SendLowStockAlertAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return;

            var subject = $"Düşük Stok Uyarısı - {product.Name}";
            var body = $@"
                <h2>Düşük Stok Uyarısı</h2>
                <p>Aşağıdaki ürün için stok seviyesi düşük:</p>
                <p><strong>Ürün:</strong> {product.Name}</p>
                <p><strong>Ürün Kodu:</strong> {product.Code}</p>
                <p><strong>Mevcut Stok:</strong> {product.StockQuantity}</p>
                <br/>
                <p>Lütfen yeni sipariş verilmesini sağlayın.</p>
            ";

            // In production, send to warehouse/purchasing team
            await SendEmailAsync("warehouse@textilecrm.com", subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            var subject = "Textile CRM'e Hoş Geldiniz";
            var body = $@"
                <h2>Sayın {name},</h2>
                <p>Textile CRM sistemine hoş geldiniz!</p>
                <p>Hesabınız başarıyla oluşturulmuştur.</p>
                <p>Sisteme giriş yapmak için aşağıdaki bilgileri kullanabilirsiniz:</p>
                <p><strong>Email:</strong> {email}</p>
                <br/>
                <p>İyi çalışmalar dileriz.</p>
            ";

            await SendEmailAsync(email, subject, body);
        }
    }
}

