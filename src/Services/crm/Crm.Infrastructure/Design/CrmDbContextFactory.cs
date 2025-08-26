using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Crm.Infrastructure.Data;

namespace Crm.Infrastructure.Design;

/// <summary>
/// Design-time factory for CrmDbContext to support EF migrations
/// </summary>
public class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrmDbContext>();
        
        // Use a development connection string for migrations
        // In production, this will be overridden by the actual configuration
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tosserpdb;Username=postgres;Password=postgres123");
        
        return new CrmDbContext(optionsBuilder.Options);
    }
}
