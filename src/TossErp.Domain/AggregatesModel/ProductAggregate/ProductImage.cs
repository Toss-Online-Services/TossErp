using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.ProductAggregate;

public class ProductImage : Entity
{
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string AltText { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected ProductImage() 
    {
        ImageUrl = string.Empty;
        AltText = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public ProductImage(Guid productId, string imageUrl, string altText = "", bool isPrimary = false)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ImageUrl = imageUrl;
        AltText = altText;
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void UpdateAltText(string altText)
    {
        AltText = altText;
    }
} 
