using System;
using System.Collections.Generic;

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
    public Customer Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending,
    InProduction,
    Completed,
    Delivered,
    Cancelled,
    Processing,
    Confirmed,
    New

}