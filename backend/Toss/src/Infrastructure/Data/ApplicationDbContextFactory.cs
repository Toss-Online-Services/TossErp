using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Toss.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances during EF migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a default connection string for migrations
        // This will be replaced by actual connection strings from configuration at runtime
        var connectionString = "Server=127.0.0.1;Port=5432;Database=TossErp;Username=toss;Password=toss123;";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}