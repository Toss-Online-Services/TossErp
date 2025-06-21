using TossErp.Domain.SeedWork;
using TossErp.Domain.Events;

namespace TossErp.Domain.AggregatesModel.SaleAggregate;

public class Sale : Entity, IAggregateRoot
{
    public string SaleNumber { get; private set; } = string.Empty;
    public Guid BusinessId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public string PaymentStatus { get; private set; } = string.Empty; // "pending", "completed", "failed", "refunded"
    public string SaleStatus { get; private set; } = string.Empty; // "completed", "cancelled", "pending"
    public DateTime SaleDate { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public string? Notes { get; private set; }
    public bool IsOffline { get; private set; }
    public DateTime? SyncedAt { get; private set; }

    private readonly List<SaleItem> _items;
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private readonly List<PaymentTransaction> _payments;
    public IReadOnlyCollection<PaymentTransaction> Payments => _payments.AsReadOnly();

    protected Sale()
    {
        _items = new List<SaleItem>();
        _payments = new List<PaymentTransaction>();
        SaleNumber = string.Empty;
        PaymentMethod = string.Empty;
        PaymentStatus = "pending";
        SaleStatus = "pending";
        SaleDate = DateTime.UtcNow;
    }

    public Sale(Guid businessId, Guid? customerId, string paymentMethod, Guid? createdBy = null, string? notes = null, bool isOffline = false) : this()
    {
        Id = Guid.NewGuid();
        SaleNumber = GenerateSaleNumber();
        BusinessId = businessId;
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        PaymentStatus = "pending";
        SaleStatus = "pending";
        SaleDate = DateTime.UtcNow;
        CreatedBy = createdBy;
        Notes = notes;
        IsOffline = isOffline;
        SubTotal = 0;
        TaxAmount = 0;
        DiscountAmount = 0;
        TotalAmount = 0;

        AddDomainEvent(new SaleCreatedDomainEvent(this));
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice, decimal? discount = null)
    {
        var item = new SaleItem(Id, productId, productName, quantity, unitPrice, discount);
        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
        }
    }

    public void UpdateItemQuantity(Guid itemId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.UpdateQuantity(quantity);
            RecalculateTotals();
        }
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        DiscountAmount = discountAmount;
        RecalculateTotals();
    }

    public void CompleteSale()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot complete sale with no items");

        SaleStatus = "completed";
        PaymentStatus = "completed";
        
        AddDomainEvent(new SaleCompletedDomainEvent(this));
    }

    public void CancelSale(string reason = "")
    {
        SaleStatus = "cancelled";
        PaymentStatus = "refunded";
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}; Cancelled: {reason}";
        
        AddDomainEvent(new SaleCancelledDomainEvent(this));
    }

    public void AddPayment(decimal amount, string paymentMethod, string transactionId = "", string status = "completed")
    {
        var payment = new PaymentTransaction(Id, amount, paymentMethod, transactionId, status);
        _payments.Add(payment);
        
        if (status == "completed")
        {
            PaymentStatus = "completed";
        }
    }

    public void MarkAsSynced()
    {
        SyncedAt = DateTime.UtcNow;
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(item => item.TotalPrice);
        TaxAmount = SubTotal * 0.15m; // 15% VAT for South Africa
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
    }

    private string GenerateSaleNumber()
    {
        return $"SALE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    public decimal GetTotalPaid() => _payments.Where(p => p.Status == "completed").Sum(p => p.Amount);
    
    public bool IsFullyPaid() => GetTotalPaid() >= TotalAmount;
    
    public decimal GetRemainingBalance() => TotalAmount - GetTotalPaid();
} 
