using System;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public Employee Employee { get; set; } = null!;
}