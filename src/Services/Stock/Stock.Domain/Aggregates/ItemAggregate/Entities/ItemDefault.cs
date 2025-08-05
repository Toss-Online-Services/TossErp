using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemDefault : BaseEntity
{
    public ItemAggregate Item { get; private set; } = null!;
    public string Company { get; private set; } = string.Empty;
    public string? DefaultWarehouse { get; private set; }
    public string? DefaultSupplier { get; private set; }
    public string? DefaultPriceList { get; private set; }
    public string? DefaultExpenseAccount { get; private set; }
    public string? DefaultIncomeAccount { get; private set; }
    public string? DefaultCostCenter { get; private set; }
    public string? DefaultSalesTax { get; private set; }
    public string? DefaultPurchaseTax { get; private set; }
    public string? DefaultUOM { get; private set; }
    public string? DefaultPurchaseUOM { get; private set; }
    public string? DefaultSalesUOM { get; private set; }
    public decimal? MinOrderQty { get; private set; }
    public decimal? SafetyStock { get; private set; }
    public int? LeadTimeDays { get; private set; }
    public decimal? LastPurchaseRate { get; private set; }
    public decimal? StandardRate { get; private set; }
    public decimal? ValuationRate { get; private set; }
    public string? ValuationMethod { get; private set; }
    public bool AllowNegativeStock { get; private set; }
    public bool IsPurchaseItem { get; private set; }
    public bool IsSalesItem { get; private set; }
    public bool IncludeInManufacturing { get; private set; }
    public bool IsSubContractedItem { get; private set; }
    public string? DefaultBOM { get; private set; }
    public bool InspectionRequiredBeforePurchase { get; private set; }
    public bool InspectionRequiredBeforeDelivery { get; private set; }
    public string? QualityInspectionTemplate { get; private set; }
    public bool IsCustomerProvidedItem { get; private set; }
    public string? Customer { get; private set; }
    public string? CountryOfOrigin { get; private set; }
    public string? CustomsTariffNumber { get; private set; }
    public bool EnableDeferredExpense { get; private set; }
    public int? NoOfMonthsExp { get; private set; }
    public bool EnableDeferredRevenue { get; private set; }
    public int? NoOfMonths { get; private set; }
    public decimal? OverDeliveryReceiptAllowance { get; private set; }
    public decimal? OverBillingAllowance { get; private set; }
    public decimal? WarrantyPeriod { get; private set; }
    public decimal? RetainSample { get; private set; }
    public decimal? SampleQuantity { get; private set; }
    public string? CustomerCode { get; private set; }
    public string? DefaultItemManufacturer { get; private set; }
    public string? DefaultManufacturerPartNo { get; private set; }
    public decimal? TotalProjectedQty { get; private set; }

    private ItemDefault() { } // For EF Core

    public ItemDefault(ItemAggregate item, string company)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Company = company ?? throw new ArgumentNullException(nameof(company));
        AllowNegativeStock = false;
        IsPurchaseItem = true;
        IsSalesItem = true;
        IncludeInManufacturing = true;
        InspectionRequiredBeforePurchase = false;
        InspectionRequiredBeforeDelivery = false;
        IsCustomerProvidedItem = false;
        EnableDeferredExpense = false;
        EnableDeferredRevenue = false;

        AddDomainEvent(new ItemDefaultCreatedEvent(this));
    }

    public void UpdateWarehouseSettings(string? defaultWarehouse)
    {
        DefaultWarehouse = defaultWarehouse;
        AddDomainEvent(new ItemDefaultWarehouseUpdatedEvent(this));
    }

    public void UpdateSupplierSettings(string? defaultSupplier, int? leadTimeDays)
    {
        DefaultSupplier = defaultSupplier;
        LeadTimeDays = leadTimeDays;
        AddDomainEvent(new ItemDefaultSupplierUpdatedEvent(this));
    }

    public void UpdatePricingSettings(
        string? defaultPriceList,
        decimal? lastPurchaseRate,
        decimal? standardRate,
        decimal? valuationRate,
        string? valuationMethod)
    {
        DefaultPriceList = defaultPriceList;
        LastPurchaseRate = lastPurchaseRate;
        StandardRate = standardRate;
        ValuationRate = valuationRate;
        ValuationMethod = valuationMethod;
        AddDomainEvent(new ItemDefaultPricingUpdatedEvent(this));
    }

    public void UpdateAccountSettings(
        string? defaultExpenseAccount,
        string? defaultIncomeAccount,
        string? defaultCostCenter)
    {
        DefaultExpenseAccount = defaultExpenseAccount;
        DefaultIncomeAccount = defaultIncomeAccount;
        DefaultCostCenter = defaultCostCenter;
        AddDomainEvent(new ItemDefaultAccountUpdatedEvent(this));
    }

    public void UpdateTaxSettings(string? defaultSalesTax, string? defaultPurchaseTax)
    {
        DefaultSalesTax = defaultSalesTax;
        DefaultPurchaseTax = defaultPurchaseTax;
        AddDomainEvent(new ItemDefaultTaxUpdatedEvent(this));
    }

    public void UpdateUOMSettings(string? defaultUOM, string? defaultPurchaseUOM, string? defaultSalesUOM)
    {
        DefaultUOM = defaultUOM;
        DefaultPurchaseUOM = defaultPurchaseUOM;
        DefaultSalesUOM = defaultSalesUOM;
        AddDomainEvent(new ItemDefaultUOMUpdatedEvent(this));
    }

    public void UpdateInventorySettings(
        decimal? minOrderQty,
        decimal? safetyStock,
        bool allowNegativeStock)
    {
        MinOrderQty = minOrderQty;
        SafetyStock = safetyStock;
        AllowNegativeStock = allowNegativeStock;
        AddDomainEvent(new ItemDefaultInventoryUpdatedEvent(this));
    }

    public void UpdateSalesSettings(bool isSalesItem)
    {
        IsSalesItem = isSalesItem;
        AddDomainEvent(new ItemDefaultSalesUpdatedEvent(this));
    }

    public void UpdatePurchaseSettings(bool isPurchaseItem)
    {
        IsPurchaseItem = isPurchaseItem;
        AddDomainEvent(new ItemDefaultPurchaseUpdatedEvent(this));
    }

    public void UpdateManufacturingSettings(
        bool includeInManufacturing,
        bool isSubContractedItem,
        string? defaultBOM)
    {
        IncludeInManufacturing = includeInManufacturing;
        IsSubContractedItem = isSubContractedItem;
        DefaultBOM = defaultBOM;
        AddDomainEvent(new ItemDefaultManufacturingUpdatedEvent(this));
    }

    public void UpdateQualitySettings(
        bool inspectionRequiredBeforePurchase,
        bool inspectionRequiredBeforeDelivery,
        string? qualityInspectionTemplate)
    {
        InspectionRequiredBeforePurchase = inspectionRequiredBeforePurchase;
        InspectionRequiredBeforeDelivery = inspectionRequiredBeforeDelivery;
        QualityInspectionTemplate = qualityInspectionTemplate;
        AddDomainEvent(new ItemDefaultQualityUpdatedEvent(this));
    }

    public void UpdateCustomerSettings(bool isCustomerProvidedItem, string? customer, string? customerCode)
    {
        IsCustomerProvidedItem = isCustomerProvidedItem;
        Customer = customer;
        CustomerCode = customerCode;
        AddDomainEvent(new ItemDefaultCustomerUpdatedEvent(this));
    }

    public void UpdateInternationalSettings(string? countryOfOrigin, string? customsTariffNumber)
    {
        CountryOfOrigin = countryOfOrigin;
        CustomsTariffNumber = customsTariffNumber;
        AddDomainEvent(new ItemDefaultInternationalUpdatedEvent(this));
    }

    public void UpdateDeferredSettings(
        bool enableDeferredExpense,
        int? noOfMonthsExp,
        bool enableDeferredRevenue,
        int? noOfMonths)
    {
        EnableDeferredExpense = enableDeferredExpense;
        NoOfMonthsExp = noOfMonthsExp;
        EnableDeferredRevenue = enableDeferredRevenue;
        NoOfMonths = noOfMonths;
        AddDomainEvent(new ItemDefaultDeferredUpdatedEvent(this));
    }

    public void UpdateAllowanceSettings(
        decimal? overDeliveryReceiptAllowance,
        decimal? overBillingAllowance)
    {
        OverDeliveryReceiptAllowance = overDeliveryReceiptAllowance;
        OverBillingAllowance = overBillingAllowance;
        AddDomainEvent(new ItemDefaultAllowanceUpdatedEvent(this));
    }

    public void UpdateWarrantySettings(decimal? warrantyPeriod, decimal? retainSample, decimal? sampleQuantity)
    {
        WarrantyPeriod = warrantyPeriod;
        RetainSample = retainSample;
        SampleQuantity = sampleQuantity;
        AddDomainEvent(new ItemDefaultWarrantyUpdatedEvent(this));
    }

    public void UpdateManufacturerSettings(string? defaultItemManufacturer, string? defaultManufacturerPartNo)
    {
        DefaultItemManufacturer = defaultItemManufacturer;
        DefaultManufacturerPartNo = defaultManufacturerPartNo;
        AddDomainEvent(new ItemDefaultManufacturerUpdatedEvent(this));
    }

    public void UpdateProjectedQty(decimal? totalProjectedQty)
    {
        TotalProjectedQty = totalProjectedQty;
        AddDomainEvent(new ItemDefaultProjectedQtyUpdatedEvent(this));
    }

    public bool IsAvailableForSale()
    {
        return IsSalesItem;
    }

    public bool IsAvailableForPurchase()
    {
        return IsPurchaseItem;
    }

    public bool IsAvailableForManufacturing()
    {
        return IncludeInManufacturing;
    }

    public bool AllowsNegativeStock()
    {
        return AllowNegativeStock;
    }

    public bool RequiresInspectionBeforePurchase()
    {
        return InspectionRequiredBeforePurchase;
    }

    public bool RequiresInspectionBeforeDelivery()
    {
        return InspectionRequiredBeforeDelivery;
    }

    public bool IsCustomerProvided()
    {
        return IsCustomerProvidedItem;
    }

    public bool HasDeferredExpense()
    {
        return EnableDeferredExpense;
    }

    public bool HasDeferredRevenue()
    {
        return EnableDeferredRevenue;
    }
} 
