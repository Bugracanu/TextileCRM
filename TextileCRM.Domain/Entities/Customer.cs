namespace TextileCRM.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

