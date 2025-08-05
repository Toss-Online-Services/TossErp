using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Application database context interface
/// Defines the contract for database access in the Stock bounded context
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Items database set
    /// </summary>
    DbSet<ItemAggregate> Items { get; }

    /// <summary>
    /// Stock entries database set
    /// </summary>
    DbSet<StockEntryAggregate> StockEntries { get; }

    /// <summary>
    /// Warehouses database set
    /// </summary>
    DbSet<WarehouseAggregate> Warehouses { get; }

    /// <summary>
    /// Batches database set
    /// </summary>
    DbSet<Batch> Batches { get; }

    /// <summary>
    /// Serial numbers database set
    /// </summary>
    DbSet<SerialNo> SerialNumbers { get; }

    /// <summary>
    /// Stock ledger entries database set
    /// </summary>
    DbSet<StockLedgerEntry> StockLedgerEntries { get; }

    /// <summary>
    /// Stock entry types database set
    /// </summary>
    DbSet<StockEntryType> StockEntryTypes { get; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 
