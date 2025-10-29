using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _orderRepository.FindAsync(o => o.CustomerId == customerId);
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.CreatedDate = DateTime.Now;
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();
            }
        }
    }
}