
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
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<FileAttachment> FileAttachments { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<StockAlert> StockAlerts { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Identity columns
        modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .UseIdentityColumn(1, 1); // Seed: 1, Increment: 1
            
        modelBuilder.Entity<Order>()
            .Property(o => o.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<WorkLog>()
            .Property(w => w.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<Invoice>()
            .Property(i => i.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<Payment>()
            .Property(p => p.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<FileAttachment>()
            .Property(f => f.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<Notification>()
            .Property(n => n.Id)
            .UseIdentityColumn(1, 1);
            
        modelBuilder.Entity<StockAlert>()
            .Property(s => s.Id)
            .UseIdentityColumn(1, 1);
        
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
            
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<StockAlert>()
            .HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<FileAttachment>()
            .HasOne(f => f.UploadedByUser)
            .WithMany()
            .HasForeignKey(f => f.UploadedBy)
            .OnDelete(DeleteBehavior.NoAction);
            
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

