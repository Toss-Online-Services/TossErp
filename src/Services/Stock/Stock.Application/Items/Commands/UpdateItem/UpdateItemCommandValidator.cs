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

        RuleFor(x => x.ItemType)
            .Must(BeValidItemType).When(x => !string.IsNullOrWhiteSpace(x.ItemType))
            .WithMessage("Invalid item type.");

        RuleFor(x => x.StandardRate)
            .GreaterThanOrEqualTo(0).When(x => x.StandardRate.HasValue)
            .WithMessage("Standard rate must be greater than or equal to 0.");

        RuleFor(x => x.MinimumPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinimumPrice.HasValue)
            .WithMessage("Minimum price must be greater than or equal to 0.");

        RuleFor(x => x.WeightPerUnit)
            .GreaterThanOrEqualTo(0).When(x => x.WeightPerUnit.HasValue)
            .WithMessage("Weight per unit must be greater than or equal to 0.");

        RuleFor(x => x.Length)
            .GreaterThanOrEqualTo(0).When(x => x.Length.HasValue)
            .WithMessage("Length must be greater than or equal to 0.");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(0).When(x => x.Width.HasValue)
            .WithMessage("Width must be greater than or equal to 0.");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(0).When(x => x.Height.HasValue)
            .WithMessage("Height must be greater than or equal to 0.");

        // Business rule: Minimum price should not exceed standard rate when both are provided
        RuleFor(x => x)
            .Must(x => !x.MinimumPrice.HasValue || !x.StandardRate.HasValue || x.MinimumPrice.Value <= x.StandardRate.Value)
            .When(x => x.MinimumPrice.HasValue && x.StandardRate.HasValue)
            .WithMessage("Minimum price cannot exceed standard rate.");
    }

    private static bool BeValidItemType(string? itemType)
    {
        return string.IsNullOrWhiteSpace(itemType) || Enum.TryParse<ItemType>(itemType, true, out _);
    }
}
