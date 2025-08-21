using FluentValidation;

namespace Crm.Application.Commands;

public class RecordPurchaseCommandValidator : AbstractValidator<RecordPurchaseCommand>
{
    public RecordPurchaseCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Purchase amount must be greater than zero")
            .LessThanOrEqualTo(1000000).WithMessage("Purchase amount must be reasonable");
    }
}
