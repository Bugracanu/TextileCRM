using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
            await _customerRepository.SaveChangesAsync();
        }
    }
}