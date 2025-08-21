using FluentValidation;
using Financial.Application.DTOs;

namespace Financial.Application.Validators;

/// <summary>
/// Validator for insurance policy creation
/// </summary>
public class CreateInsurancePolicyValidator : AbstractValidator<CreateInsurancePolicyDto>
{
    public CreateInsurancePolicyValidator()
    {
        RuleFor(x => x.PolicyholderName)
            .NotEmpty().WithMessage("Policyholder name is required")
            .MaximumLength(200).WithMessage("Policyholder name cannot exceed 200 characters");

        RuleFor(x => x.PolicyholderContact)
            .NotEmpty().WithMessage("Policyholder contact is required")
            .MaximumLength(100).WithMessage("Policyholder contact cannot exceed 100 characters");

        RuleFor(x => x.InsuranceType)
            .IsInEnum().WithMessage("Valid insurance type is required");

        RuleFor(x => x.CoverageAmount)
            .GreaterThan(0).WithMessage("Coverage amount must be greater than zero")
            .LessThanOrEqualTo(10000000).WithMessage("Coverage amount cannot exceed $10,000,000");

        RuleFor(x => x.PremiumAmount)
            .GreaterThan(0).WithMessage("Premium amount must be greater than zero")
            .LessThanOrEqualTo(100000).WithMessage("Premium amount cannot exceed $100,000");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.BeneficiaryName)
            .NotEmpty().WithMessage("Beneficiary name is required")
            .MaximumLength(200).WithMessage("Beneficiary name cannot exceed 200 characters");

        RuleFor(x => x.BeneficiaryRelationship)
            .NotEmpty().WithMessage("Beneficiary relationship is required")
            .MaximumLength(50).WithMessage("Beneficiary relationship cannot exceed 50 characters");

        RuleFor(x => x.Terms)
            .MaximumLength(2000).WithMessage("Terms cannot exceed 2000 characters");
    }
}

/// <summary>
/// Validator for insurance claim creation
/// </summary>
public class CreateInsuranceClaimValidator : AbstractValidator<CreateInsuranceClaimDto>
{
    public CreateInsuranceClaimValidator()
    {
        RuleFor(x => x.PolicyId)
            .NotEmpty().WithMessage("Policy ID is required");

        RuleFor(x => x.ClaimAmount)
            .GreaterThan(0).WithMessage("Claim amount must be greater than zero");

        RuleFor(x => x.IncidentDate)
            .NotEmpty().WithMessage("Incident date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Incident date cannot be in the future");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Claim description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.DocumentationPath)
            .MaximumLength(500).WithMessage("Documentation path cannot exceed 500 characters");
    }
}

/// <summary>
/// Validator for policy payment
/// </summary>
public class PolicyPaymentValidator : AbstractValidator<PolicyPaymentDto>
{
    public PolicyPaymentValidator()
    {
        RuleFor(x => x.PolicyId)
            .NotEmpty().WithMessage("Policy ID is required");

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
