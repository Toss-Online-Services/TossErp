using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;

// ItemAggregate Events
public record ItemCreatedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemBasicInfoUpdatedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemPricingUpdatedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemInventorySettingsUpdatedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemValuationMethodUpdatedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemDisabledEvent(ItemAggregate Item) : IDomainEvent;
public record ItemEnabledEvent(ItemAggregate Item) : IDomainEvent;
public record ItemDeletedEvent(ItemAggregate Item) : IDomainEvent;
public record ItemRestoredEvent(ItemAggregate Item) : IDomainEvent;

// ItemVariant Events
public record ItemVariantCreatedEvent(ItemVariant ItemVariant) : IDomainEvent;
public record ItemVariantUpdatedEvent(ItemVariant ItemVariant) : IDomainEvent;
public record ItemVariantDeletedEvent(ItemVariant ItemVariant) : IDomainEvent;
public record ItemVariantAddedEvent(ItemAggregate Item, ItemVariant ItemVariant) : IDomainEvent;
public record ItemVariantRemovedEvent(ItemAggregate Item, ItemVariant ItemVariant) : IDomainEvent;

// ItemVariantAttribute Events
public record ItemVariantAttributeCreatedEvent(ItemVariantAttribute ItemVariantAttribute) : IDomainEvent;
public record ItemVariantAttributeValueUpdatedEvent(ItemVariantAttribute ItemVariantAttribute) : IDomainEvent;
public record ItemVariantAttributeDisabledEvent(ItemVariantAttribute ItemVariantAttribute) : IDomainEvent;
public record ItemVariantAttributeEnabledEvent(ItemVariantAttribute ItemVariantAttribute) : IDomainEvent;

// ItemPrice Events
public record ItemPriceCreatedEvent(ItemPrice ItemPrice) : IDomainEvent;
public record ItemPriceUpdatedEvent(ItemPrice ItemPrice) : IDomainEvent;
public record ItemPriceDeletedEvent(ItemPrice ItemPrice) : IDomainEvent;
public record ItemPriceAddedEvent(ItemAggregate Item, ItemPrice ItemPrice) : IDomainEvent;
public record ItemPriceRemovedEvent(ItemAggregate Item, ItemPrice ItemPrice) : IDomainEvent;

// ItemSupplier Events
public record ItemSupplierCreatedEvent(ItemSupplier ItemSupplier) : IDomainEvent;
public record ItemSupplierUpdatedEvent(ItemSupplier ItemSupplier) : IDomainEvent;
public record ItemSupplierDeletedEvent(ItemSupplier ItemSupplier) : IDomainEvent;
public record ItemSupplierAddedEvent(ItemAggregate Item, ItemSupplier ItemSupplier) : IDomainEvent;
public record ItemSupplierRemovedEvent(ItemAggregate Item, ItemSupplier ItemSupplier) : IDomainEvent;

// ItemBarcode Events
public record ItemBarcodeCreatedEvent(ItemBarcode ItemBarcode) : IDomainEvent;
public record ItemBarcodeUpdatedEvent(ItemBarcode ItemBarcode) : IDomainEvent;
public record ItemBarcodeDefaultSetEvent(ItemBarcode ItemBarcode) : IDomainEvent;
public record ItemBarcodeDeactivatedEvent(ItemBarcode ItemBarcode) : IDomainEvent;
public record ItemBarcodeActivatedEvent(ItemBarcode ItemBarcode) : IDomainEvent;
public record ItemBarcodeDeletedEvent(ItemBarcode ItemBarcode) : IDomainEvent;

// ItemTax Events
public record ItemTaxCreatedEvent(ItemTax ItemTax) : IDomainEvent;
public record ItemTaxUpdatedEvent(ItemTax ItemTax) : IDomainEvent;
public record ItemTaxDeactivatedEvent(ItemTax ItemTax) : IDomainEvent;
public record ItemTaxActivatedEvent(ItemTax ItemTax) : IDomainEvent;
public record ItemTaxDeletedEvent(ItemTax ItemTax) : IDomainEvent;

// ItemReorder Events
public record ItemReorderCreatedEvent(ItemReorder ItemReorder) : IDomainEvent;
public record ItemReorderUpdatedEvent(ItemReorder ItemReorder) : IDomainEvent;
public record ItemReorderDeactivatedEvent(ItemReorder ItemReorder) : IDomainEvent;
public record ItemReorderActivatedEvent(ItemReorder ItemReorder) : IDomainEvent;
public record ItemReorderDeletedEvent(ItemReorder ItemReorder) : IDomainEvent;

// ItemAttribute Events
public record ItemAttributeCreatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeUpdatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeTypeUpdatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeDeactivatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeActivatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeDeletedEvent(ItemAttribute ItemAttribute) : IDomainEvent;

// ItemAlternative Events
public record ItemAlternativeCreatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeUpdatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativePreferredSetEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeDeactivatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeActivatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeDeletedEvent(ItemAlternative ItemAlternative) : IDomainEvent;

// ItemManufacturer Events
public record ItemManufacturerCreatedEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;
public record ItemManufacturerUpdatedEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;
public record ItemManufacturerDefaultSetEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;
public record ItemManufacturerDeactivatedEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;
public record ItemManufacturerActivatedEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;
public record ItemManufacturerDeletedEvent(ItemManufacturer ItemManufacturer) : IDomainEvent;

// UOMConversionDetail Events
public record UOMConversionDetailCreatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailUpdatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailDeactivatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailActivatedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;
public record UOMConversionDetailDeletedEvent(UOMConversionDetail UOMConversionDetail) : IDomainEvent;

// ItemDefault Events
public record ItemDefaultCreatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultDeactivatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultActivatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultDeletedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultProjectedQtyUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultWarehouseUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultSupplierUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultPricingUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultAccountUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultTaxUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultUOMUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultInventorySettingsUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultManufacturerUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultWarrantyUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultInventoryUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultSalesUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultPurchaseUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultManufacturingUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultQualityUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultAllowanceUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultCustomerUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultDeferredUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent;
public record ItemDefaultInternationalUpdatedEvent(ItemDefault ItemDefault) : IDomainEvent; 
