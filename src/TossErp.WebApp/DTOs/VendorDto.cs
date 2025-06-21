using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Website { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPurchases { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public string? Notes { get; set; }
        public string BusinessType { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
    }

    public class CreateVendorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public string BusinessType { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
    }

    public class UpdateVendorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        public bool IsActive { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public string BusinessType { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
    }
} 
