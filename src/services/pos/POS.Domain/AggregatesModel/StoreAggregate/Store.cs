using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StoreAggregate;

public class Store : AggregateRoot
{
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Store()
    {
        Name = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public Store(string name, string address, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string address, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
} 
