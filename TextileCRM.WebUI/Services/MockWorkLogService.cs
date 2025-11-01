using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockWorkLogService : IWorkLogService
    {
        private readonly List<WorkLog> _workLogs;

        public MockWorkLogService()
        {
            _workLogs = new List<WorkLog>
            {
                new WorkLog { Id = 1, EmployeeId = 1, CheckInTime = DateTime.Now.AddDays(-1).AddHours(-8), CheckOutTime = DateTime.Now.AddDays(-1), Notes = "Normal mesai" },
                new WorkLog { Id = 2, EmployeeId = 2, CheckInTime = DateTime.Now.AddDays(-1).AddHours(-8), CheckOutTime = DateTime.Now.AddDays(-1), Notes = "Fazla mesai" },
                new WorkLog { Id = 3, EmployeeId = 3, CheckInTime = DateTime.Now.AddHours(-4), CheckOutTime = null, Notes = "Devam ediyor" }
            };
        }

        public async Task<IEnumerable<WorkLog>> GetAllWorkLogsAsync()
        {
            return await Task.FromResult(_workLogs);
        }

        public async Task<WorkLog?> GetWorkLogByIdAsync(int id)
        {
            return await Task.FromResult(_workLogs.FirstOrDefault(w => w.Id == id));
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsByEmployeeIdAsync(int employeeId)
        {
            return await Task.FromResult(_workLogs.Where(w => w.EmployeeId == employeeId).ToList());
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(_workLogs.Where(w => w.CheckInTime >= startDate && w.CheckInTime <= endDate).ToList());
        }

        public async Task CreateWorkLogAsync(WorkLog workLog)
        {
            workLog.Id = _workLogs.Max(w => w.Id) + 1;
            _workLogs.Add(workLog);
            await Task.CompletedTask;
        }

        public async Task UpdateWorkLogAsync(WorkLog workLog)
        {
            var existingWorkLog = _workLogs.FirstOrDefault(w => w.Id == workLog.Id);
            if (existingWorkLog != null)
            {
                existingWorkLog.EmployeeId = workLog.EmployeeId;
                existingWorkLog.CheckInTime = workLog.CheckInTime;
                existingWorkLog.CheckOutTime = workLog.CheckOutTime;
                existingWorkLog.WorkHours = workLog.WorkHours;
                existingWorkLog.Notes = workLog.Notes;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteWorkLogAsync(int id)
        {
            var workLog = _workLogs.FirstOrDefault(w => w.Id == id);
            if (workLog != null)
            {
                _workLogs.Remove(workLog);
            }
            await Task.CompletedTask;
        }

        public async Task CheckInAsync(int employeeId)
        {
            var workLog = new WorkLog
            {
                Id = _workLogs.Max(w => w.Id) + 1,
                EmployeeId = employeeId,
                CheckInTime = DateTime.Now,
                CheckOutTime = null,
                Notes = "Yeni giriş"
            };
            _workLogs.Add(workLog);
            await Task.CompletedTask;
        }

        public async Task CheckOutAsync(int workLogId)
        {
            var workLog = _workLogs.FirstOrDefault(w => w.Id == workLogId);
            if (workLog != null)
            {
                workLog.CheckOutTime = DateTime.Now;
            }
            await Task.CompletedTask;
        }
    }
}