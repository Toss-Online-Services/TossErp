using FluentValidation;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.Items.Commands.CreateItem;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required.")
            .MaximumLength(50).WithMessage("Item code cannot exceed 50 characters.");

        RuleFor(x => x.ItemName)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.ItemGroup)
            .NotEmpty().WithMessage("Item group is required.")
            .MaximumLength(100).WithMessage("Item group cannot exceed 100 characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters.");

        RuleFor(x => x.ItemType)
            .NotEmpty().WithMessage("Item type is required.")
            .Must(BeValidItemType).WithMessage("Invalid item type.");

        RuleFor(x => x.StockUOM)
            .NotEmpty().WithMessage("Stock UOM is required.")
            .MaximumLength(20).WithMessage("Stock UOM cannot exceed 20 characters.");

        RuleFor(x => x.ValuationMethod)
            .NotEmpty().WithMessage("Valuation method is required.")
            .Must(BeValidValuationMethod).WithMessage("Invalid valuation method.");

        RuleFor(x => x.StandardRate)
            .GreaterThanOrEqualTo(0).WithMessage("Standard rate must be greater than or equal to 0.");

        RuleFor(x => x.WeightPerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("Weight per unit must be greater than or equal to 0.");

        RuleFor(x => x.WeightUOM)
            .GreaterThanOrEqualTo(0).WithMessage("Weight UOM must be greater than or equal to 0.");

        RuleFor(x => x.ReOrderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Re-order level must be greater than or equal to 0.");

        RuleFor(x => x.ReOrderQty)
            .GreaterThanOrEqualTo(0).WithMessage("Re-order quantity must be greater than or equal to 0.");

        RuleFor(x => x.MaxQty)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum quantity must be greater than or equal to 0.");

        RuleFor(x => x.MinQty)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity must be greater than or equal to 0.");

        // Validate variants
        RuleForEach(x => x.Variants).SetValidator(new CreateItemVariantCommandValidator());

        // Validate prices
        RuleForEach(x => x.Prices).SetValidator(new CreateItemPriceCommandValidator());

        // Validate suppliers
        RuleForEach(x => x.Suppliers).SetValidator(new CreateItemSupplierCommandValidator());

        // Validate customers
        RuleForEach(x => x.Customers).SetValidator(new CreateItemCustomerCommandValidator());

        // Validate barcodes
        RuleForEach(x => x.Barcodes).SetValidator(new CreateItemBarcodeCommandValidator());

        // Validate taxes
        RuleForEach(x => x.Taxes).SetValidator(new CreateItemTaxCommandValidator());

        // Validate reorder levels
        RuleForEach(x => x.ReorderLevels).SetValidator(new CreateItemReorderCommandValidator());

        // Validate attributes
        RuleForEach(x => x.Attributes).SetValidator(new CreateItemAttributeCommandValidator());

        // Validate alternatives
        RuleForEach(x => x.Alternatives).SetValidator(new CreateItemAlternativeCommandValidator());

        // Validate manufacturers
        RuleForEach(x => x.Manufacturers).SetValidator(new CreateItemManufacturerCommandValidator());

        // Validate website specifications
        RuleForEach(x => x.WebsiteSpecifications).SetValidator(new CreateItemWebsiteSpecificationCommandValidator());

        // Validate quality inspection parameters
        RuleForEach(x => x.QualityInspectionParameters).SetValidator(new CreateItemQualityInspectionParameterCommandValidator());

        // Validate UOM conversions
        RuleForEach(x => x.UOMConversions).SetValidator(new CreateUOMConversionDetailCommandValidator());
    }

    private static bool BeValidItemType(string itemType)
    {
        return Enum.TryParse<ItemType>(itemType, out _);
    }

    private static bool BeValidValuationMethod(string valuationMethod)
    {
        return Enum.TryParse<ValuationMethod>(valuationMethod, out _);
    }
}

