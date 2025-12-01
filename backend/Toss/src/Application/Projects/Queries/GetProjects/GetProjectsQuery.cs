using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Queries.GetProjects;

public record GetProjectsQuery : IRequest<PaginatedList<ProjectDto>>
{
    public int? CustomerId { get; init; }
    public int? ShopId { get; init; }
    public ProjectStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public record ProjectDto
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
    public int TaskCount { get; init; }
    public int MaterialCount { get; init; }
    public int LabourEntryCount { get; init; }
    public decimal TotalMaterialCost { get; init; }
    public decimal TotalLabourCost { get; init; }
    public decimal TotalCost { get; init; }
    public DateTimeOffset Created { get; init; }
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PaginatedList<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetProjectsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.Projects
            .Include(p => p.Customer)
            .Include(p => p.Shop)
            .Include(p => p.Invoice)
            .Include(p => p.Tasks)
            .Include(p => p.Materials)
            .Include(p => p.LabourEntries)
            .Where(p => p.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(p => p.CustomerId == request.CustomerId.Value);
        }

        if (request.ShopId.HasValue)
        {
            query = query.Where(p => p.ShopId == request.ShopId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(p => p.Created)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer != null ? p.Customer.FullName : null,
                ShopId = p.ShopId,
                ShopName = p.Shop != null ? p.Shop.Name : null,
                Status = p.Status,
                StartDate = p.StartDate,
                ExpectedCompletionDate = p.ExpectedCompletionDate,
                CompletedDate = p.CompletedDate,
                InvoiceId = p.InvoiceId,
                InvoiceNumber = p.Invoice != null ? p.Invoice.DocumentNumber : null,
                TaskCount = p.Tasks.Count,
                MaterialCount = p.Materials.Count,
                LabourEntryCount = p.LabourEntries.Count,
                TotalMaterialCost = p.Materials.Sum(m => m.TotalCost),
                TotalLabourCost = p.LabourEntries.Sum(l => l.TotalCost),
                TotalCost = p.Materials.Sum(m => m.TotalCost) + p.LabourEntries.Sum(l => l.TotalCost),
                Created = p.Created
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

