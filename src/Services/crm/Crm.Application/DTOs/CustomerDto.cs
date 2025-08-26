using Crm.Domain.Entities;

namespace Crm.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerSegment Segment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public decimal TotalSpent { get; set; }
    public int PurchaseCount { get; set; }
    public int LoyaltyPoints { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsLapsed { get; set; }
    public bool IsHighValue { get; set; }
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
