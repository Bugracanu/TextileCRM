using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public Invoice Invoice { get; set; } = null!;
}

public enum PaymentMethod
{
    [Display(Name = "Nakit")]
    Cash,
    
    [Display(Name = "Kredi Kartı")]
    CreditCard,
    
    [Display(Name = "Banka Transferi")]
    BankTransfer,
    
    [Display(Name = "Çek")]
    Check,
    
    [Display(Name = "Havale/EFT")]
    WireTransfer,
    
    [Display(Name = "Diğer")]
    Other
}

public enum PaymentStatus
{
    [Display(Name = "Beklemede")]
    Pending,
    
    [Display(Name = "İşleniyor")]
    Processing,
    
    [Display(Name = "Tamamlandı")]
    Completed,
    
    [Display(Name = "Başarısız")]
    Failed,
    
    [Display(Name = "İade Edildi")]
    Refunded
}

