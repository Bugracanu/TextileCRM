using System;

namespace TextileCRM.Domain.Entities;

public class WorkLog
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal? WorkHours { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public Employee Employee { get; set; }
}