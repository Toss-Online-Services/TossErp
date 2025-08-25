using Crm.Domain.Entities;

namespace Crm.Application.DTOs;

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public decimal TotalSpent { get; set; }
    public int PurchaseCount { get; set; }
    public int LoyaltyPoints { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsLapsed { get; set; }
    public bool IsHighValue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class CreateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class UpdateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CustomerListFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? Segment { get; set; }
    public bool? IsLapsed { get; set; }
    public bool? IsHighValue { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public decimal? MinTotalSpent { get; set; }
    public decimal? MaxTotalSpent { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
