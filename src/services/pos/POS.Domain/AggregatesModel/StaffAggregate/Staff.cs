using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.StaffAggregate;

public class Staff : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string? Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Staff() { }

    public Staff(string name, string email, string phone, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Staff email cannot be empty");

        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Staff phone cannot be empty");

        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string phone, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Staff email cannot be empty");

        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Staff phone cannot be empty");

        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
} 
