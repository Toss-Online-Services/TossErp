using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;

namespace TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Events;

// WarehouseAggregate Events
public record WarehouseCreatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseBasicInfoUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseGroupSetEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseParentUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseAccountUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseRejectedSetEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseTypeUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseInTransitUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseContactInfoUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseContactUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseAddressUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseTreeStructureUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseSettingsUpdatedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseDisabledEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseEnabledEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseDeletedEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseRestoredEvent(WarehouseAggregate Warehouse) : IDomainEvent;
public record WarehouseChildAddedEvent(WarehouseAggregate Warehouse, WarehouseAggregate ChildWarehouse) : IDomainEvent;
public record WarehouseChildRemovedEvent(WarehouseAggregate Warehouse, WarehouseAggregate ChildWarehouse) : IDomainEvent;

// Bin Events
public record BinAddedEvent(WarehouseAggregate Warehouse, Bin Bin) : IDomainEvent;
public record BinRemovedEvent(WarehouseAggregate Warehouse, Bin Bin) : IDomainEvent;
public record WarehouseBinAddedEvent(WarehouseAggregate Warehouse, Bin Bin) : IDomainEvent;
public record WarehouseBinRemovedEvent(WarehouseAggregate Warehouse, Bin Bin) : IDomainEvent;
public record BinCreatedEvent(Bin Bin) : IDomainEvent;
public record BinUpdatedEvent(Bin Bin) : IDomainEvent;
public record BinDeletedEvent(Bin Bin) : IDomainEvent; 
