using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetAccounts;

public record GetAccountsQuery : IRequest<PaginatedList<AccountDto>>
{
    public int? StoreId { get; init; }
    public AccountType? Type { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, PaginatedList<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAccountsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new PaginatedList<AccountDto>(new List<AccountDto>(), 0, request.PageNumber, request.PageSize);
        }

        var query = _context.Accounts
            .Where(a => a.BusinessId == _businessContext.CurrentBusinessId);

        if (request.StoreId.HasValue)
        {
            query = query.Where(a => a.StoreId == request.StoreId);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(a => a.Type == request.Type.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(a => a.IsActive == request.IsActive.Value);
        }

        query = query.OrderBy(a => a.Name);

        return await query
            .Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type,
                CurrentBalance = a.CurrentBalance,
                IsActive = a.IsActive,
                StoreId = a.StoreId,
                AccountNumber = a.AccountNumber,
                BankName = a.BankName,
                Notes = a.Notes
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record AccountDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public AccountType Type { get; init; }
    public decimal CurrentBalance { get; init; }
    public bool IsActive { get; init; }
    public int? StoreId { get; init; }
    public string? AccountNumber { get; init; }
    public string? BankName { get; init; }
    public string? Notes { get; init; }
}

