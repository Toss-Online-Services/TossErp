using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;

namespace TossErp.Stock.Domain.Events;

public record StockEntryTypeCreatedEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeUpdatedEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypePurposeUpdatedEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeTransitUpdatedEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeNegativeStockUpdatedEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeDisabledEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeEnabledEvent(StockEntryType StockEntryType) : IDomainEvent;
public record StockEntryTypeStockEntryAddedEvent(StockEntryType StockEntryType, StockEntryAggregate StockEntry) : IDomainEvent;
public record StockEntryTypeStockEntryRemovedEvent(StockEntryType StockEntryType, StockEntryAggregate StockEntry) : IDomainEvent; 
