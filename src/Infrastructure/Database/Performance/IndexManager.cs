using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TossErp.Infrastructure.Database.Performance;

/// <summary>
/// Utility for managing database indexes with performance optimization
/// </summary>
public static class IndexManager
{
    /// <summary>
    /// Creates a performance-optimized index with standard TOSS patterns
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="name">Index name</param>
    /// <param name="table">Table name</param>
    /// <param name="columns">Columns to index</param>
    /// <param name="options">Index options</param>
    public static void CreateOptimizedIndex(
        MigrationBuilder migrationBuilder,
        string name,
        string table,
        string[] columns,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();

        var indexBuilder = migrationBuilder.CreateIndex(name, table, columns, options.Unique);

        // Apply performance optimizations
        if (options.FillFactor.HasValue)
        {
            indexBuilder.Annotation("Npgsql:StorageParameter:fillfactor", options.FillFactor.Value.ToString());
        }

        if (options.IncludeColumns?.Length > 0)
        {
            indexBuilder.Annotation("Npgsql:IncludeProperties", options.IncludeColumns);
        }

        if (options.Where != null)
        {
            indexBuilder.HasFilter(options.Where);
        }

        if (options.Method.HasValue)
        {
            indexBuilder.Annotation("Npgsql:IndexMethod", options.Method.Value.ToString().ToLowerInvariant());
        }

        if (options.Concurrently)
        {
            // Note: EF Core doesn't directly support CONCURRENTLY, but we can use raw SQL
            var concurrentSql = GenerateConcurrentIndexSql(name, table, columns, options);
            migrationBuilder.Sql(concurrentSql);
            
            // Remove the standard index creation since we're using raw SQL
            migrationBuilder.DropIndex(name, table);
        }
    }

    /// <summary>
    /// Creates a tenant-aware index with automatic tenant filtering
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="businessColumns">Business logic columns</param>
    /// <param name="options">Index options</param>
    public static void CreateTenantAwareIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string[] businessColumns,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();

        // Always include TenantId as the first column for tenant isolation
        var columns = new[] { "TenantId" }.Concat(businessColumns).ToArray();
        var indexName = $"IX_{table}_TenantId_{string.Join("_", businessColumns)}";

