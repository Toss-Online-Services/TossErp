namespace Crm.Domain.Entities;

public class Opportunity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string? Stage { get; set; }
}
