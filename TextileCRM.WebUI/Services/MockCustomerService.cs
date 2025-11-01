using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockCustomerService : ICustomerService
    {
        private readonly List<Customer> _customers;

        public MockCustomerService()
        {
            _customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "ABC Tekstil", ContactName = "Ahmet Yılmaz", Email = "ahmet@abctekstil.com", Phone = "0212 555 1234", Address = "İstanbul" },
                new Customer { Id = 2, Name = "XYZ Giyim", ContactName = "Mehmet Demir", Email = "mehmet@xyzgiyim.com", Phone = "0216 444 5678", Address = "İzmir" },
                new Customer { Id = 3, Name = "Moda Kumaş", ContactName = "Ayşe Kaya", Email = "ayse@modakumas.com", Phone = "0312 333 9876", Address = "Ankara" }
            };
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await Task.FromResult(_customers);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            customer.Id = _customers.Max(c => c.Id) + 1;
            _customers.Add(customer);
            await Task.CompletedTask;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.ContactName = customer.ContactName;
                existingCustomer.CompanyName = customer.CompanyName;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            await Task.CompletedTask;
        }
    }
}