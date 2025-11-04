using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);
        Task<IEnumerable<Invoice>> GetInvoicesByOrderIdAsync(int orderId);
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(int id);
        Task<string> GenerateInvoiceNumberAsync();
        Task UpdateInvoiceStatusAsync(int id, InvoiceStatus status);
        Task<decimal> GetTotalPaidAmountAsync(int invoiceId);
        Task<decimal> GetRemainingAmountAsync(int invoiceId);
    }
}

