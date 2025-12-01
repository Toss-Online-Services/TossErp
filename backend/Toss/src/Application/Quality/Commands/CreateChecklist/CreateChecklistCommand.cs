using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Quality;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Commands.CreateChecklist;

public record CreateChecklistCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<ChecklistItemDto> Items { get; init; } = new();
}

public record ChecklistItemDto
{
    public string Title { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int Order { get; init; }
}

public class CreateChecklistCommandHandler : IRequestHandler<CreateChecklistCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateChecklistCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateChecklistCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Checklist name is required.");
        }

        // Check for duplicate name
        var duplicateExists = await _context.QualityChecklists
            .AnyAsync(c => c.BusinessId == _businessContext.CurrentBusinessId!.Value
                && c.Name == request.Name, cancellationToken);

        if (duplicateExists)
        {
            throw new ValidationException("A checklist with this name already exists.");
        }

        var checklist = new QualityChecklist
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        // Add items
        foreach (var itemDto in request.Items.OrderBy(i => i.Order))
        {
            if (string.IsNullOrWhiteSpace(itemDto.Title))
            {
                throw new ValidationException("Checklist item title is required.");
            }

            var item = new ChecklistItem
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                QualityChecklist = checklist,
                Title = itemDto.Title,
                IsRequired = itemDto.IsRequired,
                Order = itemDto.Order
            };

            checklist.Items.Add(item);
        }

        _context.QualityChecklists.Add(checklist);
        await _context.SaveChangesAsync(cancellationToken);

        return checklist.Id;
    }
}

