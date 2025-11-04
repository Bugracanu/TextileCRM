using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class StockAlert
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CurrentQuantity { get; set; }
    public int ThresholdQuantity { get; set; }
    public StockAlertType AlertType { get; set; }
    public StockAlertStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public int? ResolvedBy { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public Product Product { get; set; } = null!;
}

public enum StockAlertType
{
    [Display(Name = "Düşük Stok")]
    LowStock,
    
    [Display(Name = "Stok Bitti")]
    OutOfStock,
    
    [Display(Name = "Yeniden Sipariş Noktası")]
    ReorderPoint,
    
    [Display(Name = "Fazla Stok")]
    OverStock
}

public enum StockAlertStatus
{
    [Display(Name = "Aktif")]
    Active,
    
    [Display(Name = "Çözüldü")]
    Resolved,
    
    [Display(Name = "Göz Ardı Edildi")]
    Ignored
}

