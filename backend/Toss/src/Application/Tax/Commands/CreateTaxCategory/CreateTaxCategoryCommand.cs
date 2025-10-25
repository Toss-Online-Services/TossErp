using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Tax;

namespace Toss.Application.Tax.Commands.CreateTaxCategory;

public record CreateTaxCategoryCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

public class CreateTaxCategoryCommandHandler : IRequestHandler<CreateTaxCategoryCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTaxCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTaxCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new TaxCategory
        {
            Name = request.Name,
            DisplayOrder = request.DisplayOrder
        };

        _context.TaxCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}

