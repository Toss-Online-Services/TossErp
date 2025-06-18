using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(command => command.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(command => command.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required.");
    }
} 
