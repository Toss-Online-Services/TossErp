using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? CustomerId { get; init; }
    public int? ShopId { get; init; }
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? ExpectedCompletionDate { get; init; }
    public string? Notes { get; init; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateProjectCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Project title is required.");
        }

        // Validate customer exists if provided
        if (request.CustomerId.HasValue)
        {
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == request.CustomerId.Value
                    && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!customerExists)
            {
                throw new NotFoundException("Customer", request.CustomerId.Value.ToString());
            }
        }

        // Validate shop exists if provided
        if (request.ShopId.HasValue)
        {
            var shopExists = await _context.Stores
                .AnyAsync(s => s.Id == request.ShopId.Value
                    && s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!shopExists)
            {
                throw new NotFoundException("Store", request.ShopId.Value.ToString());
            }
        }

        var project = new Project
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Title = request.Title,
            Description = request.Description,
            CustomerId = request.CustomerId,
            ShopId = request.ShopId,
            Status = ProjectStatus.New,
            StartDate = request.StartDate,
            ExpectedCompletionDate = request.ExpectedCompletionDate,
            Notes = request.Notes
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}

