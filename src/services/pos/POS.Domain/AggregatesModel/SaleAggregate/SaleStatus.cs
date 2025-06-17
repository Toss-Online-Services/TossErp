#nullable enable
namespace POS.Domain.AggregatesModel.SaleAggregate;

public enum SaleStatus
{
    Pending,    // Initial state when sale is created
    Processing, // Sale is being processed (e.g., payment processing)
    Completed,  // Sale has been successfully completed
    Cancelled,  // Sale was cancelled before completion
    Voided,     // Sale was voided after completion
    Refunded    // Sale was refunded after completion
}

