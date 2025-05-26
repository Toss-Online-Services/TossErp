namespace Ordering.API.Application.Queries;

public class OrderViewModel
{
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; }
}

public class OrderItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
    public string PictureUrl { get; set; }
}

public class OrderSummary
{
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
}

public record CardType
{
    public int Id { get; init; }
    public string Name { get; init; }
}
