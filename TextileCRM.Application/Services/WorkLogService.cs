using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public WorkLogService(IRepository<WorkLog> workLogRepository, IRepository<Employee> employeeRepository)
        {
            _workLogRepository = workLogRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<WorkLog>> GetAllWorkLogsAsync()
        {
            return await _workLogRepository.GetAllAsync();
        }

        public async Task<WorkLog> GetWorkLogByIdAsync(int id)
        {
            return await _workLogRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsByEmployeeIdAsync(int employeeId)
        {
            return await _workLogRepository.FindAsync(wl => wl.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _workLogRepository.FindAsync(wl => wl.CheckInTime >= startDate && wl.CheckInTime <= endDate);
        }

        public async Task CreateWorkLogAsync(WorkLog workLog)
        {
            workLog.CreatedDate = DateTime.Now;
            await _workLogRepository.AddAsync(workLog);
            await _workLogRepository.SaveChangesAsync();
        }

        public async Task UpdateWorkLogAsync(WorkLog workLog)
        {
            await _workLogRepository.UpdateAsync(workLog);
            await _workLogRepository.SaveChangesAsync();
        }

        public async Task DeleteWorkLogAsync(int id)
        {
            await _workLogRepository.DeleteAsync(id);
            await _workLogRepository.SaveChangesAsync();
        }

        public async Task CheckInAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee != null)
            {
                var workLog = new WorkLog
                {
                    EmployeeId = employeeId,
                    CheckInTime = DateTime.Now,
                    CreatedDate = DateTime.Now
                };
                
                await _workLogRepository.AddAsync(workLog);
                await _workLogRepository.SaveChangesAsync();
            }
        }

        public async Task CheckOutAsync(int workLogId)
        {
            var workLog = await _workLogRepository.GetByIdAsync(workLogId);
            if (workLog != null && !workLog.CheckOutTime.HasValue)
            {
                workLog.CheckOutTime = DateTime.Now;
                
                // Calculate work hours
                var duration = workLog.CheckOutTime.Value - workLog.CheckInTime;
                workLog.WorkHours = (decimal)duration.TotalHours;
                
                await _workLogRepository.UpdateAsync(workLog);
                await _workLogRepository.SaveChangesAsync();
            }
        }
    }
}