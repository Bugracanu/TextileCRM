using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? PaidDate { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public Order Order { get; set; } = null!;
    [JsonIgnore]
    public Customer Customer { get; set; } = null!;
    [JsonIgnore]
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum InvoiceStatus
{
    [Display(Name = "Taslak")]
    Draft,
    
    [Display(Name = "Gönderildi")]
    Sent,
    
    [Display(Name = "Kısmi Ödendi")]
    PartiallyPaid,
    
    [Display(Name = "Ödendi")]
    Paid,
    
    [Display(Name = "Gecikmiş")]
    Overdue,
    
    [Display(Name = "İptal Edildi")]
    Cancelled
}

