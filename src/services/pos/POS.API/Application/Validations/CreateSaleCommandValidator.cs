using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(command => command.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(command => command.StoreId)
            .NotEmpty()
            .WithMessage("Store ID is required.");

        RuleFor(command => command.TaxAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax amount must be zero or greater.");

        RuleFor(command => command.DiscountAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount amount must be zero or greater.");

        RuleFor(command => command.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required.");
    }
} 
