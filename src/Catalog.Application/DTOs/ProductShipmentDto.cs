namespace Catalog.Application.DTOs;

public class ProductShipmentDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public bool IsShipEnabled { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool ShipSeparately { get; set; }
    public decimal AdditionalShippingCharge { get; set; }
    public int DeliveryDateId { get; set; }
} 
