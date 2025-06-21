using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class GroupPurchaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public List<GroupPurchaseItemDto> Items { get; set; } = new();
        public decimal TotalValue { get; set; }
        public int TotalParticipants { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
        
        // UI expected properties
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public int MemberCount { get; set; }
        public string GroupNumber { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal GroupPrice { get; set; }
        public int MinimumQuantity { get; set; }
        public int TargetQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string DeliveryLocation { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
    }

    public class GroupPurchaseItemDto
    {
        public Guid Id { get; set; }
        public Guid GroupPurchaseId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateGroupPurchaseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid OrganizerId { get; set; }

        [Required]
        public List<CreateGroupPurchaseItemDto> Items { get; set; } = new();

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // UI expected properties
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal GroupPrice { get; set; }
        public int MinimumQuantity { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DeliveryLocation { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class CreateGroupPurchaseItemDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }
    }

    public class UpdateGroupPurchaseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
} 
