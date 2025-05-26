namespace Catalog.Application.DTOs;

public class ProductRecurringDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int RecurringCycleLength { get; set; }
    public int RecurringCyclePeriodId { get; set; }
    public int RecurringTotalCycles { get; set; }
} 
