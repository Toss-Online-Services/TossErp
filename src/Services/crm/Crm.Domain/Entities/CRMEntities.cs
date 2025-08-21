using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.SeedWork;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Entities;

/// <summary>
/// Contact entity representing individual contact persons
/// </summary>
public class Contact : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Department { get; private set; }
    public ContactType ContactType { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; }
    public Address? Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastContactedAt { get; private set; }

    private Contact() { } // EF Core

    public Contact(
        Guid id,
        string firstName,
        string lastName,
        EmailAddress email,
        ContactType contactType = ContactType.General,
        bool isPrimary = false,
        PhoneNumber? phone = null,
        string? jobTitle = null,
        string? department = null,
        Address? address = null)
    {
        Id = id;
        FirstName = firstName?.Trim() ?? throw new ArgumentException("First name cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("Last name cannot be empty");
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        JobTitle = jobTitle?.Trim();
        Department = department?.Trim();
        ContactType = contactType;
        IsPrimary = isPrimary;
        IsActive = true;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateContactInfo(string firstName, string lastName, EmailAddress email, PhoneNumber? phone = null)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentException("First name cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("Last name cannot be empty");
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
    }

    public void UpdateJobInfo(string? jobTitle, string? department)
    {
        JobTitle = jobTitle?.Trim();
        Department = department?.Trim();
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void RemoveAsPrimary()
    {
        IsPrimary = false;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void RecordContact()
    {
        LastContactedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Activity entity for tracking customer interactions
/// </summary>
public class Activity : Entity
{
    public ActivityType Type { get; private set; }
    public string Subject { get; private set; }
    public string? Description { get; private set; }
    public ActivityStatus Status { get; private set; }
    public ActivityPriority Priority { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? AssignedTo { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public string? Outcome { get; private set; }
    public string? NextAction { get; private set; }

    private Activity() { } // EF Core

    public Activity(
        Guid id,
        ActivityType type,
        string subject,
        DateTime scheduledAt,
        string createdBy,
        string? description = null,
        ActivityPriority priority = ActivityPriority.Medium,
        string? assignedTo = null)
    {
        Id = id;
        Type = type;
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Description = description?.Trim();
        Status = ActivityStatus.Scheduled;
        Priority = priority;
        ScheduledAt = scheduledAt;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        AssignedTo = assignedTo;
    }

    public void UpdateDetails(string subject, string? description = null, ActivityPriority? priority = null)
    {
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Description = description?.Trim();
        if (priority.HasValue)
            Priority = priority.Value;
    }

    public void Reschedule(DateTime newScheduledAt)
    {
        if (Status == ActivityStatus.Completed)
            throw new InvalidOperationException("Cannot reschedule completed activity");

        ScheduledAt = newScheduledAt;
        Status = ActivityStatus.Scheduled;
    }

    public void Assign(string assignedTo)
    {
        AssignedTo = assignedTo ?? throw new ArgumentNullException(nameof(assignedTo));
    }

    public void Start()
    {
        if (Status != ActivityStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled activities can be started");

        Status = ActivityStatus.InProgress;
    }

    public void Complete(string? outcome = null, string? nextAction = null, TimeSpan? duration = null)
    {
        Status = ActivityStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Outcome = outcome?.Trim();
        NextAction = nextAction?.Trim();
        Duration = duration;
    }

    public void Cancel()
    {
        if (Status == ActivityStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed activity");

        Status = ActivityStatus.Cancelled;
    }

    public bool IsOverdue => Status == ActivityStatus.Scheduled && ScheduledAt < DateTime.UtcNow;
    public bool IsCompleted => Status == ActivityStatus.Completed;
}

/// <summary>
/// Note entity for storing unstructured information
/// </summary>
public class Note : Entity
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public NoteType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public bool IsPrivate { get; private set; }
    public List<string> Tags { get; private set; }

    private Note() 
    { 
        Tags = new List<string>();
    } // EF Core

    public Note(
        Guid id,
        string title,
        string content,
        string createdBy,
        NoteType type = NoteType.General,
        bool isPrivate = false,
        List<string>? tags = null)
    {
        Id = id;
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Content = content?.Trim() ?? throw new ArgumentException("Content cannot be empty");
        Type = type;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        IsPrivate = isPrivate;
        Tags = tags?.Select(t => t.Trim().ToLowerInvariant()).ToList() ?? new List<string>();
    }

    public void Update(string title, string content, string modifiedBy, NoteType? type = null)
    {
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Content = content?.Trim() ?? throw new ArgumentException("Content cannot be empty");
        if (type.HasValue)
            Type = type.Value;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void AddTag(string tag)
    {
        var normalizedTag = tag?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalizedTag))
            return;

        if (!Tags.Contains(normalizedTag))
            Tags.Add(normalizedTag);
    }

    public void RemoveTag(string tag)
    {
        var normalizedTag = tag?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalizedTag))
            return;

        Tags.Remove(normalizedTag);
    }

    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
    }
}

/// <summary>
/// Communication entity for tracking all communications
/// </summary>
public class Communication : Entity
{
    public CommunicationType Type { get; private set; }
    public CommunicationDirection Direction { get; private set; }
    public string Subject { get; private set; }
    public string? Content { get; private set; }
    public DateTime CommunicatedAt { get; private set; }
    public string? From { get; private set; }
    public string? To { get; private set; }
    public CommunicationStatus Status { get; private set; }
    public string? ExternalId { get; private set; } // For email IDs, call logs, etc.
    public List<string> Attachments { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }

    private Communication() 
    { 
        Attachments = new List<string>();
    } // EF Core

    public Communication(
        Guid id,
        CommunicationType type,
        CommunicationDirection direction,
        string subject,
        DateTime communicatedAt,
        string createdBy,
        string? content = null,
        string? from = null,
        string? to = null,
        string? externalId = null)
    {
        Id = id;
        Type = type;
        Direction = direction;
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Content = content?.Trim();
        CommunicatedAt = communicatedAt;
        From = from?.Trim();
        To = to?.Trim();
        Status = CommunicationStatus.Completed;
        ExternalId = externalId?.Trim();
        Attachments = new List<string>();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
    }

    public void UpdateContent(string subject, string? content = null)
    {
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Content = content?.Trim();
    }

    public void UpdateStatus(CommunicationStatus status)
    {
        Status = status;
    }

    public void AddAttachment(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var trimmed = filePath.Trim();
        if (!Attachments.Contains(trimmed))
            Attachments.Add(trimmed);
    }

    public void RemoveAttachment(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        Attachments.Remove(filePath.Trim());
    }
}

/// <summary>
/// Document entity for file attachments
/// </summary>
public class Document : Entity
{
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public DocumentType Type { get; private set; }
    public string? Description { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string UploadedBy { get; private set; }
    public bool IsActive { get; private set; }
    public string? Tags { get; private set; }

    private Document() { } // EF Core

    public Document(
        Guid id,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        string uploadedBy,
        DocumentType type = DocumentType.General,
        string? description = null,
        string? tags = null)
    {
        Id = id;
        FileName = fileName?.Trim() ?? throw new ArgumentException("File name cannot be empty");
        FilePath = filePath?.Trim() ?? throw new ArgumentException("File path cannot be empty");
        ContentType = contentType?.Trim() ?? throw new ArgumentException("Content type cannot be empty");
        FileSize = fileSize > 0 ? fileSize : throw new ArgumentException("File size must be positive");
        Type = type;
        Description = description?.Trim();
        UploadedAt = DateTime.UtcNow;
        UploadedBy = uploadedBy ?? throw new ArgumentNullException(nameof(uploadedBy));
        IsActive = true;
        Tags = tags?.Trim();
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim();
    }

    public void UpdateTags(string tags)
    {
        Tags = tags?.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
