using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetCashbookEntries;

public record GetCashbookEntriesQuery : IRequest<PaginatedList<CashbookEntryDto>>
{
    public int? AccountId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public CashbookEntryType? Type { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetCashbookEntriesQueryHandler : IRequestHandler<GetCashbookEntriesQuery, PaginatedList<CashbookEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCashbookEntriesQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<CashbookEntryDto>> Handle(GetCashbookEntriesQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new PaginatedList<CashbookEntryDto>(new List<CashbookEntryDto>(), 0, request.PageNumber, request.PageSize);
        }

        var query = _context.CashbookEntries
            .Where(e => e.BusinessId == _businessContext.CurrentBusinessId);

        if (request.AccountId.HasValue)
        {
            query = query.Where(e => e.AccountId == request.AccountId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(e => e.EntryDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(e => e.EntryDate <= request.ToDate.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(e => e.Type == request.Type.Value);
        }

        query = query.OrderByDescending(e => e.EntryDate).ThenByDescending(e => e.Id);

        return await query
            .Select(e => new CashbookEntryDto
            {
                Id = e.Id,
                AccountId = e.AccountId,
                AccountName = e.Account.Name,
                Amount = e.Amount,
                EntryDate = e.EntryDate,
                Type = e.Type,
                Reference = e.Reference,
                Notes = e.Notes,
                CounterpartyAccountId = e.CounterpartyAccountId,
                CounterpartyAccountName = e.CounterpartyAccount != null ? e.CounterpartyAccount.Name : null,
                SourceType = e.SourceType,
                SourceId = e.SourceId,
                PaymentId = e.PaymentId
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record CashbookEntryDto
{
    public int Id { get; init; }
    public int AccountId { get; init; }
    public string AccountName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTimeOffset EntryDate { get; init; }
    public CashbookEntryType Type { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public int? CounterpartyAccountId { get; init; }
    public string? CounterpartyAccountName { get; init; }
    public string? SourceType { get; init; }
    public int? SourceId { get; init; }
    public int? PaymentId { get; init; }
}

