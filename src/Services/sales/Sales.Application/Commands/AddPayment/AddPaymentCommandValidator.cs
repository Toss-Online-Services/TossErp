using FluentValidation;

namespace TossErp.Sales.Application.Commands.AddPayment;

/// <summary>
/// Validator for AddPaymentCommand
/// </summary>
public class AddPaymentCommandValidator : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Payment method must be a valid value");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Reference cannot exceed 100 characters");
    }
}
