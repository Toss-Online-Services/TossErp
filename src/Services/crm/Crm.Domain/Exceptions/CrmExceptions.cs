namespace Crm.Domain.Exceptions;

public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(Guid customerId) 
        : base($"Customer with ID '{customerId}' was not found.")
    {
    }

    public CustomerNotFoundException(string email) 
        : base($"Customer with email '{email}' was not found.")
    {
    }
}

public class CustomerValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public CustomerValidationException(string message) 
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public CustomerValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}

public class DuplicateCustomerException : Exception
{
    public DuplicateCustomerException(string email) 
        : base($"A customer with email '{email}' already exists.")
    {
    }
}
