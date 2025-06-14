using System;
using System.Collections.Generic;
using TossErp.POS.Domain.SeedWork;
using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public Address Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<PaymentMethod> _paymentMethods;
    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer()
    {
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Address = new Address();
        CreatedAt = DateTime.UtcNow;
        _paymentMethods = new List<PaymentMethod>();
    }

    public Buyer(string name, string email, string phone, Address address) : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (address == null)
            throw new DomainException("Address cannot be null");

        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string phone, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (address == null)
            throw new DomainException("Address cannot be null");

        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPaymentMethod(PaymentMethod paymentMethod)
    {
        if (paymentMethod == null)
            throw new DomainException("Payment method cannot be null");

        _paymentMethods.Add(paymentMethod);
        AddDomainEvent(new PaymentMethodAddedDomainEvent(this, paymentMethod));
    }

    public void RemovePaymentMethod(int paymentMethodId)
    {
        var paymentMethod = _paymentMethods.Find(pm => pm.Id == paymentMethodId);
        if (paymentMethod != null)
        {
            _paymentMethods.Remove(paymentMethod);
        }
    }

    public new void AddDomainEvent(DomainEvent domainEvent)
    {
        base.AddDomainEvent(domainEvent);
    }

    public new void ClearDomainEvents()
    {
        base.ClearDomainEvents();
    }
}
