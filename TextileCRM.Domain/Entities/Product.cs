namespace TextileCRM.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum ProductCategory
{
    Fabric,
    Thread,
    Button,
    Zipper,
    Accessory,
    FinishedProduct,
    Other
}