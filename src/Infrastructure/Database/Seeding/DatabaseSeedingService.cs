using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Infrastructure.Database.Seeding;

/// <summary>
/// Interface for database seeders
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Execution order (lower values execute first)
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Environment where this seeder should run
    /// </summary>
    string[] Environments { get; }

    /// <summary>
    /// Seeds data into the database
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SeedAsync(DbContext context, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstract base class for database seeders
/// </summary>
public abstract class BaseDatabaseSeeder : IDatabaseSeeder
{
    public abstract int Order { get; }
    public virtual string[] Environments => new[] { "Development", "Staging", "Production" };

    public abstract Task SeedAsync(DbContext context, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds data only if it doesn't already exist
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="entities">Entities to seed</param>
    /// <param name="keySelector">Key selector for uniqueness check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task SeedIfNotExistsAsync<T>(
        DbContext context,
        IEnumerable<T> entities,
        Func<T, object> keySelector,
        CancellationToken cancellationToken = default) where T : class
    {
        var dbSet = context.Set<T>();
        var existingKeys = await dbSet.Select(e => keySelector(e)).ToListAsync(cancellationToken);

        var newEntities = entities.Where(e => !existingKeys.Contains(keySelector(e))).ToList();

        if (newEntities.Count > 0)
        {
            await dbSet.AddRangeAsync(newEntities, cancellationToken);
        }
    }

    /// <summary>
    /// Truncates and re-seeds reference data
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="entities">Entities to seed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task TruncateAndSeedAsync<T>(
        DbContext context,
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default) where T : class
    {
        var dbSet = context.Set<T>();
        
        // Remove existing data
        var existing = await dbSet.ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            dbSet.RemoveRange(existing);
        }

        // Add new data
        await dbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Executes raw SQL with proper error handling
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="sql">SQL to execute</param>
    /// <param name="parameters">Parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task ExecuteSqlAsync(
        DbContext context,
        string sql,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(sql, parameters ?? Array.Empty<object>(), cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute SQL: {sql}", ex);
        }
    }
}

/// <summary>
/// Database seeding service
/// </summary>
public interface IDatabaseSeedingService
{
    /// <summary>
    /// Seeds all registered seeders for the current environment
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SeedAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds specific seeder by type
    /// </summary>
    /// <typeparam name="T">Seeder type</typeparam>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SeedAsync<T>(CancellationToken cancellationToken = default) where T : IDatabaseSeeder;
}

/// <summary>
/// Default database seeding service implementation
/// </summary>
public class DatabaseSeedingService : IDatabaseSeedingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseSeedingService> _logger;

    public DatabaseSeedingService(
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        ILogger<DatabaseSeedingService> logger)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database seeding for environment: {Environment}", _environment.EnvironmentName);

        var seeders = _serviceProvider.GetServices<IDatabaseSeeder>()
            .Where(s => s.Environments.Contains(_environment.EnvironmentName, StringComparer.OrdinalIgnoreCase))
            .OrderBy(s => s.Order)
            .ToList();

        _logger.LogInformation("Found {Count} seeders to execute", seeders.Count);

        using var scope = _serviceProvider.CreateScope();
        
        foreach (var seeder in seeders)
        {
            try
            {
                _logger.LogInformation("Executing seeder: {SeederType}", seeder.GetType().Name);

                // Get the appropriate DbContext for this seeder
                var dbContext = GetDbContextForSeeder(scope.ServiceProvider, seeder);
                
                await seeder.SeedAsync(dbContext, scope.ServiceProvider, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully completed seeder: {SeederType}", seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute seeder: {SeederType}", seeder.GetType().Name);
                throw;
            }
        }

        _logger.LogInformation("Database seeding completed successfully");
    }

    public async Task SeedAsync<T>(CancellationToken cancellationToken = default) where T : IDatabaseSeeder
    {
        _logger.LogInformation("Starting specific seeder: {SeederType}", typeof(T).Name);

        var seeder = _serviceProvider.GetService<T>()
            ?? throw new InvalidOperationException($"Seeder {typeof(T).Name} is not registered");

        if (!seeder.Environments.Contains(_environment.EnvironmentName, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Seeder {SeederType} is not configured for environment {Environment}", 
                typeof(T).Name, _environment.EnvironmentName);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = GetDbContextForSeeder(scope.ServiceProvider, seeder);

        try
        {
            await seeder.SeedAsync(dbContext, scope.ServiceProvider, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully completed seeder: {SeederType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute seeder: {SeederType}", typeof(T).Name);
            throw;
        }
    }

    private DbContext GetDbContextForSeeder(IServiceProvider serviceProvider, IDatabaseSeeder seeder)
    {
        // Convention: Look for a property or attribute that specifies the DbContext type
        var seederType = seeder.GetType();
        
        // Check for DbContextAttribute
        var contextAttribute = seederType.GetCustomAttributes(typeof(DbContextAttribute), false)
            .FirstOrDefault() as DbContextAttribute;

        if (contextAttribute != null)
        {
            return (DbContext)serviceProvider.GetRequiredService(contextAttribute.ContextType);
        }

        // Fallback: Look for any registered DbContext
        var dbContextTypes = serviceProvider.GetServices<DbContext>().ToList();
        
        if (dbContextTypes.Count == 1)
        {
            return dbContextTypes.First();
        }

        throw new InvalidOperationException(
            $"Cannot determine DbContext for seeder {seederType.Name}. " +
            "Use [DbContext(typeof(YourDbContext))] attribute or register only one DbContext.");
    }
}

/// <summary>
/// Attribute to specify which DbContext a seeder should use
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DbContextAttribute : Attribute
{
    public Type ContextType { get; }

    public DbContextAttribute(Type contextType)
    {
        if (!typeof(DbContext).IsAssignableFrom(contextType))
        {
            throw new ArgumentException($"Type {contextType.Name} must inherit from DbContext", nameof(contextType));
        }

        ContextType = contextType;
    }
}

/// <summary>
/// Service collection extensions for database seeding
/// </summary>
public static class DatabaseSeedingExtensions
{
    /// <summary>
    /// Adds database seeding services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabaseSeeding(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeedingService, DatabaseSeedingService>();
        return services;
    }

    /// <summary>
    /// Registers a database seeder
    /// </summary>
    /// <typeparam name="T">Seeder type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabaseSeeder<T>(this IServiceCollection services)
        where T : class, IDatabaseSeeder
    {
        services.AddScoped<IDatabaseSeeder, T>();
        services.AddScoped<T>();
        return services;
    }

    /// <summary>
    /// Seeds the database during application startup
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <param name="seedInProduction">Whether to seed in production environment</param>
    /// <returns>Application builder</returns>
    public static async Task<IHostApplicationBuilder> SeedDatabaseAsync(
        this IHostApplicationBuilder app,
        bool seedInProduction = false)
    {
        var environment = app.Environment;
        
        if (!seedInProduction && environment.IsProduction())
        {
            return app;
        }

        using var scope = app.Services.CreateScope();
        var seedingService = scope.ServiceProvider.GetService<IDatabaseSeedingService>();

        if (seedingService != null)
        {
            await seedingService.SeedAllAsync();
        }

        return app;
    }
}
