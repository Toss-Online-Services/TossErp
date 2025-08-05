using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record StockLedgerEntryCreatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryVoucherDetailUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntrySerialBatchUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryAccountSettingsUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryCompanySettingsUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryUOMSettingsUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryReferenceDocumentUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryRemarksUpdatedEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryCancelledEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryDisabledEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent;
public record StockLedgerEntryEnabledEvent(StockLedgerEntry StockLedgerEntry) : IDomainEvent; 
