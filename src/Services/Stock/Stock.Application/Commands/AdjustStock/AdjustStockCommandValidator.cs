using FluentValidation;

namespace TossErp.Stock.Application.Commands.AdjustStock;

/// <summary>
/// Validator for AdjustStockCommand
/// </summary>
public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse ID is required.");

        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("Quantity cannot be zero for stock adjustments.")
            .WithMessage("Quantity must be non-zero for stock adjustments.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required for stock adjustments.")
            .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by is required.")
            .MaximumLength(100).WithMessage("Created by cannot exceed 100 characters.");

        // Business rule: Reason should be descriptive for audit purposes
        RuleFor(x => x.Reason)
            .MinimumLength(10).WithMessage("Reason should be descriptive (minimum 10 characters) for audit purposes.");
    }
}
