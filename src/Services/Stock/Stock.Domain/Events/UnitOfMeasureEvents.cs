using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record UnitOfMeasureCreatedEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureUpdatedEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureDeactivatedEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureActivatedEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureDeletedEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureBaseUnitSetEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureCompoundUnitSetEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureCategorySetEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureWholeNumberSetEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureDisabledEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureEnabledEvent(UnitOfMeasure UnitOfMeasure) : IDomainEvent;
public record UnitOfMeasureConversionDetailAddedEvent(UnitOfMeasure UnitOfMeasure, UOMConversionDetail ConversionDetail) : IDomainEvent;
public record UnitOfMeasureConversionDetailRemovedEvent(UnitOfMeasure UnitOfMeasure, UOMConversionDetail ConversionDetail) : IDomainEvent;
public record UnitOfMeasureItemAddedEvent(UnitOfMeasure UnitOfMeasure, ItemAggregate Item) : IDomainEvent;
public record UnitOfMeasureItemRemovedEvent(UnitOfMeasure UnitOfMeasure, ItemAggregate Item) : IDomainEvent; 
