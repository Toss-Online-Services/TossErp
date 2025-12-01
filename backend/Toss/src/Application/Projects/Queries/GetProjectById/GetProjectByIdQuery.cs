using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;
using TaskStatus = Toss.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery : IRequest<ProjectDetailDto?>
{
    public int Id { get; init; }
}

public record ProjectDetailDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public int? ShopId { get; init; }
    public string? ShopName { get; init; }
    public ProjectStatus Status { get; init; }
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? ExpectedCompletionDate { get; init; }
    public DateTimeOffset? CompletedDate { get; init; }
    public int? InvoiceId { get; init; }
    public string? InvoiceNumber { get; init; }
    public string? Notes { get; init; }
    public List<ProjectTaskDto> Tasks { get; init; } = new();
    public List<ProjectMaterialDto> Materials { get; init; } = new();
    public List<LabourEntryDto> LabourEntries { get; init; } = new();
    public decimal TotalMaterialCost { get; init; }
    public decimal TotalLabourCost { get; init; }
    public decimal TotalCost { get; init; }
    public DateTimeOffset Created { get; init; }
}

public record ProjectTaskDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskStatus Status { get; init; }
    public string? AssigneeId { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public DateTimeOffset? CompletedDate { get; init; }
    public decimal? EstimatedHours { get; init; }
    public decimal? ActualHours { get; init; }
}

public record ProjectMaterialDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public decimal TotalCost { get; init; }
    public string? Notes { get; init; }
}

public record LabourEntryDto
{
    public int Id { get; init; }
    public string? UserId { get; init; }
    public DateTimeOffset WorkDate { get; init; }
    public decimal Hours { get; init; }
    public decimal Rate { get; init; }
    public decimal TotalCost { get; init; }
    public string? Description { get; init; }
    public int? ProjectTaskId { get; init; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetProjectByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<ProjectDetailDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var project = await _context.Projects
            .Include(p => p.Customer)
            .Include(p => p.Shop)
            .Include(p => p.Invoice)
            .Include(p => p.Tasks)
            .Include(p => p.Materials)
                .ThenInclude(m => m.Product)
            .Include(p => p.LabourEntries)
            .Where(p => p.Id == request.Id
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (project == null)
        {
            return null;
        }

        return new ProjectDetailDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            CustomerId = project.CustomerId,
            CustomerName = project.Customer != null ? project.Customer.FullName : null,
            ShopId = project.ShopId,
            ShopName = project.Shop != null ? project.Shop.Name : null,
            Status = project.Status,
            StartDate = project.StartDate,
            ExpectedCompletionDate = project.ExpectedCompletionDate,
            CompletedDate = project.CompletedDate,
            InvoiceId = project.InvoiceId,
            InvoiceNumber = project.Invoice != null ? project.Invoice.DocumentNumber : null,
            Notes = project.Notes,
            Tasks = project.Tasks.Select(t => new ProjectTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                AssigneeId = t.AssigneeId,
                DueDate = t.DueDate,
                CompletedDate = t.CompletedDate,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours
            }).ToList(),
            Materials = project.Materials.Select(m => new ProjectMaterialDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                ProductName = m.Product.Name,
                Quantity = m.Quantity,
                UnitCost = m.UnitCost,
                TotalCost = m.TotalCost,
                Notes = m.Notes
            }).ToList(),
            LabourEntries = project.LabourEntries.Select(l => new LabourEntryDto
            {
                Id = l.Id,
                UserId = l.UserId,
                WorkDate = l.WorkDate,
                Hours = l.Hours,
                Rate = l.Rate,
                TotalCost = l.TotalCost,
                Description = l.Description,
                ProjectTaskId = l.ProjectTaskId
            }).ToList(),
            TotalMaterialCost = project.Materials.Sum(m => m.TotalCost),
            TotalLabourCost = project.LabourEntries.Sum(l => l.TotalCost),
            TotalCost = project.Materials.Sum(m => m.TotalCost) + project.LabourEntries.Sum(l => l.TotalCost),
            Created = project.Created
        };
    }
}

