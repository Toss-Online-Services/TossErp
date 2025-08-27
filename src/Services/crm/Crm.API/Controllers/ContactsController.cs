using Microsoft.AspNetCore.Mvc;

namespace Crm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(ILogger<ContactsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<ContactListResponse> GetContacts(
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contacts with search: {Search}, type: {Type}, status: {Status}", 
                search, type, status);

            // For now, return sample data that matches our frontend interface
            var allContacts = GetSampleContacts();

            // Apply filtering
            var filteredContacts = allContacts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLowerInvariant();
                filteredContacts = filteredContacts.Where(c =>
                    c.Name.ToLowerInvariant().Contains(searchLower) ||
                    c.Email.ToLowerInvariant().Contains(searchLower) ||
                    (c.Company?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (c.Phone?.Contains(search) ?? false));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                filteredContacts = filteredContacts.Where(c => c.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                filteredContacts = filteredContacts.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            var filteredList = filteredContacts.ToList();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                
                filteredList = sortBy.ToLowerInvariant() switch
                {
                    "name" => isDescending 
                        ? filteredList.OrderByDescending(c => c.Name).ToList()
                        : filteredList.OrderBy(c => c.Name).ToList(),
                    "email" => isDescending 
                        ? filteredList.OrderByDescending(c => c.Email).ToList()
                        : filteredList.OrderBy(c => c.Email).ToList(),
                    "company" => isDescending 
                        ? filteredList.OrderByDescending(c => c.Company).ToList()
                        : filteredList.OrderBy(c => c.Company).ToList(),
                    "type" => isDescending 
                        ? filteredList.OrderByDescending(c => c.Type).ToList()
                        : filteredList.OrderBy(c => c.Type).ToList(),
                    "status" => isDescending 
                        ? filteredList.OrderByDescending(c => c.Status).ToList()
                        : filteredList.OrderBy(c => c.Status).ToList(),
                    "createdat" => isDescending 
                        ? filteredList.OrderByDescending(c => c.CreatedAt).ToList()
                        : filteredList.OrderBy(c => c.CreatedAt).ToList(),
                    "lastactivity" => isDescending 
                        ? filteredList.OrderByDescending(c => c.LastActivity).ToList()
                        : filteredList.OrderBy(c => c.LastActivity).ToList(),
                    _ => filteredList
                };
            }

            // Apply pagination
            var totalCount = filteredList.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var skip = (page - 1) * pageSize;
            var paginatedContacts = filteredList.Skip(skip).Take(pageSize).ToList();

            var response = new ContactListResponse
            {
                Contacts = paginatedContacts,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contacts");
            return StatusCode(500, "An error occurred while retrieving contacts");
        }
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ContactDto> GetContact(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var contacts = GetSampleContacts();
            var contact = contacts.FirstOrDefault(c => c.Id == id.ToString());

            if (contact == null)
                return NotFound($"Contact with ID {id} not found");

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact {ContactId}", id);
            return StatusCode(500, "An error occurred while retrieving the contact");
        }
    }

    [HttpPost]
    public ActionResult<ContactDto> CreateContact([FromBody] CreateContactRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new contact: {Name}", request.Name);

            var contact = new ContactDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Company = request.Company,
                Type = request.Type,
                Status = request.Status,
                Industry = request.Industry,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow.ToString("O"),
                LastActivity = DateTime.UtcNow.ToString("O"),
                AssignedTo = "Current User" // This would come from authentication
            };

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact");
            return StatusCode(500, "An error occurred while creating the contact");
        }
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ContactDto> UpdateContact(Guid id, [FromBody] UpdateContactRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating contact: {ContactId}", id);

            // In a real implementation, this would update the database
            var updatedContact = new ContactDto
            {
                Id = id.ToString(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Company = request.Company,
                Type = request.Type,
                Status = request.Status,
                Industry = request.Industry,
                Notes = request.Notes,
                CreatedAt = request.CreatedAt ?? DateTime.UtcNow.AddDays(-30).ToString("O"),
                LastActivity = DateTime.UtcNow.ToString("O"),
                AssignedTo = request.AssignedTo
            };

            return Ok(updatedContact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {ContactId}", id);
            return StatusCode(500, "An error occurred while updating the contact");
        }
    }

    [HttpDelete("{id:guid}")]
    public ActionResult DeleteContact(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting contact: {ContactId}", id);
            
            // In a real implementation, this would delete from the database
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {ContactId}", id);
            return StatusCode(500, "An error occurred while deleting the contact");
        }
    }

    [HttpDelete]
    public ActionResult DeleteContacts([FromBody] BulkDeleteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Bulk deleting {Count} contacts", request.Ids.Count);
            
            // In a real implementation, this would delete multiple contacts from the database
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting contacts");
            return StatusCode(500, "An error occurred while deleting the contacts");
        }
    }

    private static List<ContactDto> GetSampleContacts()
    {
        return new List<ContactDto>
        {
            new()
            {
                Id = "1",
                Name = "Thabo Mthembu",
                Email = "thabo@ruralenterprises.co.za",
                Phone = "+27 82 123 4567",
                Company = "Mthembu Spaza Shop",
                Type = "customer",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-200).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-5).ToString("O"),
                Industry = "Retail",
                AssignedTo = "Sarah Johnson"
            },
            new()
            {
                Id = "2",
                Name = "Nomsa Khumalo",
                Email = "nomsa.khumalo@gmail.com",
                Phone = "+27 83 456 7890",
                Company = "Khumalo Catering Services",
                Type = "lead",
                Status = "prospect",
                CreatedAt = DateTime.UtcNow.AddDays(-180).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-7).ToString("O"),
                Industry = "Food & Hospitality",
                AssignedTo = "Mike Wilson"
            },
            new()
            {
                Id = "3",
                Name = "Johannes Pieterse",
                Email = "johannes@farmsupplies.co.za",
                Phone = "+27 81 789 0123",
                Company = "Pieterse Farm Supplies",
                Type = "vendor",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-150).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-2).ToString("O"),
                Industry = "Agriculture",
                AssignedTo = "Lisa Chen"
            },
            new()
            {
                Id = "4",
                Name = "Lerato Mokone",
                Email = "lerato@handicrafts.org",
                Phone = "+27 84 234 5678",
                Company = "Traditional Handicrafts Co-op",
                Type = "partner",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-120).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-1).ToString("O"),
                Industry = "Arts & Crafts",
                AssignedTo = "David Brown"
            },
            new()
            {
                Id = "5",
                Name = "Sipho Ndlovu",
                Email = "sipho.ndlovu@transport.co.za",
                Phone = "+27 85 345 6789",
                Company = "Ndlovu Transport Services",
                Type = "customer",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-100).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-8).ToString("O"),
                Industry = "Transportation",
                AssignedTo = "Sarah Johnson"
            },
            new()
            {
                Id = "6",
                Name = "Zanele Dube",
                Email = "zanele@techstartup.co.za",
                Phone = "+27 86 567 8901",
                Company = "Dube Tech Solutions",
                Type = "lead",
                Status = "prospect",
                CreatedAt = DateTime.UtcNow.AddDays(-90).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-12).ToString("O"),
                Industry = "Technology",
                AssignedTo = "Mike Wilson"
            },
            new()
            {
                Id = "7",
                Name = "Mandla Zulu",
                Email = "mandla@construction.co.za",
                Phone = "+27 87 678 9012",
                Company = "Zulu Construction",
                Type = "customer",
                Status = "inactive",
                CreatedAt = DateTime.UtcNow.AddDays(-300).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-90).ToString("O"),
                Industry = "Construction",
                AssignedTo = "Lisa Chen"
            },
            new()
            {
                Id = "8",
                Name = "Nosipho Mhlongo",
                Email = "nosipho@education.org.za",
                Phone = "+27 88 789 0123",
                Company = "Community Education Center",
                Type = "partner",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-80).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-3).ToString("O"),
                Industry = "Education",
                AssignedTo = "David Brown"
            },
            new()
            {
                Id = "9",
                Name = "Bongani Mokoena",
                Email = "bongani@healthcare.co.za",
                Phone = "+27 89 890 1234",
                Company = "Rural Health Clinic",
                Type = "customer",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-70).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-4).ToString("O"),
                Industry = "Healthcare",
                AssignedTo = "Sarah Johnson"
            },
            new()
            {
                Id = "10",
                Name = "Thandiwe Ngcobo",
                Email = "thandiwe@agriculture.co.za",
                Phone = "+27 90 901 2345",
                Company = "Ngcobo Farming Cooperative",
                Type = "vendor",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-60).ToString("O"),
                LastActivity = DateTime.UtcNow.AddDays(-6).ToString("O"),
                Industry = "Agriculture",
                AssignedTo = "Mike Wilson"
            }
        };
    }
}

// DTOs for the API
public class ContactDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string Type { get; set; } = string.Empty; // lead, customer, vendor, partner
    public string Status { get; set; } = string.Empty; // active, inactive, prospect
    public string CreatedAt { get; set; } = string.Empty;
    public string? LastActivity { get; set; }
    public string? Industry { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
}

public class ContactListResponse
{
    public List<ContactDto> Contacts { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CreateContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Notes { get; set; }
}

public class UpdateContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public string? CreatedAt { get; set; }
}

public class BulkDeleteRequest
{
    public List<string> Ids { get; set; } = new();
}
