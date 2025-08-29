using TossErp.Accounts.Application.DTOs;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.ValueObjects;
using InvoiceAggregate = TossErp.Accounts.Domain.Aggregates.Invoice;
using InvoiceLine = TossErp.Accounts.Domain.Entities.InvoiceLine;

namespace TossErp.Accounts.Application.Common;

public static class InvoiceMappingHelper
{
    public static InvoiceDto MapToDto(InvoiceAggregate invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? invoice.CustomerName,
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = DateOnly.FromDateTime(invoice.InvoiceDate),
            DueDate = DateOnly.FromDateTime(invoice.DueDate),
            SubtotalAmount = invoice.SubTotal.Amount,
            TaxAmount = invoice.TotalTax.Amount,
            DiscountAmount = 0, // Not in aggregate
            TotalAmount = invoice.TotalAmount.Amount,
            PaidAmount = invoice.PaidAmount.Amount,
            BalanceAmount = invoice.OutstandingAmount.Amount,
            Currency = invoice.Currency.ToString(),
            LineItems = invoice.Lines.Select(MapAggregateLineItemToDto).ToList(),
            BillingAddress = null, // Not in aggregate - would need customer address
            ShippingAddress = null, // Not in aggregate
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            InternalNotes = null, // Not in aggregate
            Reference = null, // Not in aggregate
            PurchaseOrderNumber = null, // Not in aggregate
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy,
            LastModified = null, // Not in aggregate
            LastModifiedBy = null, // Not in aggregate
            PaidDate = invoice.Status == TossErp.Accounts.Domain.Enums.InvoiceStatus.Paid ? DateOnly.FromDateTime(DateTime.UtcNow) : null
        };
    }

    public static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? invoice.CustomerName ?? string.Empty,
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = DateOnly.FromDateTime(invoice.IssueDate),
            DueDate = DateOnly.FromDateTime(invoice.DueDate),
            SubtotalAmount = invoice.SubtotalAmount.Amount,
            TaxAmount = invoice.TaxAmount.Amount,
            DiscountAmount = invoice.DiscountAmount?.Amount ?? 0,
            TotalAmount = invoice.TotalAmount.Amount,
            PaidAmount = invoice.PaidAmount?.Amount ?? 0,
            BalanceAmount = invoice.BalanceAmount?.Amount ?? invoice.TotalAmount.Amount,
            Currency = invoice.Currency.ToString(),
            LineItems = invoice.LineItems.Select(MapEntityLineItemToDto).ToList(),
            BillingAddress = invoice.BillingAddress != null ? new CustomerAddressDto
            {
                Street = invoice.BillingAddress.Street,
                Street2 = invoice.BillingAddress.Suburb,
                City = invoice.BillingAddress.City,
                State = invoice.BillingAddress.Province,
                PostalCode = invoice.BillingAddress.PostalCode,
                Country = invoice.BillingAddress.Country
            } : null,
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            InternalNotes = invoice.InternalNotes,
            Reference = invoice.Reference,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy,
            LastModified = invoice.LastModified,
            LastModifiedBy = invoice.LastModifiedBy,
            PaidDate = invoice.PaidDate.HasValue ? DateOnly.FromDateTime(invoice.PaidDate.Value) : null
        };
    }

    private static InvoiceLineItemDto MapAggregateLineItemToDto(InvoiceLine lineItem)
    {
        return new InvoiceLineItemDto
        {
            Id = lineItem.Id,
            Description = lineItem.Description ?? lineItem.ItemName,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Amount,
            LineTotal = lineItem.LineTotal?.Amount ?? 0,
            TaxRate = lineItem.TaxRate?.Rate ?? 0,
            TaxAmount = lineItem.TaxAmount?.Amount ?? 0,
            ProductCode = lineItem.ProductCode
        };
    }

    private static InvoiceLineItemDto MapEntityLineItemToDto(TossErp.Accounts.Domain.Entities.InvoiceLineItem lineItem)
    {
        return new InvoiceLineItemDto
        {
            Id = lineItem.Id,
            ProductName = lineItem.ItemName,
            Description = lineItem.Description ?? string.Empty,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Amount,
            LineTotal = lineItem.LineTotal?.Amount ?? 0,
            TaxRate = lineItem.TaxRate?.Rate ?? 0,
            TaxAmount = lineItem.TaxAmount?.Amount ?? 0
        };
    }

    public static Address ConvertToAddressValueObject(CustomerAddress? customerAddress)
    {
        if (customerAddress == null)
            throw new ArgumentNullException(nameof(customerAddress));

        return Address.Create(
            customerAddress.AddressLine1 ?? string.Empty,
            customerAddress.City ?? string.Empty,
            customerAddress.State ?? string.Empty,
            customerAddress.PostalCode ?? string.Empty,
            customerAddress.Country ?? "South Africa",
            customerAddress.Suburb,
            customerAddress.TownshipName
        );
    }
}