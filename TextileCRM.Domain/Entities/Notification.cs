using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public bool IsRead { get; set; }
    public string? Link { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ReadDate { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public User User { get; set; } = null!;
}

public enum NotificationType
{
    [Display(Name = "Bilgi")]
    Info,
    
    [Display(Name = "Başarılı")]
    Success,
    
    [Display(Name = "Uyarı")]
    Warning,
    
    [Display(Name = "Hata")]
    Error,
    
    [Display(Name = "Sipariş")]
    Order,
    
    [Display(Name = "Ödeme")]
    Payment,
    
    [Display(Name = "Stok")]
    Stock,
    
    [Display(Name = "Sistem")]
    System
}

public enum NotificationPriority
{
    [Display(Name = "Düşük")]
    Low,
    
    [Display(Name = "Normal")]
    Normal,
    
    [Display(Name = "Yüksek")]
    High,
    
    [Display(Name = "Acil")]
    Urgent
}

