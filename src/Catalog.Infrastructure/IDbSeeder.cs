namespace Catalog.Infrastructure;

public interface IDbSeeder<TContext> where TContext : class
{
    Task SeedAsync(TContext context);
} 
