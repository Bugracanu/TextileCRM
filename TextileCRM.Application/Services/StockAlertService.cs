using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly IRepository<StockAlert> _alertRepository;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public StockAlertService(
            IRepository<StockAlert> alertRepository,
            IProductService productService,
            INotificationService notificationService,
            IEmailService emailService)
        {
            _alertRepository = alertRepository;
            _productService = productService;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public async Task<IEnumerable<StockAlert>> GetAllStockAlertsAsync()
        {
            return await _alertRepository.GetAllAsync();
        }

        public async Task<StockAlert?> GetStockAlertByIdAsync(int id)
        {
            return await _alertRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<StockAlert>> GetActiveAlertsAsync()
        {
            var alerts = await _alertRepository.GetAllAsync();
            return alerts.Where(a => a.Status == StockAlertStatus.Active).OrderByDescending(a => a.CreatedDate);
        }

        public async Task<IEnumerable<StockAlert>> GetAlertsByProductIdAsync(int productId)
        {
            var alerts = await _alertRepository.GetAllAsync();
            return alerts.Where(a => a.ProductId == productId);
        }

        public async Task<IEnumerable<StockAlert>> GetAlertsByTypeAsync(StockAlertType type)
        {
            var alerts = await _alertRepository.GetAllAsync();
            return alerts.Where(a => a.AlertType == type);
        }

        public async Task<StockAlert> CreateStockAlertAsync(StockAlert alert)
        {
            alert.CreatedDate = DateTime.Now;
            alert.Status = StockAlertStatus.Active;
            await _alertRepository.AddAsync(alert);
            return alert;
        }

        public async Task UpdateStockAlertAsync(StockAlert alert)
        {
            await _alertRepository.UpdateAsync(alert);
        }

        public async Task DeleteStockAlertAsync(int id)
        {
            await _alertRepository.DeleteAsync(id);
        }

        public async Task ResolveAlertAsync(int id, int resolvedBy, string? notes = null)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert != null && alert.Status == StockAlertStatus.Active)
            {
                alert.Status = StockAlertStatus.Resolved;
                alert.ResolvedDate = DateTime.Now;
                alert.ResolvedBy = resolvedBy;
                alert.Notes = notes;
                await _alertRepository.UpdateAsync(alert);
            }
        }

        public async Task CheckAndCreateAlertsAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return;

            // Check if there's already an active alert for this product
            var existingAlerts = await GetAlertsByProductIdAsync(productId);
            var hasActiveAlert = existingAlerts.Any(a => a.Status == StockAlertStatus.Active);

            StockAlertType? alertType = null;
            int threshold = 10; // Default threshold

            if (product.StockQuantity == 0)
            {
                alertType = StockAlertType.OutOfStock;
                threshold = 0;
            }
            else if (product.StockQuantity <= 5)
            {
                alertType = StockAlertType.LowStock;
                threshold = 5;
            }
            else if (product.StockQuantity <= 10)
            {
                alertType = StockAlertType.ReorderPoint;
                threshold = 10;
            }

            if (alertType.HasValue && !hasActiveAlert)
            {
                var alert = new StockAlert
                {
                    ProductId = productId,
                    CurrentQuantity = product.StockQuantity,
                    ThresholdQuantity = threshold,
                    AlertType = alertType.Value,
                    Status = StockAlertStatus.Active,
                    CreatedDate = DateTime.Now
                };

                await _alertRepository.AddAsync(alert);

                // Send notifications
                await SendAlertNotifications(product, alertType.Value);
            }
        }

        public async Task CheckAllProductsAsync()
        {
            var products = await _productService.GetAllProductsAsync();
            foreach (var product in products)
            {
                await CheckAndCreateAlertsAsync(product.Id);
            }
        }

        private async Task SendAlertNotifications(Product product, StockAlertType alertType)
        {
            var message = alertType switch
            {
                StockAlertType.OutOfStock => $"{product.Name} ürünü stokta kalmamıştır!",
                StockAlertType.LowStock => $"{product.Name} ürününün stok seviyesi düşük: {product.StockQuantity} adet",
                StockAlertType.ReorderPoint => $"{product.Name} ürünü için yeniden sipariş noktasına ulaşıldı: {product.StockQuantity} adet",
                _ => $"{product.Name} ürünü için stok uyarısı"
            };

            var priority = alertType == StockAlertType.OutOfStock 
                ? NotificationPriority.Urgent 
                : NotificationPriority.High;

            // Create notification (assuming userId 1 is admin/warehouse manager)
            // In production, send to all relevant users
            try
            {
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    UserId = 1,
                    Title = "Stok Uyarısı",
                    Message = message,
                    Type = NotificationType.Stock,
                    Priority = priority,
                    EntityType = "Product",
                    EntityId = product.Id
                });

                // Send email alert
                await _emailService.SendLowStockAlertAsync(product.Id);
            }
            catch (Exception)
            {
                // Log error but don't fail the stock check
            }
        }
    }
}

