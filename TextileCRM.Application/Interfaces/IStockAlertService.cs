using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IStockAlertService
    {
        Task<IEnumerable<StockAlert>> GetAllStockAlertsAsync();
        Task<StockAlert?> GetStockAlertByIdAsync(int id);
        Task<IEnumerable<StockAlert>> GetActiveAlertsAsync();
        Task<IEnumerable<StockAlert>> GetAlertsByProductIdAsync(int productId);
        Task<IEnumerable<StockAlert>> GetAlertsByTypeAsync(StockAlertType type);
        Task<StockAlert> CreateStockAlertAsync(StockAlert alert);
        Task UpdateStockAlertAsync(StockAlert alert);
        Task DeleteStockAlertAsync(int id);
        Task ResolveAlertAsync(int id, int resolvedBy, string? notes = null);
        Task CheckAndCreateAlertsAsync(int productId);
        Task CheckAllProductsAsync();
    }
}

