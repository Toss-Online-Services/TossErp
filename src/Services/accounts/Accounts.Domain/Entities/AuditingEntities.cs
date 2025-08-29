using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Accounting Document entity for document management
/// </summary>
[Table("AccountingDocuments")]
public class AccountingDocument : Entity
{
    [Required]
    [StringLength(200)]
    public string DocumentName { get; private set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DocumentType { get; private set; } = string.Empty;

    [StringLength(500)]
    public string? DocumentPath { get; private set; }

    public long FileSize { get; private set; }

    [StringLength(50)]
    public string? MimeType { get; private set; }

    public Guid? RelatedEntityId { get; private set; }

    [StringLength(100)]
    public string? RelatedEntityType { get; private set; }

    [StringLength(500)]
    public string? Description { get; private set; }

    private AccountingDocument() : base() { }

    public AccountingDocument(
        Guid id,
        string tenantId,
        string documentName,
        string documentType,
        string? documentPath = null,
        long fileSize = 0,
        string? mimeType = null,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        string? description = null,
        string? createdBy = null) : base(id, tenantId)
    {
        DocumentName = documentName ?? throw new ArgumentNullException(nameof(documentName));
        DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
        DocumentPath = documentPath;
        FileSize = fileSize;
        MimeType = mimeType;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        Description = description;
        CreatedBy = createdBy;
    }

    public static AccountingDocument Create(
        string tenantId,
        string documentName,
        string documentType,
        string? documentPath = null,
        long fileSize = 0,
        string? mimeType = null,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        string? description = null,
        string? createdBy = null)
    {
        return new AccountingDocument(
            Guid.NewGuid(),
            tenantId,
            documentName,
            documentType,
            documentPath,
            fileSize,
            mimeType,
            relatedEntityId,
            relatedEntityType,
            description,
            createdBy);
    }
}

/// <summary>
/// Accounting Audit Log entity for tracking changes
/// </summary>
[Table("AccountingAuditLogs")]
public class AccountingAuditLog : Entity
{
    [Required]
    [StringLength(100)]
    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    [Required]
    [StringLength(50)]
    public string Action { get; private set; } = string.Empty;

    [StringLength(500)]
    public string? OldValues { get; private set; }

    [StringLength(500)]
    public string? NewValues { get; private set; }

    [StringLength(200)]
    public string? ActionBy { get; private set; }

    public DateTime ActionDate { get; private set; }

    [StringLength(500)]
    public string? Description { get; private set; }

    private AccountingAuditLog() : base() { }

    public AccountingAuditLog(
        Guid id,
        string tenantId,
        string entityType,
        Guid entityId,
        string action,
        string? oldValues = null,
        string? newValues = null,
        string? actionBy = null,
        DateTime? actionDate = null,
        string? description = null,
        string? createdBy = null) : base(id, tenantId)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId;
        Action = action ?? throw new ArgumentNullException(nameof(action));
        OldValues = oldValues;
        NewValues = newValues;
        ActionBy = actionBy;
        ActionDate = actionDate ?? DateTime.UtcNow;
        Description = description;
        CreatedBy = createdBy;
    }

    public static AccountingAuditLog Create(
        string tenantId,
        string entityType,
        Guid entityId,
        string action,
        string? oldValues = null,
        string? newValues = null,
        string? actionBy = null,
        DateTime? actionDate = null,
        string? description = null,
        string? createdBy = null)
    {
        return new AccountingAuditLog(
            Guid.NewGuid(),
            tenantId,
            entityType,
            entityId,
            action,
            oldValues,
            newValues,
            actionBy,
            actionDate,
            description,
            createdBy);
    }
}
