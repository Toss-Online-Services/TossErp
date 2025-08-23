using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Infrastructure.Database.Performance;

/// <summary>
/// Background service for automatic database performance optimization
/// </summary>
public class DatabaseOptimizationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseOptimizationService> _logger;
    private readonly DatabaseOptimizationOptions _options;

    public DatabaseOptimizationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseOptimizationService> logger,
        IOptions<DatabaseOptimizationOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Database optimization service is disabled");
            return;
        }

        _logger.LogInformation("Database optimization service started with interval: {Interval}", _options.OptimizationInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOptimizationCycleAsync(stoppingToken);
                await Task.Delay(_options.OptimizationInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Database optimization service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database optimization cycle");
                
                // Wait before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task RunOptimizationCycleAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database optimization cycle...");

        using var scope = _serviceProvider.CreateScope();
        var performanceMonitor = scope.ServiceProvider.GetRequiredService<IDatabasePerformanceMonitor>();

        // Analyze current performance
        var analysis = await performanceMonitor.AnalyzePerformanceAsync(cancellationToken);
        
        _logger.LogInformation("Performance analysis completed. Score: {Score}/100", analysis.OverallScore);

        // Run optimization if score is below threshold
        if (analysis.OverallScore < _options.PerformanceThreshold)
        {
            _logger.LogInformation("Performance score {Score} is below threshold {Threshold}. Running optimization...", 
                analysis.OverallScore, _options.PerformanceThreshold);

            await performanceMonitor.OptimizePerformanceAsync(cancellationToken);
            
            _logger.LogInformation("Database optimization completed");
        }
        else
        {
            _logger.LogInformation("Performance score {Score} is above threshold. Skipping optimization.", analysis.OverallScore);
        }

        // Log recommendations if any
        if (analysis.IndexRecommendations.Count > 0)
        {
            _logger.LogInformation("Found {Count} index recommendations", analysis.IndexRecommendations.Count);
            
            foreach (var recommendation in analysis.IndexRecommendations)
            {
                _logger.LogInformation("Index recommendation for {Table}: {Description}", 
                    recommendation.TableName, recommendation.Description);
            }
        }

        // Log slow queries if any
        if (analysis.SlowQueries.Count > 0)
        {
            _logger.LogWarning("Found {Count} slow queries", analysis.SlowQueries.Count);
            
            foreach (var slowQuery in analysis.SlowQueries.Take(3)) // Log top 3
            {
                _logger.LogWarning("Slow query (avg: {AvgTime}ms, calls: {Calls}): {Query}", 
                    slowQuery.MeanExecutionTimeMs, slowQuery.Calls, 
                    slowQuery.Query.Length > 100 ? slowQuery.Query[..100] + "..." : slowQuery.Query);
            }
        }
    }
}

/// <summary>
/// Configuration options for database optimization service
/// </summary>
public class DatabaseOptimizationOptions
{
    public const string SectionName = "DatabaseOptimization";

    /// <summary>
    /// Whether the optimization service is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval between optimization cycles
    /// </summary>
    public TimeSpan OptimizationInterval { get; set; } = TimeSpan.FromHours(6);

    /// <summary>
    /// Performance score threshold below which optimization runs
    /// </summary>
    public int PerformanceThreshold { get; set; } = 80;

    /// <summary>
    /// Whether to run optimization during business hours
    /// </summary>
    public bool OptimizeDuringBusinessHours { get; set; } = false;

    /// <summary>
    /// Business hours start (24-hour format)
    /// </summary>
    public int BusinessHoursStart { get; set; } = 8;

    /// <summary>
    /// Business hours end (24-hour format)
    /// </summary>
    public int BusinessHoursEnd { get; set; } = 18;

    /// <summary>
    /// Time zone for business hours
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}

/// <summary>
/// Service collection extensions for performance monitoring
/// </summary>
public static class PerformanceServiceExtensions
{
    /// <summary>
    /// Adds database performance monitoring services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabasePerformanceMonitoring(
        this IServiceCollection services,
        Action<DatabaseOptimizationOptions>? configureOptions = null)
    {
        // Configure options
        var optionsBuilder = services.AddOptions<DatabaseOptimizationOptions>()
            .BindConfiguration(DatabaseOptimizationOptions.SectionName);

        if (configureOptions != null)
        {
            optionsBuilder.Configure(configureOptions);
        }

        // Register services
        services.AddScoped<IDatabasePerformanceMonitor, DatabasePerformanceMonitor>();
        services.AddHostedService<DatabaseOptimizationService>();

        return services;
    }

    /// <summary>
    /// Adds database performance monitoring with specific connection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabasePerformanceMonitoring(
        this IServiceCollection services,
        string connectionString,
        Action<DatabaseOptimizationOptions>? configureOptions = null)
    {
        // Register connection factory
        services.AddScoped<Npgsql.NpgsqlConnection>(_ => new Npgsql.NpgsqlConnection(connectionString));
        
        return services.AddDatabasePerformanceMonitoring(configureOptions);
    }
}
