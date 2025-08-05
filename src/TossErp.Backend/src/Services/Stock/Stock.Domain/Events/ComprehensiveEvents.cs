using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;

namespace TossErp.Stock.Domain.Events;

// Bin Events
public record BinCreatedEvent(Bin Bin) : IDomainEvent;
public record BinUpdatedEvent(Bin Bin) : IDomainEvent;
public record BinDeletedEvent(Bin Bin) : IDomainEvent;

// UOMConversionDetail Events
public record UOMConversionDetailCreatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailUpdatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailDeactivatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailActivatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailDeletedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent; 
