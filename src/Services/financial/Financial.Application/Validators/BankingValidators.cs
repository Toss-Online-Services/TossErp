using FluentValidation;
using Financial.Application.DTOs;

namespace Financial.Application.Validators;

/// <summary>
/// Validator for linking bank accounts
/// </summary>
public class LinkBankAccountValidator : AbstractValidator<LinkBankAccountDto>
{
    public LinkBankAccountValidator()
    {
        RuleFor(x => x.AccountHolderName)
            .NotEmpty().WithMessage("Account holder name is required")
            .MaximumLength(200).WithMessage("Account holder name cannot exceed 200 characters");

        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Bank name is required")
            .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required")
            .MaximumLength(50).WithMessage("Account number cannot exceed 50 characters")
            .Matches(@"^[0-9]+$").WithMessage("Account number must contain only digits");

        RuleFor(x => x.RoutingNumber)
            .NotEmpty().WithMessage("Routing number is required")
            .Length(9).WithMessage("Routing number must be exactly 9 digits")
            .Matches(@"^[0-9]+$").WithMessage("Routing number must contain only digits");

        RuleFor(x => x.AccountType)
            .NotEmpty().WithMessage("Account type is required")
            .MaximumLength(20).WithMessage("Account type cannot exceed 20 characters")
            .Must(x => new[] { "checking", "savings", "business" }.Contains(x?.ToLower()))
            .WithMessage("Account type must be 'checking', 'savings', or 'business'");

        RuleFor(x => x.MicroDepositAmount1)
            .GreaterThan(0).WithMessage("First micro deposit amount must be greater than zero")
            .LessThanOrEqualTo(1).WithMessage("Micro deposit amount cannot exceed $1.00")
            .When(x => x.VerificationMethod == "micro-deposits");

        RuleFor(x => x.MicroDepositAmount2)
            .GreaterThan(0).WithMessage("Second micro deposit amount must be greater than zero")
            .LessThanOrEqualTo(1).WithMessage("Micro deposit amount cannot exceed $1.00")
            .NotEqual(x => x.MicroDepositAmount1).WithMessage("Micro deposit amounts must be different")
            .When(x => x.VerificationMethod == "micro-deposits");

        RuleFor(x => x.VerificationMethod)
            .NotEmpty().WithMessage("Verification method is required")
            .Must(x => new[] { "micro-deposits", "instant", "manual" }.Contains(x?.ToLower()))
            .WithMessage("Verification method must be 'micro-deposits', 'instant', or 'manual'");
    }
}

/// <summary>
/// Validator for recording bank transactions
/// </summary>
public class RecordBankTransactionValidator : AbstractValidator<RecordBankTransactionDto>
{
    public RecordBankTransactionValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("Transaction amount cannot be zero");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Transaction date is required")
            .LessThanOrEqualTo(DateTime.Today.AddDays(1)).WithMessage("Transaction date cannot be in the future");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Transaction description is required")
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Transaction category is required")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters");

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("Reference cannot exceed 100 characters");
    }
}

/// <summary>
/// Validator for bank account reconciliation
/// </summary>
public class ReconcileBankAccountValidator : AbstractValidator<ReconcileBankAccountDto>
{
    public ReconcileBankAccountValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.StatementDate)
            .NotEmpty().WithMessage("Statement date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Statement date cannot be in the future");

        RuleFor(x => x.StatementBalance)
            .NotEqual(0).WithMessage("Statement balance is required");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

        RuleFor(x => x.ReconciliationItems)
            .NotNull().WithMessage("Reconciliation items are required")
            .Must(x => x?.Any() == true).WithMessage("At least one reconciliation item is required");

        RuleForEach(x => x.ReconciliationItems)
            .SetValidator(new ReconciliationItemValidator());
    }
}

/// <summary>
/// Validator for individual reconciliation items
/// </summary>
public class ReconciliationItemValidator : AbstractValidator<ReconciliationItemDto>
{
    public ReconciliationItemValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required");

        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("Amount cannot be zero");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");

        RuleFor(x => x.IsCleared)
            .NotNull().WithMessage("Cleared status is required");
    }
}
