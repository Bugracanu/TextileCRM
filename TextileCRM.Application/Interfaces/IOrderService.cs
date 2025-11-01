using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task UpdateOrderStatusAsync(int id, OrderStatus status);
    }
}