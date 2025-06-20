using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.Models;

public class CustomerDetails
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    public string Phone { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    public string? Country { get; set; } = "USA";

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string[] Interests { get; set; } = Array.Empty<string>();

    public bool IsActive { get; set; } = true;

    public bool SendMarketingEmails { get; set; } = false;

    public string? Notes { get; set; }
} 