public class CreateItemVariantCommandValidator : AbstractValidator<CreateItemVariantCommand>
{
    public CreateItemVariantCommandValidator()
    {
        RuleFor(x => x.VariantName)
            .NotEmpty().WithMessage("Variant name is required.")
            .MaximumLength(100).WithMessage("Variant name cannot exceed 100 characters.");

        RuleFor(x => x.VariantCode)
            .NotEmpty().WithMessage("Variant code is required.")
            .MaximumLength(50).WithMessage("Variant code cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleForEach(x => x.Attributes).SetValidator(new CreateItemVariantAttributeCommandValidator());
    }
}

public class CreateItemVariantAttributeCommandValidator : AbstractValidator<CreateItemVariantAttributeCommand>
{
    public CreateItemVariantAttributeCommandValidator()
    {
        RuleFor(x => x.Attribute)
            .NotEmpty().WithMessage("Attribute is required.")
            .MaximumLength(100).WithMessage("Attribute cannot exceed 100 characters.");

        RuleFor(x => x.AttributeValue)
            .NotEmpty().WithMessage("Attribute value is required.")
            .MaximumLength(200).WithMessage("Attribute value cannot exceed 200 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0.");
    }
}

public class CreateItemPriceCommandValidator : AbstractValidator<CreateItemPriceCommand>
{
    public CreateItemPriceCommandValidator()
    {
        RuleFor(x => x.PriceList)
            .NotEmpty().WithMessage("Price list is required.")
            .MaximumLength(100).WithMessage("Price list cannot exceed 100 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .MaximumLength(10).WithMessage("Currency cannot exceed 10 characters.");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Rate must be greater than or equal to 0.");

        RuleFor(x => x.MinimumQty)
            .GreaterThanOrEqualTo(0).When(x => x.MinimumQty.HasValue);

        RuleFor(x => x.MaximumQty)
            .GreaterThanOrEqualTo(0).When(x => x.MaximumQty.HasValue)
            .WithMessage("Maximum quantity must be greater than or equal to 0.");
    }
}

public class CreateItemSupplierCommandValidator : AbstractValidator<CreateItemSupplierCommand>
{
    public CreateItemSupplierCommandValidator()
    {
        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Supplier is required.")
            .MaximumLength(100).WithMessage("Supplier cannot exceed 100 characters.");

        RuleFor(x => x.SupplierPartNo)
            .MaximumLength(100).WithMessage("Supplier part number cannot exceed 100 characters.");

        RuleFor(x => x.MinimumQty)
            .GreaterThanOrEqualTo(0).When(x => x.MinimumQty.HasValue)
            .WithMessage("Minimum quantity must be greater than or equal to 0.");

        RuleFor(x => x.MaximumQty)
            .GreaterThanOrEqualTo(0).When(x => x.MaximumQty.HasValue)
            .WithMessage("Maximum quantity must be greater than or equal to 0.");

        RuleFor(x => x.LeadTimeDays)
            .GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue)
            .WithMessage("Lead time days must be greater than or equal to 0.");
    }
}

public class CreateItemCustomerCommandValidator : AbstractValidator<CreateItemCustomerCommand>
{
    public CreateItemCustomerCommandValidator()
    {
        RuleFor(x => x.Customer)
            .NotEmpty().WithMessage("Customer is required.")
            .MaximumLength(100).WithMessage("Customer cannot exceed 100 characters.");

        RuleFor(x => x.CustomerItemCode)
            .MaximumLength(100).WithMessage("Customer item code cannot exceed 100 characters.");

        RuleFor(x => x.CustomerItemName)
            .MaximumLength(200).WithMessage("Customer item name cannot exceed 200 characters.");

        RuleFor(x => x.RefRate)
            .GreaterThanOrEqualTo(0).When(x => x.RefRate.HasValue)
            .WithMessage("Reference rate must be greater than or equal to 0.");

        RuleFor(x => x.RefCurrency)
            .MaximumLength(10).When(x => !string.IsNullOrEmpty(x.RefCurrency))
            .WithMessage("Reference currency cannot exceed 10 characters.");
    }
}

public class CreateItemBarcodeCommandValidator : AbstractValidator<CreateItemBarcodeCommand>
{
    public CreateItemBarcodeCommandValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode is required.")
            .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters.");

        RuleFor(x => x.BarcodeType)
            .NotEmpty().WithMessage("Barcode type is required.")
            .MaximumLength(50).WithMessage("Barcode type cannot exceed 50 characters.");
    }
}

