using MediatR;

namespace Catalog.Domain.Events;

public class CategoryUpdatedEvent : INotification
{
    public int CategoryId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public int? OldParentCategoryId { get; }
    public int? NewParentCategoryId { get; }

    public CategoryUpdatedEvent(int categoryId, string oldName, string newName, int? oldParentCategoryId, int? newParentCategoryId)
    {
        CategoryId = categoryId;
        OldName = oldName;
        NewName = newName;
        OldParentCategoryId = oldParentCategoryId;
        NewParentCategoryId = newParentCategoryId;
    }
} 
