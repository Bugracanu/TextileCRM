using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<Invoice> _invoiceRepository;

        public PaymentService(IRepository<Payment> paymentRepository, IRepository<Invoice> invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Where(p => p.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByMethodAsync(PaymentMethod method)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Where(p => p.PaymentMethod == method);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Where(p => p.Status == status);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            payment.CreatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(payment.PaymentReference))
            {
                payment.PaymentReference = await GeneratePaymentReferenceAsync();
            }
            await _paymentRepository.AddAsync(payment);
            
            // Update invoice status if fully paid
            await UpdateInvoiceStatusAfterPaymentAsync(payment.InvoiceId);
            
            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _paymentRepository.UpdateAsync(payment);
            await UpdateInvoiceStatusAfterPaymentAsync(payment.InvoiceId);
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment != null)
            {
                var invoiceId = payment.InvoiceId;
                await _paymentRepository.DeleteAsync(id);
                await UpdateInvoiceStatusAfterPaymentAsync(invoiceId);
            }
        }

        public async Task<string> GeneratePaymentReferenceAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var year = DateTime.Now.Year;
            var prefix = $"PAY-{year}-";
            
            var lastPayment = payments
                .Where(p => p.PaymentReference.StartsWith(prefix))
                .OrderByDescending(p => p.PaymentReference)
                .FirstOrDefault();

            if (lastPayment == null)
            {
                return $"{prefix}00001";
            }

            var lastNumber = int.Parse(lastPayment.PaymentReference.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D5}";
        }

        public async Task ProcessPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment != null && payment.Status == PaymentStatus.Pending)
            {
                payment.Status = PaymentStatus.Processing;
                await _paymentRepository.UpdateAsync(payment);
                
                // Simulate payment processing
                // In real scenario, this would integrate with payment gateway
                await Task.Delay(100);
                
                payment.Status = PaymentStatus.Completed;
                payment.TransactionId = Guid.NewGuid().ToString().Substring(0, 16).ToUpper();
                await _paymentRepository.UpdateAsync(payment);
                
                await UpdateInvoiceStatusAfterPaymentAsync(payment.InvoiceId);
            }
        }

        private async Task UpdateInvoiceStatusAfterPaymentAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null) return;

            var payments = await _paymentRepository.GetAllAsync();
            var totalPaid = payments
                .Where(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);
            
            var remaining = invoice.TotalAmount - totalPaid;

            if (remaining <= 0)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidDate = DateTime.Now;
                await _invoiceRepository.UpdateAsync(invoice);
            }
            else if (totalPaid > 0)
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }
    }
}

