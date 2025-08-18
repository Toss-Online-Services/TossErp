using FluentValidation;
using TossErp.Sales.Application.Queries.GetDailySales;

namespace TossErp.Sales.Application.Queries.GetDailySales;

/// <summary>
/// Validator for GetDailySalesQuery
/// </summary>
public class GetDailySalesQueryValidator : AbstractValidator<GetDailySalesQuery>
{
    public GetDailySalesQueryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.Today.AddDays(1))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateTime.Today.AddYears(-1))
            .WithMessage("Date cannot be more than 1 year in the past");
    }
}
