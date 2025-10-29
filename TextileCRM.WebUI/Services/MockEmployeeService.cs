using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockEmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees;

        public MockEmployeeService()
        {
            _employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Ali", LastName = "Yılmaz", Email = "ali@example.com", Phone = "555-111-2222", Department = Department.Production, HireDate = DateTime.Now.AddYears(-2) },
                new Employee { Id = 2, FirstName = "Zeynep", LastName = "Kaya", Email = "zeynep@example.com", Phone = "555-222-3333", Department = Department.Sales, HireDate = DateTime.Now.AddYears(-1) },
                new Employee { Id = 3, FirstName = "Mustafa", LastName = "Demir", Email = "mustafa@example.com", Phone = "555-333-4444", Department = Department.Management, HireDate = DateTime.Now.AddMonths(-6) }
            };
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await Task.FromResult(_employees);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(Department department)
        {
            return await Task.FromResult(_employees.Where(e => e.Department == department).ToList());
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            employee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(employee);
            await Task.CompletedTask;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existingEmployee != null)
            {
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.Email = employee.Email;
                existingEmployee.Phone = employee.Phone;
                existingEmployee.Department = employee.Department;
                existingEmployee.HireDate = employee.HireDate;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                _employees.Remove(employee);
            }
            await Task.CompletedTask;
        }
    }
}