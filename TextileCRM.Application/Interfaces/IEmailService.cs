using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextileCRM.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
        Task SendOrderConfirmationAsync(int orderId);
        Task SendInvoiceAsync(int invoiceId);
        Task SendPaymentConfirmationAsync(int paymentId);
        Task SendLowStockAlertAsync(int productId);
        Task SendWelcomeEmailAsync(string email, string name);
    }
}

