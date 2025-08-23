using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Infrastructure.Database.Performance;

/// <summary>
/// Interface for database performance monitoring
/// </summary>
public interface IDatabasePerformanceMonitor
{
    /// <summary>
    /// Analyzes database performance and generates recommendations
    /// </summary>
    Task<PerformanceAnalysisResult> AnalyzePerformanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current database statistics
    /// </summary>
    Task<DatabaseStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies missing indexes
    /// </summary>
    Task<List<IndexRecommendation>> GetIndexRecommendationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets slow query analysis
    /// </summary>
    Task<List<SlowQueryAnalysis>> GetSlowQueriesAsync(int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes database performance automatically
    /// </summary>
    Task OptimizePerformanceAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Database performance monitoring service
/// </summary>
public class DatabasePerformanceMonitor : IDatabasePerformanceMonitor
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<DatabasePerformanceMonitor> _logger;

    public DatabasePerformanceMonitor(
        NpgsqlConnection connection,
        ILogger<DatabasePerformanceMonitor> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<PerformanceAnalysisResult> AnalyzePerformanceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database performance analysis...");

        var statistics = await GetStatisticsAsync(cancellationToken);
        var indexRecommendations = await GetIndexRecommendationsAsync(cancellationToken);
        var slowQueries = await GetSlowQueriesAsync(10, cancellationToken);

        var result = new PerformanceAnalysisResult
        {
            Statistics = statistics,
            IndexRecommendations = indexRecommendations,
            SlowQueries = slowQueries,
            AnalyzedAt = DateTimeOffset.UtcNow,
            OverallScore = CalculatePerformanceScore(statistics, indexRecommendations, slowQueries)
        };

        _logger.LogInformation("Performance analysis completed. Overall score: {Score}/100", result.OverallScore);
        
        return result;
    }

    public async Task<DatabaseStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureConnectionOpenAsync(cancellationToken);

        using var command = new NpgsqlCommand(@"
            SELECT 
                pg_database_size(current_database()) as database_size,
                (SELECT count(*) FROM pg_stat_activity WHERE state = 'active') as active_connections,
                (SELECT count(*) FROM pg_stat_activity) as total_connections,
                (SELECT sum(seq_scan) FROM pg_stat_user_tables) as sequential_scans,
                (SELECT sum(idx_scan) FROM pg_stat_user_tables) as index_scans,
                (SELECT sum(n_tup_ins) FROM pg_stat_user_tables) as total_inserts,
                (SELECT sum(n_tup_upd) FROM pg_stat_user_tables) as total_updates,
                (SELECT sum(n_tup_del) FROM pg_stat_user_tables) as total_deletes,
                (SELECT count(*) FROM pg_stat_user_tables) as table_count,
                (SELECT count(*) FROM pg_stat_user_indexes) as index_count,
                (SELECT extract(epoch from (now() - stats_reset)) FROM pg_stat_database WHERE datname = current_database()) as stats_age_seconds
        ", _connection);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        if (await reader.ReadAsync(cancellationToken))
        {
            return new DatabaseStatistics
            {
                DatabaseSizeBytes = reader.GetInt64("database_size"),
                ActiveConnections = reader.GetInt32("active_connections"),
                TotalConnections = reader.GetInt32("total_connections"),
                SequentialScans = reader.IsDBNull("sequential_scans") ? 0 : reader.GetInt64("sequential_scans"),
                IndexScans = reader.IsDBNull("index_scans") ? 0 : reader.GetInt64("index_scans"),
                TotalInserts = reader.IsDBNull("total_inserts") ? 0 : reader.GetInt64("total_inserts"),
                TotalUpdates = reader.IsDBNull("total_updates") ? 0 : reader.GetInt64("total_updates"),
                TotalDeletes = reader.IsDBNull("total_deletes") ? 0 : reader.GetInt64("total_deletes"),
                TableCount = reader.GetInt32("table_count"),
                IndexCount = reader.GetInt32("index_count"),
                StatsAgeSeconds = reader.IsDBNull("stats_age_seconds") ? 0 : reader.GetDouble("stats_age_seconds")
            };
        }

        throw new InvalidOperationException("Failed to retrieve database statistics");
    }

    public async Task<List<IndexRecommendation>> GetIndexRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureConnectionOpenAsync(cancellationToken);

        var recommendations = new List<IndexRecommendation>();

        // Check for tables with high sequential scan ratio
        using var command = new NpgsqlCommand(@"
            SELECT 
                schemaname,
                tablename,
                seq_scan,
                seq_tup_read,
                idx_scan,
                idx_tup_fetch,
                n_tup_ins + n_tup_upd + n_tup_del as total_writes,
                pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as table_size
            FROM pg_stat_user_tables 
            WHERE seq_scan > 0 
                AND (idx_scan = 0 OR seq_scan::float / GREATEST(idx_scan, 1) > 5)
                AND n_tup_ins + n_tup_upd + n_tup_del > 1000
            ORDER BY seq_scan DESC
            LIMIT 20
        ", _connection);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var tableName = $"{reader.GetString("schemaname")}.{reader.GetString("tablename")}";
            var seqScan = reader.GetInt64("seq_scan");
            var idxScan = reader.IsDBNull("idx_scan") ? 0 : reader.GetInt64("idx_scan");
            
            recommendations.Add(new IndexRecommendation
            {
                TableName = tableName,
                RecommendationType = IndexRecommendationType.MissingIndex,
                Severity = seqScan > 10000 ? RecommendationSeverity.High : RecommendationSeverity.Medium,
                Description = $"Table {tableName} has high sequential scan ratio ({seqScan} seq vs {idxScan} idx)",
                EstimatedImpact = EstimateIndexImpact(seqScan, idxScan),
                SuggestedIndexSql = await GenerateIndexSuggestionAsync(tableName, cancellationToken)
            });
        }

        // Check for unused indexes
        await CheckUnusedIndexesAsync(recommendations, cancellationToken);

        return recommendations;
    }

