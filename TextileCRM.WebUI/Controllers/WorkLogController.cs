using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers
{
    [Authorize]
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IEmployeeService _employeeService;

        public WorkLogController(
            IWorkLogService workLogService,
            IEmployeeService employeeService)
        {
            _workLogService = workLogService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var workLogs = await _workLogService.GetAllWorkLogsAsync();
            return View(workLogs);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateEmployeeDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkLog workLog)
        {
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                await _workLogService.CreateWorkLogAsync(workLog);
                return RedirectToAction(nameof(Index));
            }
            await PopulateEmployeeDropdown();
            return View(workLog);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }
            await PopulateEmployeeDropdown();
            return View(workLog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkLog workLog)
        {
            if (id != workLog.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                await _workLogService.UpdateWorkLogAsync(workLog);
                return RedirectToAction(nameof(Index));
            }
            await PopulateEmployeeDropdown();
            return View(workLog);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }
            return View(workLog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _workLogService.DeleteWorkLogAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(int employeeId)
        {
            await _workLogService.CheckInAsync(employeeId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(int workLogId)
        {
            await _workLogService.CheckOutAsync(workLogId);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEmployeeDropdown()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "FullName");
        }
    }
}