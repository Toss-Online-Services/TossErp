using FluentValidation;
using POS.API.Application.Commands;

namespace POS.API.Application.Validations
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category ID is required");
        }
    }
} 
