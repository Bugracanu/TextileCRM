using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace TextileCRM.Infrastructure.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "TextileCRM.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new AppDbContext(optionsBuilder.Options);
    }
}