using System;
using System.Collections.Generic;

namespace TextileCRM.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal Salary { get; set; }
    public Department Department { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}

public enum Department
{
    Management,
    Sales,
    Production,
    Cutting,
    Sewing,
    Packaging,
    Warehouse,
    Accounting,
    HumanResources
}