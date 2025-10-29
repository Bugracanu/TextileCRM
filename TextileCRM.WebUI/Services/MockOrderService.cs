using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockOrderService : IOrderService
    {
        private readonly List<Order> _orders;

        public MockOrderService()
        {
            _orders = new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-10), DeliveryDate = DateTime.Now.AddDays(5), Status = OrderStatus.InProduction, TotalAmount = 1500.00m },
                new Order { Id = 2, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-5), DeliveryDate = DateTime.Now.AddDays(10), Status = OrderStatus.Pending, TotalAmount = 2200.00m },
                new Order { Id = 3, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-2), DeliveryDate = DateTime.Now.AddDays(15), Status = OrderStatus.Pending, TotalAmount = 800.00m }
            };
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_orders);
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await Task.FromResult(_orders.Where(o => o.CustomerId == customerId).ToList());
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.Id = _orders.Max(o => o.Id) + 1;
            _orders.Add(order);
            await Task.CompletedTask;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder != null)
            {
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.DeliveryDate = order.DeliveryDate;
                existingOrder.Status = order.Status;
                existingOrder.TotalAmount = order.TotalAmount;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _orders.Remove(order);
            }
            await Task.CompletedTask;
        }

        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.Status = status;
            }
            await Task.CompletedTask;
        }
    }
}