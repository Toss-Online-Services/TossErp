using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Application.Common.DTOs;

/// <summary>
/// Purchase Order DTO
/// </summary>
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public PaymentTerms PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? ApprovalNotes { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? SentAt { get; set; }
    public string? SentBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? CancelledBy { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalReceivedQuantity { get; set; }
    public decimal TotalRemainingQuantity { get; set; }
    public bool IsFullyReceived { get; set; }
    public bool IsPartiallyReceived { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Purchase Order Item DTO
/// </summary>
public class PurchaseOrderItemDto
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string? Notes { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal LineTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsFullyReceived { get; set; }
    public decimal RemainingQuantity { get; set; }
}

/// <summary>
/// Supplier DTO
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public SupplierStatus Status { get; set; }
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; }
    public decimal? LeadTimeDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Create Purchase Order Request
/// </summary>
public class CreatePurchaseOrderRequest
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PaymentTerms PaymentTerms { get; set; } = PaymentTerms.Net30;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Create Purchase Order Item Request
/// </summary>
public class CreatePurchaseOrderItemRequest
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; } = 0.15m;
    public decimal? DiscountPercentage { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Submit Purchase Order Request
/// </summary>
public class SubmitPurchaseOrderRequest
{
    public Guid PurchaseOrderId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Approve Purchase Order Request
/// </summary>
public class ApprovePurchaseOrderRequest
{
    public Guid PurchaseOrderId { get; set; }
    public string? ApprovalNotes { get; set; }
}

/// <summary>
/// Send Purchase Order Request
/// </summary>
public class SendPurchaseOrderRequest
{
    public Guid PurchaseOrderId { get; set; }
}

/// <summary>
/// Acknowledge Purchase Order Request
/// </summary>
public class AcknowledgePurchaseOrderRequest
{
    public Guid PurchaseOrderId { get; set; }
}

/// <summary>
/// Receive Items Request
/// </summary>
public class ReceiveItemsRequest
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ItemId { get; set; }
    public decimal ReceivedQuantity { get; set; }
}

/// <summary>
/// Receive Purchase Order Request
/// </summary>
public class ReceivePurchaseOrderRequest
{
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public string? ReceiptNumber { get; set; }
    public string? Notes { get; set; }
    public List<ReceivePurchaseOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Receive Purchase Order Item Request
/// </summary>
public class ReceivePurchaseOrderItemRequest
{
    public Guid PurchaseOrderItemId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Cancel Purchase Order Request
/// </summary>
public class CancelPurchaseOrderRequest
{
    public Guid PurchaseOrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Put Purchase Order On Hold Request
/// </summary>
public class PutPurchaseOrderOnHoldRequest
{
    public Guid PurchaseOrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Resume Purchase Order From Hold Request
/// </summary>
public class ResumePurchaseOrderFromHoldRequest
{
    public Guid PurchaseOrderId { get; set; }
}

/// <summary>
/// Update Purchase Order Delivery Date Request
/// </summary>
public class UpdatePurchaseOrderDeliveryDateRequest
{
    public Guid PurchaseOrderId { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
}

/// <summary>
/// Add Purchase Order Notes Request
/// </summary>
public class AddPurchaseOrderNotesRequest
{
    public Guid PurchaseOrderId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Create Supplier Request
/// </summary>
public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; }
    public decimal? LeadTimeDays { get; set; }
}

/// <summary>
/// Update Supplier Contact Info Request
/// </summary>
public class UpdateSupplierContactInfoRequest
{
    public Guid SupplierId { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// Update Supplier Business Info Request
/// </summary>
public class UpdateSupplierBusinessInfoRequest
{
    public Guid SupplierId { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
}

/// <summary>
/// Update Supplier Financial Info Request
/// </summary>
public class UpdateSupplierFinancialInfoRequest
{
    public Guid SupplierId { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; }
}

/// <summary>
/// Update Supplier Operational Info Request
/// </summary>
public class UpdateSupplierOperationalInfoRequest
{
    public Guid SupplierId { get; set; }
    public decimal? LeadTimeDays { get; set; }
}

/// <summary>
/// Activate Supplier Request
/// </summary>
public class ActivateSupplierRequest
{
    public Guid SupplierId { get; set; }
}

/// <summary>
/// Deactivate Supplier Request
/// </summary>
public class DeactivateSupplierRequest
{
    public Guid SupplierId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Put Supplier On Hold Request
/// </summary>
public class PutSupplierOnHoldRequest
{
    public Guid SupplierId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Blacklist Supplier Request
/// </summary>
public class BlacklistSupplierRequest
{
    public Guid SupplierId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Add Supplier Notes Request
/// </summary>
public class AddSupplierNotesRequest
{
    public Guid SupplierId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Purchase Order Summary DTO
/// </summary>
public class PurchaseOrderSummaryDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsOverdue { get; set; }
}

/// <summary>
/// Supplier Summary DTO
/// </summary>
public class SupplierSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public SupplierStatus Status { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; }
}

/// <summary>
/// Supplier Price List DTO
/// </summary>
public class SupplierPriceListDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string Currency { get; set; } = "ZAR";
    public bool IsCurrentlyEffective { get; set; }
    public List<SupplierPriceListItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Supplier Price List Item DTO
/// </summary>
public class SupplierPriceListItemDto
{
    public Guid Id { get; set; }
    public Guid SupplierPriceListId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? Notes { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Create Supplier Price List Request
/// </summary>
public class CreateSupplierPriceListRequest
{
    public Guid SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string Currency { get; set; } = "ZAR";
    public List<CreateSupplierPriceListItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Create Supplier Price List Item Request
/// </summary>
public class CreateSupplierPriceListItemRequest
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Update Supplier Price List Request
/// </summary>
public class UpdateSupplierPriceListRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

/// <summary>
/// Add Price List Item Request
/// </summary>
public class AddPriceListItemRequest
{
    public Guid PriceListId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? Notes { get; set; }
}
