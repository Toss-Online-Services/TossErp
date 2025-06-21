using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ParentCategoryName { get; set; }
        public int ItemCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }
    }
} 
