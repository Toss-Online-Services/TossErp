using FluentValidation;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.Items.Commands.UpdateItem;

/// <summary>
/// Validator for UpdateItemCommand
/// </summary>
public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item ID is required.");

        RuleFor(x => x.ItemName)
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.ItemName))
            .WithMessage("Item name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.ItemGroup)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ItemGroup))
            .WithMessage("Item group cannot exceed 100 characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Brand))
            .WithMessage("Brand cannot exceed 100 characters.");

        RuleFor(x => x.Unit)
            .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Unit))
            .WithMessage("Unit cannot exceed 20 characters.");

        RuleFor(x => x.ItemType)
            .Must(BeValidItemType).When(x => !string.IsNullOrWhiteSpace(x.ItemType))
            .WithMessage("Invalid item type.");

        RuleFor(x => x.SellingPrice)
            .GreaterThanOrEqualTo(0).When(x => x.SellingPrice.HasValue)
            .WithMessage("Selling price must be greater than or equal to 0.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).When(x => x.CostPrice.HasValue)
            .WithMessage("Cost price must be greater than or equal to 0.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).When(x => x.ReorderLevel.HasValue)
            .WithMessage("Reorder level must be greater than or equal to 0.");

        RuleFor(x => x.ReorderQty)
            .GreaterThanOrEqualTo(0).When(x => x.ReorderQty.HasValue)
            .WithMessage("Reorder quantity must be greater than or equal to 0.");

        RuleFor(x => x.MaxStock)
            .GreaterThanOrEqualTo(0).When(x => x.MaxStock.HasValue)
            .WithMessage("Maximum stock must be greater than or equal to 0.");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue)
            .WithMessage("Weight must be greater than or equal to 0.");

        RuleFor(x => x.Length)
            .GreaterThanOrEqualTo(0).When(x => x.Length.HasValue)
            .WithMessage("Length must be greater than or equal to 0.");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(0).When(x => x.Width.HasValue)
            .WithMessage("Width must be greater than or equal to 0.");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(0).When(x => x.Height.HasValue)
            .WithMessage("Height must be greater than or equal to 0.");

        // Business rule: Max stock should be greater than reorder level when both are provided
        RuleFor(x => x)
            .Must(x => !x.MaxStock.HasValue || !x.ReorderLevel.HasValue || x.MaxStock.Value > x.ReorderLevel.Value)
            .When(x => x.MaxStock.HasValue && x.ReorderLevel.HasValue)
            .WithMessage("Maximum stock must be greater than reorder level.");

        // Business rule: Reorder quantity should be positive when provided
        RuleFor(x => x.ReorderQty)
            .GreaterThan(0).When(x => x.ReorderQty.HasValue)
            .WithMessage("Reorder quantity must be greater than 0.");
    }

    private static bool BeValidItemType(string itemType)
    {
        return Enum.TryParse<ItemType>(itemType, true, out _);
    }
}
