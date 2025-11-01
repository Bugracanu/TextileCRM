using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IWorkLogService
    {
        Task<IEnumerable<WorkLog>> GetAllWorkLogsAsync();
        Task<WorkLog?> GetWorkLogByIdAsync(int id);
        Task<IEnumerable<WorkLog>> GetWorkLogsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<WorkLog>> GetWorkLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task CreateWorkLogAsync(WorkLog workLog);
        Task UpdateWorkLogAsync(WorkLog workLog);
        Task DeleteWorkLogAsync(int id);
        Task CheckInAsync(int employeeId);
        Task CheckOutAsync(int workLogId);
    }
}