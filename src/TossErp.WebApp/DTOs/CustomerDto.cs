using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public string? Notes { get; set; }
        public Guid BusinessId { get; set; }
        
        // UI alias properties - made writable for UI binding
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
        
        // UI alias properties - made writable for UI binding
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
    }

    public class UpdateCustomerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        public bool IsActive { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
        
        // UI alias properties - made writable for UI binding
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
    }
} 
