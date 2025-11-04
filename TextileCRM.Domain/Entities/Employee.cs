using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    
    public string FullName => $"{FirstName} {LastName}";
}

public enum Department
{
    [Display(Name = "Yönetim")]
    Management,
    
    [Display(Name = "Satış")]
    Sales,
    
    [Display(Name = "Üretim")]
    Production,
    
    [Display(Name = "Kesim")]
    Cutting,
    
    [Display(Name = "Dikiş")]
    Sewing,
    
    [Display(Name = "Paketleme")]
    Packaging,
    
    [Display(Name = "Depo")]
    Warehouse,
    
    [Display(Name = "Muhasebe")]
    Accounting,
    
    [Display(Name = "İnsan Kaynakları")]
    HumanResources
}