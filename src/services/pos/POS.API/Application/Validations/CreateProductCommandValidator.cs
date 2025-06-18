using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Product name is required and cannot exceed 200 characters.");

        RuleFor(command => command.SKU)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("SKU is required and cannot exceed 50 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(command => command.StoreId)
            .NotEmpty()
            .WithMessage("Store ID is required.");

        RuleFor(command => command.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required.");

        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .When(command => !string.IsNullOrEmpty(command.Description))
            .WithMessage("Description cannot exceed 1000 characters.");
    }
} 
