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
    public class WorkLogsApiController : ControllerBase
    {
        private readonly IWorkLogService _workLogService;
        private readonly IEmployeeService _employeeService;

        public WorkLogsApiController(IWorkLogService workLogService, IEmployeeService employeeService)
        {
            _workLogService = workLogService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Tüm çalışma kayıtlarını listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkLog>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkLog>>> GetAll()
        {
            var workLogs = await _workLogService.GetAllWorkLogsAsync();
            return Ok(workLogs);
        }

        /// <summary>
        /// ID'ye göre çalışma kaydı getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkLog), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkLog>> GetById(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound(new { message = "Çalışma kaydı bulunamadı" });
            }
            return Ok(workLog);
        }

        /// <summary>
        /// Yeni çalışma kaydı oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(WorkLog), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkLog>> Create([FromBody] WorkLog workLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Çalışan kontrolü
            var employee = await _employeeService.GetEmployeeByIdAsync(workLog.EmployeeId);
            if (employee == null)
            {
                return BadRequest(new { message = "Geçersiz çalışan ID" });
            }

            workLog.CreatedDate = DateTime.Now;
            await _workLogService.CreateWorkLogAsync(workLog);
            
            return CreatedAtAction(nameof(GetById), new { id = workLog.Id }, workLog);
        }

        /// <summary>
        /// Çalışma kaydını günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] WorkLog workLog)
        {
            if (id != workLog.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingWorkLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (existingWorkLog == null)
            {
                return NotFound(new { message = "Çalışma kaydı bulunamadı" });
            }

            await _workLogService.UpdateWorkLogAsync(workLog);
            return NoContent();
        }

        /// <summary>
        /// Çalışma kaydını siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound(new { message = "Çalışma kaydı bulunamadı" });
            }

            await _workLogService.DeleteWorkLogAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Çalışana ait kayıtları listeler
        /// </summary>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(IEnumerable<WorkLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WorkLog>>> GetByEmployee(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { message = "Çalışan bulunamadı" });
            }

            var workLogs = await _workLogService.GetWorkLogsByEmployeeIdAsync(employeeId);
            return Ok(workLogs);
        }

        /// <summary>
        /// Tarih aralığına göre çalışma kayıtlarını filtreler
        /// </summary>
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<WorkLog>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkLog>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var filteredWorkLogs = await _workLogService.GetWorkLogsByDateRangeAsync(startDate, endDate);
            
            return Ok(filteredWorkLogs);
        }

        /// <summary>
        /// Çalışanın toplam çalışma saatini hesaplar
        /// </summary>
        [HttpGet("employee/{employeeId}/total-hours")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetTotalHours(
            int employeeId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { message = "Çalışan bulunamadı" });
            }

            var workLogs = await _workLogService.GetWorkLogsByEmployeeIdAsync(employeeId);
            
            if (startDate.HasValue && endDate.HasValue)
            {
                workLogs = workLogs.Where(w => w.CheckInTime >= startDate.Value && w.CheckInTime <= endDate.Value);
            }

            var totalHours = workLogs.Sum(w => w.WorkHours ?? 0);
            
            return Ok(new { 
                employeeId = employeeId, 
                employeeName = $"{employee.FirstName} {employee.LastName}",
                totalHours = totalHours,
                startDate = startDate,
                endDate = endDate,
                recordCount = workLogs.Count()
            });
        }
    }
}

