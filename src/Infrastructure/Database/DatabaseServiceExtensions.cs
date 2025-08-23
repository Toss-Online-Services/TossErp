using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

namespace TossErp.Infrastructure.Database;

/// <summary>
/// Database service extensions for TOSS ERP III
/// </summary>
public static class DatabaseServiceExtensions
{
    /// <summary>
    /// Adds standard TOSS database configuration
    /// </summary>
    /// <typeparam name="TContext">Database context type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="connectionStringName">Connection string name (default: "DefaultConnection")</param>
    /// <param name="configureSqlOptions">Optional SQL options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddTossDatabase<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection",
        Action<NpgsqlDbContextOptionsBuilder>? configureSqlOptions = null)
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Standard TOSS configurations
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);

                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.MigrationsAssembly(typeof(TContext).Assembly.FullName);

                // Apply custom configurations
                configureSqlOptions?.Invoke(npgsqlOptions);
            });

            // Configure based on environment
            var environment = services.BuildServiceProvider().GetService<IHostEnvironment>();
            if (environment?.IsDevelopment() == true)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<TContext>(
                name: $"database-{typeof(TContext).Name.ToLowerInvariant()}",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

        return services;
    }

    /// <summary>
    /// Adds read-only database context (typically for reporting/analytics)
    /// </summary>
    /// <typeparam name="TContext">Database context type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="connectionStringName">Connection string name (default: "ReadOnlyConnection")</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddTossReadOnlyDatabase<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "ReadOnlyConnection")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);

                npgsqlOptions.CommandTimeout(60); // Longer timeout for read operations
            });

            // Read-only contexts should not track changes
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }

    /// <summary>
    /// Adds database connection factory for advanced scenarios
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabaseConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionOptions>(
            configuration.GetSection("Database"));

        services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();

        return services;
    }

    /// <summary>
    /// Configures standard Npgsql data source
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Connection string</param>
    /// <param name="configureDataSource">Optional data source configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection services,
        string connectionString,
        Action<NpgsqlDataSourceBuilder>? configureDataSource = null)
    {
        services.AddNpgsqlDataSource(connectionString, builder =>
        {
            // Standard TOSS configurations
            builder.EnableParameterLogging();
            builder.EnableDynamicJson();

            // Apply custom configurations
            configureDataSource?.Invoke(builder);
        });

        return services;
    }
}

/// <summary>
/// Database connection factory interface
/// </summary>
public interface IDatabaseConnectionFactory
{
    /// <summary>
    /// Creates a new database connection
    /// </summary>
    /// <param name="tenantId">Optional tenant ID for connection context</param>
    /// <returns>Database connection</returns>
    NpgsqlConnection CreateConnection(Guid? tenantId = null);

    /// <summary>
    /// Creates a new database connection with specific options
    /// </summary>
    /// <param name="options">Connection options</param>
    /// <returns>Database connection</returns>
    NpgsqlConnection CreateConnection(DatabaseConnectionOptions options);
}

/// <summary>
/// Default database connection factory implementation
/// </summary>
public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseConnectionFactory> _logger;

    public DatabaseConnectionFactory(
        IConfiguration configuration,
        ILogger<DatabaseConnectionFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public NpgsqlConnection CreateConnection(Guid? tenantId = null)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Default connection string not found.");

        var connection = new NpgsqlConnection(connectionString);

        // Set connection-level configurations
        if (tenantId.HasValue)
        {
            connection.Notice += (sender, e) =>
            {
                _logger.LogDebug("PostgreSQL Notice: {Message}", e.Notice.MessageText);
            };
        }

        return connection;
    }

    public NpgsqlConnection CreateConnection(DatabaseConnectionOptions options)
    {
        var connection = new NpgsqlConnection(options.ConnectionString);

        // Apply connection-specific settings
        connection.Notice += (sender, e) =>
        {
            _logger.LogDebug("PostgreSQL Notice: {Message}", e.Notice.MessageText);
        };

        return connection;
    }
}

/// <summary>
/// Database health check extensions
/// </summary>
public static class DatabaseHealthCheckExtensions
{
    /// <summary>
    /// Adds comprehensive database health checks
    /// </summary>
    /// <param name="builder">Health checks builder</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="name">Health check name</param>
    /// <param name="tags">Health check tags</param>
    /// <returns>Health checks builder</returns>
    public static Microsoft.Extensions.DependencyInjection.IHealthChecksBuilder AddTossDatabaseHealthCheck(
        this Microsoft.Extensions.DependencyInjection.IHealthChecksBuilder builder,
        IConfiguration configuration,
        string name = "database",
        string[]? tags = null)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Default connection string not found.");

        return builder.AddNpgSql(
            connectionString: connectionString,
            healthQuery: "SELECT 1;",
            name: name,
            failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            tags: tags ?? new[] { "database", "postgresql" },
            timeout: TimeSpan.FromSeconds(10));
    }
}
