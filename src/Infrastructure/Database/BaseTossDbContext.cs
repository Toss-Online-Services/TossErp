using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Infrastructure.Database;

/// <summary>
/// Base database context providing common TOSS ERP III patterns
/// </summary>
public abstract class BaseTossDbContext : DbContext
{
    private readonly ILogger<BaseTossDbContext> _logger;
    private readonly string? _tenantId;

    protected BaseTossDbContext(DbContextOptions options, ILogger<BaseTossDbContext> logger) : base(options)
    {
        _logger = logger;
        _tenantId = GetTenantIdFromContext();
    }

    /// <summary>
    /// Sets the tenant context for Row Level Security
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SetTenantContextAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId.HasValue && tenantId != Guid.Empty)
        {
            var connection = Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, false)";
            
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tenantId";
            parameter.Value = tenantId.ToString();
            command.Parameters.Add(parameter);

            await command.ExecuteNonQueryAsync(cancellationToken);
            
            _logger.LogDebug("Set tenant context to {TenantId}", tenantId);
        }
        else
        {
            _logger.LogWarning("Attempted to set invalid tenant context: {TenantId}", tenantId);
        }
    }

    /// <summary>
    /// Gets the current tenant ID from the database session
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current tenant ID or null</returns>
    public async Task<Guid?> GetCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT CURRENT_SETTING('app.current_tenant_id', true)";
            
            var result = await command.ExecuteScalarAsync(cancellationToken);
            
            if (result != null && Guid.TryParse(result.ToString(), out var tenantId))
            {
                return tenantId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get current tenant from database session");
        }

        return null;
    }

    /// <summary>
    /// Executes a command with elevated privileges (bypassing RLS)
    /// </summary>
    /// <param name="sql">SQL command</param>
    /// <param name="parameters">Parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ExecuteAsSystemAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default)
    {
        await Database.ExecuteSqlRawAsync(sql, parameters ?? Array.Empty<object>(), cancellationToken);
    }

    /// <summary>
    /// Checks if the database connection is healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy</returns>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return false;
        }
    }

    /// <summary>
    /// Gets database schema information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Schema info</returns>
    public async Task<DatabaseSchemaInfo> GetSchemaInfoAsync(CancellationToken cancellationToken = default)
    {
        var connection = Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                version() as version,
                current_database() as database_name,
                current_schema() as current_schema,
                session_user as session_user,
                current_user as current_user
        ";

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new DatabaseSchemaInfo
            {
                Version = reader.GetString("version"),
                DatabaseName = reader.GetString("database_name"),
                CurrentSchema = reader.GetString("current_schema"),
                SessionUser = reader.GetString("session_user"),
                CurrentUser = reader.GetString("current_user")
            };
        }

        throw new InvalidOperationException("Failed to retrieve database schema information");
    }

    /// <summary>
    /// Applies standard entity configurations
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isDeletedProperty = entityType.FindProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
            {
                var entityClrType = entityType.ClrType;
                var parameter = System.Linq.Expressions.Expression.Parameter(entityClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityClrType).HasQueryFilter(lambda);
            }
        }

        // Configure standard naming conventions
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Configure table names (PascalCase to snake_case)
            if (entity.GetTableName() != null)
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));
            }

            // Configure column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            // Configure foreign key names
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName($"FK_{ToSnakeCase(entity.GetTableName()!)}_{ToSnakeCase(foreignKey.DependentToPrincipal?.Name ?? "Unknown")}");
            }

            // Configure index names
            foreach (var index in entity.GetIndexes())
            {
                if (index.Name != null)
                {
                    index.SetDatabaseName(ToSnakeCase(index.Name));
                }
            }
        }
    }

    /// <summary>
    /// Configures the database provider with standard settings
    /// </summary>
    /// <param name="optionsBuilder">Options builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            _logger.LogWarning("DbContext is not configured. This should only happen during design-time operations.");
            return;
        }

        // Standard PostgreSQL configurations
        optionsBuilder.UseNpgsql(options =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
                
            options.CommandTimeout(30);
        });

        // Enable detailed logging in development
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(message => _logger.LogDebug("{Message}", message));
        }
    }

    private string? GetTenantIdFromContext()
    {
        // This would typically be injected via dependency injection
        // from the current request context (e.g., JWT claims, headers)
        return null;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (char.IsUpper(c))
            {
                if (i > 0 && input[i - 1] != '_')
                {
                    result.Append('_');
                }
                result.Append(char.ToLower(c));
            }
            else
            {
                result.Append(c);
            }
        }
        
        return result.ToString();
    }
}

/// <summary>
/// Database schema information
/// </summary>
public record DatabaseSchemaInfo
{
    public string Version { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string CurrentSchema { get; init; } = string.Empty;
    public string SessionUser { get; init; } = string.Empty;
    public string CurrentUser { get; init; } = string.Empty;
}

/// <summary>
/// Database connection options for multi-tenant scenarios
/// </summary>
public class DatabaseConnectionOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
}
