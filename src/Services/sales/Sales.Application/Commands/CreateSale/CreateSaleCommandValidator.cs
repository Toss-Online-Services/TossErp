using FluentValidation;
using TossErp.Sales.Application.Commands.CreateSale;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Commands.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.TillId)
            .NotEmpty()
            .WithMessage("Till ID is required")
            .WithErrorCode("SALES-1502");

        RuleFor(x => x.CustomerName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerName))
            .WithMessage("Customer name cannot exceed 200 characters")
            .WithErrorCode("SALES-1501");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required")
            .WithErrorCode("SALES-1103");

        RuleFor(x => x.Items)
            .Must(items => items.Count <= 100)
            .WithMessage("Cannot add more than 100 items to a single sale")
            .WithErrorCode("SALES-1503");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemRequestValidator());

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DiscountAmount.HasValue)
            .WithMessage("Discount amount cannot be negative")
            .WithErrorCode("SALES-1107");

        RuleFor(x => x.DiscountReason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.DiscountReason))
            .WithMessage("Discount reason cannot exceed 500 characters")
            .WithErrorCode("SALES-1500");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 1000 characters")
            .WithErrorCode("SALES-1500");

        // Business rule: Total discount cannot exceed 50% of subtotal
        RuleFor(x => x)
            .Must(ValidateDiscountLimit)
            .WithMessage("Total discount cannot exceed 50% of the sale subtotal")
            .WithErrorCode("SALES-1107");
    }

    private static bool ValidateDiscountLimit(CreateSaleCommand command)
    {
        if (!command.DiscountAmount.HasValue || command.DiscountAmount.Value <= 0)
            return true;

        var subtotal = command.Items.Sum(item => item.Quantity * item.UnitPrice);
        var maxDiscount = subtotal * 0.5m;

        return command.DiscountAmount.Value <= maxDiscount;
    }
}

/// <summary>
/// Validator for CreateSaleItemRequest
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    public CreateSaleItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required")
            .WithErrorCode("SALES-1200");

        RuleFor(x => x.ItemName)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Item name is required and cannot exceed 200 characters")
            .WithErrorCode("SALES-1200");

        RuleFor(x => x.ItemSku)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Item SKU is required and cannot exceed 50 characters")
            .WithErrorCode("SALES-1200");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000)
            .WithMessage("Quantity must be between 1 and 10,000")
            .WithErrorCode("SALES-1201");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1000000)
            .WithMessage("Unit price must be between 0 and 1,000,000")
            .WithErrorCode("SALES-1202");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1)
            .WithMessage("Tax rate must be between 0 and 1 (0% to 100%)")
            .WithErrorCode("SALES-1203");
    }
}