    public async Task<List<SlowQueryAnalysis>> GetSlowQueriesAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        await EnsureConnectionOpenAsync(cancellationToken);

        var slowQueries = new List<SlowQueryAnalysis>();

        using var command = new NpgsqlCommand(@"
            SELECT 
                query,
                calls,
                total_exec_time,
                mean_exec_time,
                max_exec_time,
                stddev_exec_time,
                rows,
                100.0 * shared_blks_hit / nullif(shared_blks_hit + shared_blks_read, 0) AS hit_percent
            FROM pg_stat_statements 
            WHERE calls > 10
            ORDER BY total_exec_time DESC 
            LIMIT @limit
        ", _connection);

        command.Parameters.AddWithValue("@limit", limit);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            slowQueries.Add(new SlowQueryAnalysis
            {
                Query = reader.GetString("query"),
                Calls = reader.GetInt64("calls"),
                TotalExecutionTimeMs = reader.GetDouble("total_exec_time"),
                MeanExecutionTimeMs = reader.GetDouble("mean_exec_time"),
                MaxExecutionTimeMs = reader.GetDouble("max_exec_time"),
                StandardDeviationMs = reader.GetDouble("stddev_exec_time"),
                TotalRows = reader.GetInt64("rows"),
                CacheHitPercent = reader.IsDBNull("hit_percent") ? 0 : reader.GetDouble("hit_percent")
            });
        }

