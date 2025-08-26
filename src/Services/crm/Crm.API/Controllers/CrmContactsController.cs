using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Crm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrmContactsController : ControllerBase
{
    private readonly ILogger<CrmContactsController> _logger;

    public CrmContactsController(ILogger<CrmContactsController> logger)
    {
        _logger = logger;
    }

    // DTO for contact data that matches our frontend
    public class ContactDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Type { get; set; } = "Lead";
        public string Status { get; set; } = "Active";
        public string Source { get; set; } = "Website";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }

    public class ContactQueryDto
    {
        public string? Search { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    // Sample data that matches our frontend structure
    private static readonly List<ContactDto> SampleContacts = new()
    {
        new ContactDto
        {
            Id = "1",
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@techcorp.com",
            Phone = "+1-555-0123",
            Company = "TechCorp Solutions",
            JobTitle = "CTO",
            Type = "Customer",
            Status = "Active",
            Source = "Website",
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            LastActivity = DateTime.UtcNow.AddDays(-2)
        },
        new ContactDto
        {
            Id = "2",
            FirstName = "Sarah",
            LastName = "Johnson",
            Email = "sarah.johnson@innovate.io",
            Phone = "+1-555-0456",
            Company = "Innovate Labs",
            JobTitle = "Product Manager",
            Type = "Lead",
            Status = "Active",
            Source = "Referral",
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            LastActivity = DateTime.UtcNow.AddDays(-1)
        },
        new ContactDto
        {
            Id = "3",
            FirstName = "Michael",
            LastName = "Chen",
            Email = "m.chen@dataflow.com",
            Phone = "+1-555-0789",
            Company = "DataFlow Analytics",
            JobTitle = "VP Engineering",
            Type = "Prospect",
            Status = "Active",
            Source = "LinkedIn",
            CreatedAt = DateTime.UtcNow.AddDays(-22),
            LastActivity = DateTime.UtcNow.AddDays(-5)
        },
        new ContactDto
        {
            Id = "4",
            FirstName = "Emily",
            LastName = "Rodriguez",
            Email = "emily.r@cloudscale.net",
            Phone = "+1-555-0321",
            Company = "CloudScale Systems",
            JobTitle = "Director of Operations",
            Type = "Customer",
            Status = "Active",
            Source = "Website",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            LastActivity = DateTime.UtcNow.AddHours(-6)
        },
        new ContactDto
        {
            Id = "5",
            FirstName = "David",
            LastName = "Park",
            Email = "david.park@nextech.co",
            Phone = "+1-555-0654",
            Company = "NexTech Innovations",
            JobTitle = "Founder & CEO",
            Type = "Lead",
            Status = "Active",
            Source = "Trade Show",
            CreatedAt = DateTime.UtcNow.AddDays(-12),
            LastActivity = DateTime.UtcNow.AddDays(-3)
        }
    };

    [HttpGet]
    public ActionResult<PagedResultDto<ContactDto>> GetContacts([FromQuery] ContactQueryDto query)
    {
        try
        {
            var contacts = SampleContacts.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchTerm = query.Search.ToLower();
                contacts = contacts.Where(c => 
                    c.FirstName.ToLower().Contains(searchTerm) ||
                    c.LastName.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.Company.ToLower().Contains(searchTerm) ||
                    c.JobTitle.ToLower().Contains(searchTerm));
            }

            // Apply type filter
            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                contacts = contacts.Where(c => c.Type.Equals(query.Type, StringComparison.OrdinalIgnoreCase));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                contacts = contacts.Where(c => c.Status.Equals(query.Status, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var isDescending = query.SortOrder?.ToLower() == "desc";
                
                contacts = query.SortBy.ToLower() switch
                {
                    "firstname" => isDescending ? contacts.OrderByDescending(c => c.FirstName) : contacts.OrderBy(c => c.FirstName),
                    "lastname" => isDescending ? contacts.OrderByDescending(c => c.LastName) : contacts.OrderBy(c => c.LastName),
                    "email" => isDescending ? contacts.OrderByDescending(c => c.Email) : contacts.OrderBy(c => c.Email),
                    "company" => isDescending ? contacts.OrderByDescending(c => c.Company) : contacts.OrderBy(c => c.Company),
                    "jobtitle" => isDescending ? contacts.OrderByDescending(c => c.JobTitle) : contacts.OrderBy(c => c.JobTitle),
                    "createdat" => isDescending ? contacts.OrderByDescending(c => c.CreatedAt) : contacts.OrderBy(c => c.CreatedAt),
                    "lastactivity" => isDescending ? contacts.OrderByDescending(c => c.LastActivity) : contacts.OrderBy(c => c.LastActivity),
                    _ => contacts.OrderBy(c => c.FirstName)
                };
            }
            else
            {
                contacts = contacts.OrderBy(c => c.FirstName);
            }

            var totalCount = contacts.Count();
            var pagedContacts = contacts.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);

            var result = new PagedResultDto<ContactDto>
            {
                Data = pagedContacts,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contacts");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<ContactDto> GetContact(string id)
    {
        try
        {
            var contact = SampleContacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with ID {id} not found");
            }

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public ActionResult<ContactDto> CreateContact([FromBody] ContactDto contact)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contact.Id = Guid.NewGuid().ToString();
            contact.CreatedAt = DateTime.UtcNow;
            contact.LastActivity = DateTime.UtcNow;

            SampleContacts.Add(contact);

            _logger.LogInformation("Created contact {Id}", contact.Id);
            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public ActionResult<ContactDto> UpdateContact(string id, [FromBody] ContactDto contact)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingContact = SampleContacts.FirstOrDefault(c => c.Id == id);
            if (existingContact == null)
            {
                return NotFound($"Contact with ID {id} not found");
            }

            // Update properties
            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;
            existingContact.Phone = contact.Phone;
            existingContact.Company = contact.Company;
            existingContact.JobTitle = contact.JobTitle;
            existingContact.Type = contact.Type;
            existingContact.Status = contact.Status;
            existingContact.Source = contact.Source;
            existingContact.LastActivity = DateTime.UtcNow;

            _logger.LogInformation("Updated contact {Id}", id);
            return Ok(existingContact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteContact(string id)
    {
        try
        {
            var contact = SampleContacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with ID {id} not found");
            }

            SampleContacts.Remove(contact);

            _logger.LogInformation("Deleted contact {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("bulk-delete")]
    public ActionResult BulkDeleteContacts([FromBody] List<string> ids)
    {
        try
        {
            var contactsToRemove = SampleContacts.Where(c => ids.Contains(c.Id)).ToList();
            
            foreach (var contact in contactsToRemove)
            {
                SampleContacts.Remove(contact);
            }

            _logger.LogInformation("Bulk deleted {Count} contacts", contactsToRemove.Count);
            return Ok(new { deletedCount = contactsToRemove.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting contacts");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("export")]
    public ActionResult ExportContacts()
    {
        try
        {
            var csvData = GenerateCsv();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvData);
            
            return File(bytes, "text/csv", $"contacts_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting contacts");
            return StatusCode(500, "Internal server error");
        }
    }

    private string GenerateCsv()
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("First Name,Last Name,Email,Phone,Company,Job Title,Type,Status,Source,Created At,Last Activity");
        
        foreach (var contact in SampleContacts)
        {
            csv.AppendLine($"{EscapeCsv(contact.FirstName)},{EscapeCsv(contact.LastName)},{EscapeCsv(contact.Email)},{EscapeCsv(contact.Phone)},{EscapeCsv(contact.Company)},{EscapeCsv(contact.JobTitle)},{EscapeCsv(contact.Type)},{EscapeCsv(contact.Status)},{EscapeCsv(contact.Source)},{contact.CreatedAt:yyyy-MM-dd HH:mm:ss},{contact.LastActivity:yyyy-MM-dd HH:mm:ss}");
        }
        
        return csv.ToString();
    }

    private string EscapeCsv(string field)
    {
        if (field == null) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
