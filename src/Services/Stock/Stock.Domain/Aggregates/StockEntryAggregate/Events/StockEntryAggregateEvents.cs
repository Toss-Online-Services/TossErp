using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;

namespace TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Events;

// StockEntryAggregate Events
public record StockEntryCreatedEvent(StockEntryAggregate StockEntry) : IDomainEvent;
public record StockEntryUpdatedEvent(StockEntryAggregate StockEntry) : IDomainEvent;
public record StockEntryPostedEvent(StockEntryAggregate StockEntry) : IDomainEvent;
public record StockEntryDeletedEvent(StockEntryAggregate StockEntry) : IDomainEvent;

// StockEntryDetail Events
public record StockEntryDetailAddedEvent(StockEntryAggregate StockEntry, StockEntryDetail Detail) : IDomainEvent;
public record StockEntryDetailRemovedEvent(StockEntryAggregate StockEntry, StockEntryDetail Detail) : IDomainEvent;

// StockEntryAdditionalCost Events
public record StockEntryAdditionalCostAddedEvent(StockEntryAggregate StockEntry, StockEntryAdditionalCost AdditionalCost) : IDomainEvent;
public record StockEntryAdditionalCostRemovedEvent(StockEntryAggregate StockEntry, StockEntryAdditionalCost AdditionalCost) : IDomainEvent; 
