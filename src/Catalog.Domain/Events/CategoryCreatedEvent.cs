using MediatR;

namespace Catalog.Domain.Events;

public class CategoryCreatedEvent : INotification
{
    public int CategoryId { get; }
    public string Name { get; }
    public int? ParentCategoryId { get; }

    public CategoryCreatedEvent(int categoryId, string name, int? parentCategoryId)
    {
        CategoryId = categoryId;
        Name = name;
        ParentCategoryId = parentCategoryId;
    }
} 
