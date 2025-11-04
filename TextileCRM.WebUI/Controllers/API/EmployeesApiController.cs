using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesApiController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Tüm çalışanları listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        /// <summary>
        /// ID'ye göre çalışan getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Çalışan bulunamadı" });
            }
            return Ok(employee);
        }

        /// <summary>
        /// Yeni çalışan oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee.CreatedDate = DateTime.Now;
            await _employeeService.CreateEmployeeAsync(employee);
            
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Çalışan bilgilerini günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Çalışan bulunamadı" });
            }

            await _employeeService.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        /// <summary>
        /// Çalışan siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Çalışan bulunamadı" });
            }

            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Departmana göre çalışanları filtreler
        /// </summary>
        [HttpGet("department/{department}")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetByDepartment(Department department)
        {
            var allEmployees = await _employeeService.GetAllEmployeesAsync();
            var filteredEmployees = allEmployees.Where(e => e.Department == department);
            
            return Ok(filteredEmployees);
        }

        /// <summary>
        /// İsme göre çalışan arar
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAll();
            }

            var allEmployees = await _employeeService.GetAllEmployeesAsync();
            var filteredEmployees = allEmployees.Where(e =>
                e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Department.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );

            return Ok(filteredEmployees);
        }

        /// <summary>
        /// Aktif çalışanları listeler
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetActive()
        {
            var allEmployees = await _employeeService.GetAllEmployeesAsync();
            var activeEmployees = allEmployees.Where(e => e.TerminationDate == null);
            
            return Ok(activeEmployees);
        }
    }
}

