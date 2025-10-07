using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Sales;

public enum PaymentMethod
{
    Cash,
    Card,
    MobileMoney,
    BankTransfer,
    Other
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled,
    Refunded
}

public class Payment : BaseEntity
{
    public int? SaleId { get; set; }
    public virtual Sale? Sale { get; set; }
    
    public string? SaleNumber { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? TransactionId { get; set; }
    
    // Card payment details
    public string? CardLast4 { get; set; }
    public string? CardType { get; set; }
    
    // Mobile money details
    public string? MobileMoneyProvider { get; set; }
    public string? MobileMoneyNumber { get; set; }
    
    // Bank transfer details
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    
    public int? ProcessedById { get; set; }
    public string? ProcessedByName { get; set; }
    
    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be completed");
        
        Status = PaymentStatus.Completed;
        AddDomainEvent(new PaymentCompletedEvent(Id, Amount, Method));
    }
    
    public void Refund(string reason)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded");
        
        Status = PaymentStatus.Refunded;
        Notes = $"{Notes}\nRefunded: {reason}";
        AddDomainEvent(new PaymentRefundedEvent(Id, Amount, reason));
    }
}

// Domain Events
public class PaymentCompletedEvent : DomainEvent
{
    public int PaymentId { get; }
    public decimal Amount { get; }
    public PaymentMethod Method { get; }
    
    public PaymentCompletedEvent(int paymentId, decimal amount, PaymentMethod method)
    {
        PaymentId = paymentId;
        Amount = amount;
        Method = method;
    }
}

public class PaymentRefundedEvent : DomainEvent
{
    public int PaymentId { get; }
    public decimal Amount { get; }
    public string Reason { get; }
    
    public PaymentRefundedEvent(int paymentId, decimal amount, string reason)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }
}

