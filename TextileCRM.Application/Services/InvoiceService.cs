using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Payment> _paymentRepository;

        public InvoiceService(IRepository<Invoice> invoiceRepository, IRepository<Payment> paymentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }

        public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.Where(i => i.CustomerId == customerId);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByOrderIdAsync(int orderId)
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.Where(i => i.OrderId == orderId);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.Where(i => i.Status == status);
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            invoice.CreatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
            }
            await _invoiceRepository.AddAsync(invoice);
            return invoice;
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task DeleteInvoiceAsync(int id)
        {
            await _invoiceRepository.DeleteAsync(id);
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var prefix = $"INV-{year}{month:D2}-";
            
            var lastInvoice = invoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefault();

            if (lastInvoice == null)
            {
                return $"{prefix}0001";
            }

            var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D4}";
        }

        public async Task UpdateInvoiceStatusAsync(int id, InvoiceStatus status)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice != null)
            {
                invoice.Status = status;
                if (status == InvoiceStatus.Paid)
                {
                    invoice.PaidDate = DateTime.Now;
                }
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int invoiceId)
        {
            var payments = await _paymentRepository.GetAllAsync();
            var total = payments
                .Where(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);
            return total;
        }

        public async Task<decimal> GetRemainingAmountAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null) return 0;

            var paidAmount = await GetTotalPaidAmountAsync(invoiceId);
            return invoice.TotalAmount - paidAmount;
        }
    }
}

