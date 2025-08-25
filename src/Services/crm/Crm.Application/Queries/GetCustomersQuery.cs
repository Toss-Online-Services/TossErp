using Crm.Application.DTOs;
using Crm.Domain.Entities;
using Crm.Domain.Repositories;
using MediatR;

namespace Crm.Application.Queries;

public class GetCustomersQuery : IRequest<PagedResult<CustomerDto>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Segment { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var (customers, totalCount) = await _customerRepository.GetPagedAsync(
            page: request.Page,
            limit: request.Limit,
            search: request.Search,
            status: request.Status,
            segment: request.Segment,
            sortBy: request.SortBy,
            sortOrder: request.SortOrder,
            cancellationToken);

        var customerDtos = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            DateOfBirth = c.DateOfBirth,
            Status = c.Status.ToString(),
            Segment = c.Segment.ToString(),
            TotalSpent = c.TotalSpent,
            PurchaseCount = c.PurchaseCount,
            LoyaltyPoints = c.LoyaltyPoints,
            LastPurchaseDate = c.LastPurchaseDate,
            CreatedAt = c.CreatedAt,
            IsLapsed = c.IsLapsed,
            IsHighValue = c.IsHighValue,
            AverageOrderValue = c.PurchaseCount > 0 ? c.TotalSpent / c.PurchaseCount : 0
        }).ToList();

        return new PagedResult<CustomerDto>
        {
            Data = customerDtos,
            TotalCount = totalCount,
            Page = request.Page,
            Limit = request.Limit,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Limit),
            HasNextPage = request.Page * request.Limit < totalCount,
            HasPreviousPage = request.Page > 1
        };
    }
}