public class CreateItemTaxCommandValidator : AbstractValidator<CreateItemTaxCommand>
{
    public CreateItemTaxCommandValidator()
    {
        RuleFor(x => x.TaxType)
            .NotEmpty().WithMessage("Tax type is required.")
            .MaximumLength(50).WithMessage("Tax type cannot exceed 50 characters.");

        RuleFor(x => x.TaxCategory)
            .NotEmpty().WithMessage("Tax category is required.")
            .MaximumLength(50).WithMessage("Tax category cannot exceed 50 characters.");

        RuleFor(x => x.TaxCode)
            .NotEmpty().WithMessage("Tax code is required.")
            .MaximumLength(50).WithMessage("Tax code cannot exceed 50 characters.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate must be greater than or equal to 0.");
    }
}

public class CreateItemReorderCommandValidator : AbstractValidator<CreateItemReorderCommand>
{
    public CreateItemReorderCommandValidator()
    {
        RuleFor(x => x.Warehouse)
            .NotEmpty().WithMessage("Warehouse is required.")
            .MaximumLength(100).WithMessage("Warehouse cannot exceed 100 characters.");

        RuleFor(x => x.ReOrderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Re-order level must be greater than or equal to 0.");

        RuleFor(x => x.ReOrderQty)
            .GreaterThanOrEqualTo(0).WithMessage("Re-order quantity must be greater than or equal to 0.");

        RuleFor(x => x.MinQty)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity must be greater than or equal to 0.");

        RuleFor(x => x.MaxQty)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum quantity must be greater than or equal to 0.");
    }
}

public class CreateItemAttributeCommandValidator : AbstractValidator<CreateItemAttributeCommand>
{
    public CreateItemAttributeCommandValidator()
    {
        RuleFor(x => x.Attribute)
            .NotEmpty().WithMessage("Attribute is required.")
            .MaximumLength(100).WithMessage("Attribute cannot exceed 100 characters.");

        RuleFor(x => x.AttributeValue)
            .NotEmpty().WithMessage("Attribute value is required.")
            .MaximumLength(200).WithMessage("Attribute value cannot exceed 200 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0.");
    }
}

public class CreateItemAlternativeCommandValidator : AbstractValidator<CreateItemAlternativeCommand>
{
    public CreateItemAlternativeCommandValidator()
    {
        RuleFor(x => x.AlternativeItem)
            .NotEmpty().WithMessage("Alternative item is required.")
            .MaximumLength(100).WithMessage("Alternative item cannot exceed 100 characters.");

        RuleFor(x => x.ConversionFactor)
            .GreaterThan(0).WithMessage("Conversion factor must be greater than 0.");
    }
}

public class CreateItemManufacturerCommandValidator : AbstractValidator<CreateItemManufacturerCommand>
{
    public CreateItemManufacturerCommandValidator()
    {
        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.")
            .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters.");

        RuleFor(x => x.ManufacturerPartNo)
            .MaximumLength(100).WithMessage("Manufacturer part number cannot exceed 100 characters.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Comment))
            .WithMessage("Comment cannot exceed 500 characters.");
    }
}

public class CreateItemWebsiteSpecificationCommandValidator : AbstractValidator<CreateItemWebsiteSpecificationCommand>
{
    public CreateItemWebsiteSpecificationCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Label is required.")
            .MaximumLength(100).WithMessage("Label cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0.");
    }
}

public class CreateItemQualityInspectionParameterCommandValidator : AbstractValidator<CreateItemQualityInspectionParameterCommand>
{
    public CreateItemQualityInspectionParameterCommandValidator()
    {
        RuleFor(x => x.Parameter)
            .NotEmpty().WithMessage("Parameter is required.")
            .MaximumLength(100).WithMessage("Parameter cannot exceed 100 characters.");

        RuleFor(x => x.Specification)
            .MaximumLength(500).WithMessage("Specification cannot exceed 500 characters.");

        RuleFor(x => x.AcceptanceCriteria)
            .MaximumLength(500).WithMessage("Acceptance criteria cannot exceed 500 characters.");
    }
}

public class CreateUOMConversionDetailCommandValidator : AbstractValidator<CreateUOMConversionDetailCommand>
{
    public CreateUOMConversionDetailCommandValidator()
    {
        RuleFor(x => x.UOM)
            .NotEmpty().WithMessage("UOM is required.")
            .MaximumLength(20).WithMessage("UOM cannot exceed 20 characters.");

        RuleFor(x => x.ConversionFactor)
            .GreaterThan(0).WithMessage("Conversion factor must be greater than 0.");
    }
} 
