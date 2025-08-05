using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record StockEntryDetailCreatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailBasicInfoUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailQtyUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailRatesUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailWarehousesUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailSerialBatchUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailAccountSettingsUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailAdditionalCostUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailRemarksUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailFinishedGoodUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailProcessLossUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailBOMUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailWorkOrderUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailPurchaseUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailSalesUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailAssetUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailQualityUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailPutawayUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailZeroValuationRateUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailNegativeStockUpdatedEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailDisabledEvent(StockEntryDetail StockEntryDetail) : IDomainEvent;
public record StockEntryDetailEnabledEvent(StockEntryDetail StockEntryDetail) : IDomainEvent; 
