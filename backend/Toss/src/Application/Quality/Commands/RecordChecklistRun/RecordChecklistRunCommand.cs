using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Domain.Entities.Quality;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Commands.RecordChecklistRun;

public record RecordChecklistRunCommand : IRequest<int>
{
    public int ChecklistId { get; init; }
    public DateTimeOffset RunDate { get; init; }
    public List<ChecklistRunItemDto> Results { get; init; } = new();
    public string? Notes { get; init; }
}

public record ChecklistRunItemDto
{
    public int ChecklistItemId { get; init; }
    public bool Passed { get; init; }
    public string? Notes { get; init; }
}

public class RecordChecklistRunCommandHandler : IRequestHandler<RecordChecklistRunCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public RecordChecklistRunCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<int> Handle(RecordChecklistRunCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate checklist exists
        var checklist = await _context.QualityChecklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == request.ChecklistId
                && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (checklist == null)
        {
            throw new NotFoundException("QualityChecklist", request.ChecklistId.ToString());
        }

        if (!checklist.IsActive)
        {
            throw new ValidationException("Cannot run an inactive checklist.");
        }

        // Validate all required items are present
        var requiredItems = checklist.Items.Where(i => i.IsRequired).ToList();
        var providedItemIds = request.Results.Select(r => r.ChecklistItemId).ToHashSet();

        foreach (var requiredItem in requiredItems)
        {
            if (!providedItemIds.Contains(requiredItem.Id))
            {
                throw new ValidationException($"Required checklist item '{requiredItem.Title}' is missing.");
            }
        }

        // Validate all item IDs belong to this checklist
        var checklistItemIds = checklist.Items.Select(i => i.Id).ToHashSet();
        foreach (var result in request.Results)
        {
            if (!checklistItemIds.Contains(result.ChecklistItemId))
            {
                throw new ValidationException($"Checklist item ID {result.ChecklistItemId} does not belong to this checklist.");
            }
        }

        var run = new ChecklistRun
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            QualityChecklist = checklist,
            RunDate = request.RunDate,
            RunByUserId = _user.Id ?? string.Empty,
            Notes = request.Notes
        };

        // Add results
        foreach (var resultDto in request.Results)
        {
            var runItem = new ChecklistRunItem
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                ChecklistRun = run,
                ChecklistItemId = resultDto.ChecklistItemId,
                Passed = resultDto.Passed,
                Notes = resultDto.Notes
            };

            run.Results.Add(runItem);
        }

        _context.ChecklistRuns.Add(run);
        await _context.SaveChangesAsync(cancellationToken);

        return run.Id;
    }
}

