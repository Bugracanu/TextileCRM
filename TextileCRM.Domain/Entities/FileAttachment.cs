using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TextileCRM.Domain.Entities;

public class FileAttachment
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public FileCategory Category { get; set; }
    public string EntityType { get; set; } = string.Empty; // Order, Invoice, Product etc.
    public int EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public User? UploadedByUser { get; set; }
}

public enum FileCategory
{
    [Display(Name = "Fatura")]
    Invoice,
    
    [Display(Name = "Ödeme Makbuzu")]
    PaymentReceipt,
    
    [Display(Name = "Tasarım Dosyası")]
    DesignFile,
    
    [Display(Name = "Ürün Resmi")]
    ProductImage,
    
    [Display(Name = "Sipariş Dökümanı")]
    OrderDocument,
    
    [Display(Name = "Sözleşme")]
    Contract,
    
    [Display(Name = "Diğer")]
    Other
}

