namespace TossErp.Sales.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string key)
        : base($"Entity {entityName} with key {key} was not found")
    {
        EntityName = entityName;
        Key = key;
    }

    public string EntityName { get; }
    public string Key { get; }
}

/// <summary>
/// Exception thrown when business rules are violated
/// </summary>
public class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
    {
        Errors = errors;
    }

    public Dictionary<string, string[]> Errors { get; } = new();
}
