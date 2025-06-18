using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations;

public class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Category name is required and cannot exceed 100 characters.");
    }
} 
