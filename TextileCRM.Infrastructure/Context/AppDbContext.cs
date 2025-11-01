
using Microsoft.EntityFrameworkCore;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<WorkLog> WorkLogs { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId);
            
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);
            
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);
            
        modelBuilder.Entity<WorkLog>()
            .HasOne(wl => wl.Employee)
            .WithMany(e => e.WorkLogs)
            .HasForeignKey(wl => wl.EmployeeId);
    }
}

