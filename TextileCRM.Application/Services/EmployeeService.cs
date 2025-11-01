using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(Department department)
        {
            return await _employeeRepository.FindAsync(e => e.Department == department);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            employee.CreatedDate = DateTime.Now;
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeeRepository.DeleteAsync(id);
            await _employeeRepository.SaveChangesAsync();
        }
    }
}