namespace Catalog.Application.DTOs;

public class ProductGiftCardDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public bool IsGiftCard { get; set; }
    public int GiftCardTypeId { get; set; }
    public decimal OverriddenGiftCardAmount { get; set; }
    public bool RequireOtherProducts { get; set; }
    public int RequiredProductId { get; set; }
    public bool AutomaticallyAddRequiredProducts { get; set; }
} 
