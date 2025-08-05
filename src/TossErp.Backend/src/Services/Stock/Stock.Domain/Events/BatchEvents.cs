using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record BatchCreatedEvent(Batch Batch) : IDomainEvent;
public record BatchExpiryDateUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchManufacturingDateUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchWarrantyExpiryDateUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchSupplierUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchReferenceDocumentUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchDescriptionUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchRemarksUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchTransferQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchConsumedQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchDispatchedQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchReturnedQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchScrappedQuantityUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchRetainSampleUpdatedEvent(Batch Batch) : IDomainEvent;
public record BatchDisabledEvent(Batch Batch) : IDomainEvent;
public record BatchEnabledEvent(Batch Batch) : IDomainEvent;
public record BatchStockLedgerEntryAddedEvent(Batch Batch, StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record BatchStockLedgerEntryRemovedEvent(Batch Batch, StockLedgerEntry StockLedgerEntry) : IDomainEvent; 
