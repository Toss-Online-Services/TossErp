using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(command => command.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.");

        RuleFor(command => command.StoreId)
            .NotEmpty()
            .WithMessage("Store ID is required.");
    }
} 