        CreateOptimizedIndex(migrationBuilder, indexName, table, columns, options);
    }

    /// <summary>
    /// Creates a covering index for read-heavy workloads
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="keyColumns">Key columns for the index</param>
    /// <param name="includeColumns">Columns to include (covering)</param>
    /// <param name="options">Index options</param>
    public static void CreateCoveringIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string[] keyColumns,
        string[] includeColumns,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();
        options.IncludeColumns = includeColumns;

        var indexName = $"IX_{table}_{string.Join("_", keyColumns)}_Covering";
        CreateOptimizedIndex(migrationBuilder, indexName, table, keyColumns, options);
    }

    /// <summary>
    /// Creates a partial index for efficient filtering
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="columns">Columns to index</param>
    /// <param name="whereClause">WHERE clause for partial index</param>
    /// <param name="options">Index options</param>
    public static void CreatePartialIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string[] columns,
        string whereClause,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();
        options.Where = whereClause;

        var indexName = $"IX_{table}_{string.Join("_", columns)}_Partial";
        CreateOptimizedIndex(migrationBuilder, indexName, table, columns, options);
    }

    /// <summary>
    /// Creates a GIN index for full-text search or array operations
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="column">Column to index</param>
    /// <param name="options">Index options</param>
    public static void CreateGinIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string column,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();
        options.Method = IndexMethod.Gin;

        var indexName = $"IX_{table}_{column}_GIN";
        CreateOptimizedIndex(migrationBuilder, indexName, table, new[] { column }, options);
    }

    /// <summary>
    /// Creates a GiST index for geometric or full-text search operations
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="column">Column to index</param>
    /// <param name="options">Index options</param>
    public static void CreateGistIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string column,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();
        options.Method = IndexMethod.Gist;

        var indexName = $"IX_{table}_{column}_GIST";
        CreateOptimizedIndex(migrationBuilder, indexName, table, new[] { column }, options);
    }

    /// <summary>
    /// Creates an expression index for computed values
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name</param>
    /// <param name="expression">Expression to index</param>
    /// <param name="indexName">Index name</param>
    /// <param name="options">Index options</param>
    public static void CreateExpressionIndex(
        MigrationBuilder migrationBuilder,
        string table,
        string expression,
        string indexName,
        IndexOptions? options = null)
    {
        options ??= new IndexOptions();

        var sql = new StringBuilder();
        sql.Append($"CREATE");
        
        if (options.Unique)
            sql.Append(" UNIQUE");
            
        sql.Append($" INDEX");
        
        if (options.Concurrently)
            sql.Append(" CONCURRENTLY");
            
        sql.Append($" \"{indexName}\" ON \"{table}\"");
        
        if (options.Method.HasValue)
            sql.Append($" USING {options.Method.Value.ToString().ToLowerInvariant()}");
            
        sql.Append($" ({expression})");
        
        if (options.IncludeColumns?.Length > 0)
            sql.Append($" INCLUDE ({string.Join(", ", options.IncludeColumns.Select(c => $"\"{c}\""))})");
            
        if (!string.IsNullOrEmpty(options.Where))
            sql.Append($" WHERE {options.Where}");

        // Add storage parameters
        var storageParams = new List<string>();
        if (options.FillFactor.HasValue)
            storageParams.Add($"fillfactor = {options.FillFactor.Value}");

        if (storageParams.Count > 0)
            sql.Append($" WITH ({string.Join(", ", storageParams)})");

        sql.Append(";");

        migrationBuilder.Sql(sql.ToString());
    }

    /// <summary>
    /// Drops an index safely with IF EXISTS
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="indexName">Index name</param>
    /// <param name="concurrently">Whether to drop concurrently</param>
    public static void DropIndexSafely(
        MigrationBuilder migrationBuilder,
        string indexName,
        bool concurrently = false)
    {
        var sql = $"DROP INDEX{(concurrently ? " CONCURRENTLY" : "")} IF EXISTS \"{indexName}\";";
        migrationBuilder.Sql(sql);
    }

    /// <summary>
    /// Analyzes index usage and provides recommendations
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="table">Table name to analyze</param>
    public static void AnalyzeIndexUsage(
        MigrationBuilder migrationBuilder,
        string table)
    {
        var sql = $@"
            -- Analyze index usage for table: {table}
            SELECT 
                schemaname,
                tablename,
                indexname,
                idx_scan as index_scans,
                idx_tup_read as tuples_read,
                idx_tup_fetch as tuples_fetched,
                pg_size_pretty(pg_relation_size(indexrelid)) as size
            FROM pg_stat_user_indexes 
            WHERE tablename = '{table}'
            ORDER BY idx_scan DESC;
        ";
        
        // This would typically be executed in a separate context for analysis
        // For migration purposes, we'll add it as a comment
        migrationBuilder.Sql($"-- Index usage analysis query for {table}:\n-- {sql.Replace("\n", "\n-- ")}");
    }

    private static string GenerateConcurrentIndexSql(
        string name, 
        string table, 
        string[] columns, 
        IndexOptions options)
    {
        var sql = new StringBuilder();
        sql.Append($"CREATE");
        
        if (options.Unique)
            sql.Append(" UNIQUE");
            
        sql.Append($" INDEX CONCURRENTLY \"{name}\" ON \"{table}\"");
        
        if (options.Method.HasValue)
            sql.Append($" USING {options.Method.Value.ToString().ToLowerInvariant()}");
            
        sql.Append($" ({string.Join(", ", columns.Select(c => $"\"{c}\""))})");
        
        if (options.IncludeColumns?.Length > 0)
            sql.Append($" INCLUDE ({string.Join(", ", options.IncludeColumns.Select(c => $"\"{c}\""))})");
            
        if (!string.IsNullOrEmpty(options.Where))
            sql.Append($" WHERE {options.Where}");

        // Add storage parameters
        var storageParams = new List<string>();
        if (options.FillFactor.HasValue)
            storageParams.Add($"fillfactor = {options.FillFactor.Value}");

        if (storageParams.Count > 0)
            sql.Append($" WITH ({string.Join(", ", storageParams)})");

        sql.Append(";");

        return sql.ToString();
    }
}

/// <summary>
/// Options for index creation
/// </summary>
public class IndexOptions
{
    /// <summary>
    /// Whether the index should be unique
    /// </summary>
    public bool Unique { get; set; } = false;

    /// <summary>
    /// Fill factor for the index (10-100)
    /// </summary>
    public int? FillFactor { get; set; } = 90;

    /// <summary>
    /// Columns to include in the index (covering index)
    /// </summary>
    public string[]? IncludeColumns { get; set; }

    /// <summary>
    /// WHERE clause for partial indexes
    /// </summary>
    public string? Where { get; set; }

    /// <summary>
    /// Index method (B-tree, Hash, GiST, SP-GiST, GIN, BRIN)
    /// </summary>
    public IndexMethod? Method { get; set; }

    /// <summary>
    /// Whether to create the index concurrently
    /// </summary>
    public bool Concurrently { get; set; } = false;
}

/// <summary>
/// PostgreSQL index methods
/// </summary>
public enum IndexMethod
{
    Btree,
    Hash,
    Gist,
    Spgist,
    Gin,
    Brin
}
