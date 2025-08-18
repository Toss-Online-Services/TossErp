namespace TossErp.ErrorModel;

/// <summary>
/// Base class for domain-specific exceptions
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Error code for categorizing the exception
    /// </summary>
    public string ErrorCode { get; }

    protected DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string errorCode, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string errorCode, string message) 
        : base(errorCode, message)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id) 
        : base("ENTITY_NOT_FOUND", $"{entityName} with ID '{id}' was not found.")
    {
    }

    public EntityNotFoundException(string entityName, string field, object value) 
        : base("ENTITY_NOT_FOUND", $"{entityName} with {field} '{value}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when there's a conflict with existing data
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) 
        : base("CONFLICT", message)
    {
    }

    public ConflictException(string errorCode, string message) 
        : base(errorCode, message)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) 
        : base("VALIDATION_FAILED", message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("VALIDATION_FAILED", "One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}
