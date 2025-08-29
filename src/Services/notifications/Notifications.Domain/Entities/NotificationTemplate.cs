using TossErp.Shared.SeedWork;

namespace Notifications.Domain.Entities;

public class NotificationTemplate : Entity
{
    public override Guid Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public string Language { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public List<string> RequiredVariables { get; private set; }
    public List<string> OptionalVariables { get; private set; }

    public NotificationTemplate(
        string name,
        string description,
        NotificationType type,
        NotificationChannel channel,
        string subject,
        string body,
        string language,
        string createdBy,
        List<string> requiredVariables,
        List<string> optionalVariables)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Type = type;
        Channel = channel;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        Language = language ?? throw new ArgumentNullException(nameof(language));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        RequiredVariables = requiredVariables ?? new List<string>();
        OptionalVariables = optionalVariables ?? new List<string>();
    }

    private NotificationTemplate()
    {
        Name = string.Empty;
        Description = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
        Language = string.Empty;
        CreatedBy = string.Empty;
        RequiredVariables = new List<string>();
        OptionalVariables = new List<string>();
    }

    public void Update(
        string description,
        string subject,
        string body,
        List<string> requiredVariables,
        List<string> optionalVariables,
        string updatedBy)
    {
        Description = description ?? string.Empty;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        RequiredVariables = requiredVariables ?? new List<string>();
        OptionalVariables = optionalVariables ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy ?? throw new ArgumentNullException(nameof(updatedBy));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public string RenderSubject(Dictionary<string, object> variables)
    {
        return RenderTemplate(Subject, variables);
    }

    public string RenderBody(Dictionary<string, object> variables)
    {
        return RenderTemplate(Body, variables);
    }

    private string RenderTemplate(string template, Dictionary<string, object> variables)
    {
        var result = template;
        
        foreach (var variable in variables)
        {
            result = result.Replace($"{{{{{variable.Key}}}}}", variable.Value?.ToString() ?? string.Empty);
        }

        return result;
    }

    public bool ValidateVariables(Dictionary<string, object> variables)
    {
        return RequiredVariables.All(v => variables.ContainsKey(v));
    }

    public List<string> GetMissingVariables(Dictionary<string, object> variables)
    {
        return RequiredVariables.Where(v => !variables.ContainsKey(v)).ToList();
    }
}
