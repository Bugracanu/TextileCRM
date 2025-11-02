
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
    public DbSet<User> Users { get; set; } = null!;
    
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
            
        // Seed data
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Username = "admin", 
                PasswordHash = "admin123",
                Role = "Admin", 
                IsActive = true,
                CreatedDate = new DateTime(2024, 6, 1)
            },
            new User 
            { 
                Id = 2, 
                Username = "manager", 
                PasswordHash = "manager123",
                Role = "Manager", 
                IsActive = true,
                CreatedDate = new DateTime(2024, 9, 1)
            },
            new User 
            { 
                Id = 3, 
                Username = "user", 
                PasswordHash = "user123",
                Role = "User", 
                IsActive = true,
                CreatedDate = new DateTime(2024, 10, 1)
            }
        );
    }
}

