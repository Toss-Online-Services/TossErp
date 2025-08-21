using FluentValidation;
using Financial.Application.DTOs;

namespace Financial.Application.Validators;

/// <summary>
/// Validator for loan application creation
/// </summary>
public class CreateLoanApplicationValidator : AbstractValidator<CreateLoanApplicationDto>
{
    public CreateLoanApplicationValidator()
    {
        RuleFor(x => x.BorrowerName)
            .NotEmpty().WithMessage("Borrower name is required")
            .MaximumLength(200).WithMessage("Borrower name cannot exceed 200 characters");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required")
            .MaximumLength(200).WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.BusinessType)
            .NotEmpty().WithMessage("Business type is required")
            .MaximumLength(100).WithMessage("Business type cannot exceed 100 characters");

        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("Requested amount must be greater than zero")
            .LessThanOrEqualTo(1000000).WithMessage("Requested amount cannot exceed $1,000,000");

        RuleFor(x => x.TermMonths)
            .GreaterThan(0).WithMessage("Term must be greater than zero months")
            .LessThanOrEqualTo(120).WithMessage("Term cannot exceed 120 months (10 years)");

        RuleFor(x => x.Purpose)
            .NotEmpty().WithMessage("Loan purpose is required")
            .MaximumLength(500).WithMessage("Purpose cannot exceed 500 characters");

        RuleFor(x => x.DebtToIncomeRatio)
            .GreaterThanOrEqualTo(0).WithMessage("Debt-to-income ratio cannot be negative")
            .LessThanOrEqualTo(10).WithMessage("Debt-to-income ratio seems unreasonably high");

        RuleFor(x => x.CollateralValue)
            .GreaterThanOrEqualTo(0).WithMessage("Collateral value cannot be negative")
            .When(x => x.HasCollateral);

        RuleFor(x => x.CollateralDescription)
            .NotEmpty().WithMessage("Collateral description is required when collateral is offered")
            .When(x => x.HasCollateral);

        RuleFor(x => x.GuarantorName)
            .NotEmpty().WithMessage("Guarantor name is required when guarantor is specified")
            .MaximumLength(200).WithMessage("Guarantor name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.GuarantorContact));

        RuleFor(x => x.GuarantorContact)
            .NotEmpty().WithMessage("Guarantor contact is required when guarantor is specified")
            .MaximumLength(100).WithMessage("Guarantor contact cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.GuarantorName));
    }
}

/// <summary>
/// Validator for recording loan payments
/// </summary>
public class RecordLoanPaymentValidator : AbstractValidator<RecordLoanPaymentDto>
{
    public RecordLoanPaymentValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero");

        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("Payment date is required")
            .LessThanOrEqualTo(DateTime.Today.AddDays(1)).WithMessage("Payment date cannot be in the future");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required")
            .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters");

        RuleFor(x => x.TransactionReference)
            .MaximumLength(100).WithMessage("Transaction reference cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
    }
}
