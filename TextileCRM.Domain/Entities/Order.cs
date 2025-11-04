using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public Customer Customer { get; set; } = null!;
    [JsonIgnore]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    [Display(Name = "Yeni")]
    New,
    
    [Display(Name = "Beklemede")]
    Pending,
    
    [Display(Name = "Onaylandı")]
    Confirmed,
    
    [Display(Name = "İşleniyor")]
    Processing,
    
    [Display(Name = "Üretimde")]
    InProduction,
    
    [Display(Name = "Tamamlandı")]
    Completed,
    
    [Display(Name = "Teslim Edildi")]
    Delivered,
    
    [Display(Name = "İptal Edildi")]
    Cancelled
}