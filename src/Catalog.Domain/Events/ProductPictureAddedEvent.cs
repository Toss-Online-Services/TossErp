using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductPictureAddedEvent : INotification
{
    public int ProductId { get; }
    public int PictureId { get; }
    public int DisplayOrder { get; }

    public ProductPictureAddedEvent(int productId, int pictureId, int displayOrder)
    {
        ProductId = productId;
        PictureId = pictureId;
        DisplayOrder = displayOrder;
    }
} 
