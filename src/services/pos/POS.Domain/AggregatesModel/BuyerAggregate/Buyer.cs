using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : AggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public Address Address { get; private set; }
    public string? TaxId { get; private set; }
    public string? Notes { get; private set; }
    public string Status { get; private set; } = "Active";
    public Guid StoreId { get; private set; }
    public Store? Store { get; private set; }
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

    public Buyer(Guid id, Guid storeId, string name, string email, string phone, Address address, string? taxId = null, string? notes = null) : this()
    {
        if (id == Guid.Empty)
            throw new DomainException("ID cannot be empty");
        if (storeId == Guid.Empty)
            throw new DomainException("Store ID cannot be empty");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (address == null)
            throw new DomainException("Address cannot be null");

        Id = id;
        StoreId = storeId;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        TaxId = taxId;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string phone, Address address, string? taxId = null, string? notes = null)
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
        TaxId = taxId;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPaymentMethod(PaymentMethod paymentMethod)
    {
        if (paymentMethod == null)
            throw new DomainException("Payment method cannot be null");

        _paymentMethods.Add(paymentMethod);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePaymentMethod(Guid paymentMethodId)
    {
        var paymentMethod = _paymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);
        if (paymentMethod != null)
        {
            _paymentMethods.Remove(paymentMethod);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SetStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
