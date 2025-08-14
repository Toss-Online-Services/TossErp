using FluentValidation;

namespace TossErp.Stock.Application.Commands.TransferStock;

/// <summary>
/// Validator for TransferStockCommand
/// </summary>
public class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
{
    public TransferStockCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.");

        RuleFor(x => x.FromWarehouseId)
            .NotEmpty().WithMessage("Source warehouse ID is required.");

        RuleFor(x => x.ToWarehouseId)
            .NotEmpty().WithMessage("Destination warehouse ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters.");

        RuleFor(x => x.Reason)
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Reason cannot exceed 200 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by is required.")
            .MaximumLength(100).WithMessage("Created by cannot exceed 100 characters.");

        // Business rule: Source and destination warehouses must be different
        RuleFor(x => x)
            .Must(x => x.FromWarehouseId != x.ToWarehouseId)
            .WithMessage("Source and destination warehouses must be different.");

        // Business rule: Reason should be provided for transfers
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required for stock transfers.")
            .MinimumLength(10).When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Reason should be descriptive (minimum 10 characters) for audit purposes.");
    }
}