        return slowQueries;
    }

    public async Task OptimizePerformanceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting automatic database optimization...");

        await EnsureConnectionOpenAsync(cancellationToken);

        // Update table statistics
        await UpdateStatisticsAsync(cancellationToken);

        // Reindex fragmented indexes
        await ReindexFragmentedIndexesAsync(cancellationToken);

        // Vacuum analyze tables
        await VacuumAnalyzeTablesAsync(cancellationToken);

        _logger.LogInformation("Database optimization completed");
    }

    private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }
    }

    private int CalculatePerformanceScore(
        DatabaseStatistics stats, 
        List<IndexRecommendation> indexRecs, 
        List<SlowQueryAnalysis> slowQueries)
    {
        int score = 100;

        // Penalize high sequential scan ratio
        if (stats.SequentialScans > 0 && stats.IndexScans > 0)
        {
            var seqScanRatio = (double)stats.SequentialScans / (stats.SequentialScans + stats.IndexScans);
            if (seqScanRatio > 0.3) score -= 20;
            else if (seqScanRatio > 0.1) score -= 10;
        }

        // Penalize high number of index recommendations
        if (indexRecs.Count > 5) score -= 15;
        else if (indexRecs.Count > 2) score -= 5;

        // Penalize slow queries
        var criticalSlowQueries = slowQueries.Count(q => q.MeanExecutionTimeMs > 1000);
        if (criticalSlowQueries > 3) score -= 20;
        else if (criticalSlowQueries > 1) score -= 10;

        // Penalize high connection usage
        if (stats.TotalConnections > 80) score -= 10;
        else if (stats.TotalConnections > 60) score -= 5;

        return Math.Max(0, score);
    }

    private IndexImpact EstimateIndexImpact(long seqScans, long idxScans)
    {
        if (seqScans > 50000) return IndexImpact.High;
        if (seqScans > 10000) return IndexImpact.Medium;
        return IndexImpact.Low;
    }

    private async Task<string> GenerateIndexSuggestionAsync(string tableName, CancellationToken cancellationToken)
    {
        // This would analyze the table structure and common query patterns
        // For now, return a generic suggestion
        return $"-- Consider adding indexes on frequently queried columns for {tableName}\n-- Analyze query patterns to determine specific columns";
    }

    private async Task CheckUnusedIndexesAsync(List<IndexRecommendation> recommendations, CancellationToken cancellationToken)
    {
        using var command = new NpgsqlCommand(@"
            SELECT 
                schemaname,
                tablename,
                indexname,
                idx_scan,
                pg_size_pretty(pg_relation_size(indexrelid)) as index_size
            FROM pg_stat_user_indexes 
            WHERE idx_scan = 0 
                AND indexname NOT LIKE '%_pkey'
                AND pg_relation_size(indexrelid) > 1048576  -- > 1MB
            ORDER BY pg_relation_size(indexrelid) DESC
        ", _connection);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var indexName = reader.GetString("indexname");
            var tableName = $"{reader.GetString("schemaname")}.{reader.GetString("tablename")}";
            
            recommendations.Add(new IndexRecommendation
            {
                TableName = tableName,
                IndexName = indexName,
                RecommendationType = IndexRecommendationType.UnusedIndex,
                Severity = RecommendationSeverity.Medium,
                Description = $"Index {indexName} on {tableName} is unused and consuming space",
                EstimatedImpact = IndexImpact.Low,
                SuggestedIndexSql = $"DROP INDEX IF EXISTS {indexName};"
            });
        }
    }

    private async Task UpdateStatisticsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating table statistics...");
        
        using var command = new NpgsqlCommand("ANALYZE;", _connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task ReindexFragmentedIndexesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking for fragmented indexes...");
        
        // Get fragmented indexes (this is a simplified check)
        using var command = new NpgsqlCommand(@"
            SELECT schemaname, tablename, indexname
            FROM pg_stat_user_indexes 
            WHERE idx_scan > 1000
            AND pg_relation_size(indexrelid) > 10485760  -- > 10MB
        ", _connection);

        var fragmentedIndexes = new List<string>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            fragmentedIndexes.Add(reader.GetString("indexname"));
        }

        // Reindex fragmented indexes
        foreach (var indexName in fragmentedIndexes)
        {
            try
            {
                _logger.LogInformation("Reindexing {IndexName}...", indexName);
                using var reindexCommand = new NpgsqlCommand($"REINDEX INDEX CONCURRENTLY {indexName};", _connection);
                await reindexCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to reindex {IndexName}", indexName);
            }
        }
    }

    private async Task VacuumAnalyzeTablesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running vacuum analyze on tables...");
        
        // Get tables that need vacuuming
        using var command = new NpgsqlCommand(@"
            SELECT schemaname, tablename
            FROM pg_stat_user_tables 
            WHERE n_dead_tup > greatest(n_live_tup * 0.1, 1000)
            OR (n_tup_ins + n_tup_upd + n_tup_del) > 10000
        ", _connection);

        var tablesToVacuum = new List<string>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            tablesToVacuum.Add($"{reader.GetString("schemaname")}.{reader.GetString("tablename")}");
        }

        // Vacuum analyze tables
        foreach (var tableName in tablesToVacuum)
        {
            try
            {
                _logger.LogInformation("Vacuum analyzing {TableName}...", tableName);
                using var vacuumCommand = new NpgsqlCommand($"VACUUM ANALYZE {tableName};", _connection);
                await vacuumCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to vacuum analyze {TableName}", tableName);
            }
        }
    }
}

// Supporting data models
public class PerformanceAnalysisResult
{
    public DatabaseStatistics Statistics { get; set; } = new();
    public List<IndexRecommendation> IndexRecommendations { get; set; } = new();
    public List<SlowQueryAnalysis> SlowQueries { get; set; } = new();
    public DateTimeOffset AnalyzedAt { get; set; }
    public int OverallScore { get; set; }
}

public class DatabaseStatistics
{
    public long DatabaseSizeBytes { get; set; }
    public int ActiveConnections { get; set; }
    public int TotalConnections { get; set; }
    public long SequentialScans { get; set; }
    public long IndexScans { get; set; }
    public long TotalInserts { get; set; }
    public long TotalUpdates { get; set; }
    public long TotalDeletes { get; set; }
    public int TableCount { get; set; }
    public int IndexCount { get; set; }
    public double StatsAgeSeconds { get; set; }
}

public class IndexRecommendation
{
    public string TableName { get; set; } = string.Empty;
    public string? IndexName { get; set; }
    public IndexRecommendationType RecommendationType { get; set; }
    public RecommendationSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public IndexImpact EstimatedImpact { get; set; }
    public string SuggestedIndexSql { get; set; } = string.Empty;
}

public class SlowQueryAnalysis
{
    public string Query { get; set; } = string.Empty;
    public long Calls { get; set; }
    public double TotalExecutionTimeMs { get; set; }
    public double MeanExecutionTimeMs { get; set; }
    public double MaxExecutionTimeMs { get; set; }
    public double StandardDeviationMs { get; set; }
    public long TotalRows { get; set; }
    public double CacheHitPercent { get; set; }
}

public enum IndexRecommendationType
{
    MissingIndex,
    UnusedIndex,
    DuplicateIndex,
    PartialIndex
}

public enum RecommendationSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum IndexImpact
{
    Low,
    Medium,
    High
}
