using FluentValidation;

namespace TossErp.Stock.Application.Commands.ReceiveStock;

/// <summary>
/// Validator for ReceiveStockCommand
/// </summary>
public class ReceiveStockCommandValidator : AbstractValidator<ReceiveStockCommand>
{
    public ReceiveStockCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters.");

        RuleFor(x => x.RefType)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.RefType))
            .WithMessage("Reference type cannot exceed 50 characters.");

        RuleFor(x => x.RefId)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.RefId))
            .WithMessage("Reference ID cannot exceed 100 characters.");

        RuleFor(x => x.Reason)
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Reason cannot exceed 200 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by is required.")
            .MaximumLength(100).WithMessage("Created by cannot exceed 100 characters.");

        // Business rule: If RefType is provided, RefId should also be provided
        RuleFor(x => x.RefId)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.RefType))
            .WithMessage("Reference ID is required when reference type is provided.");

        // Business rule: If RefId is provided, RefType should also be provided
        RuleFor(x => x.RefType)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.RefId))
            .WithMessage("Reference type is required when reference ID is provided.");
    }
}
