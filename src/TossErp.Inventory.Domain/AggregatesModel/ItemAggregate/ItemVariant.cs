using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Inventory.Domain.AggregatesModel.ItemAggregate
{
    public class ItemVariant : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string? VariantCode { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected ItemVariant() { }

        public ItemVariant(string name, string? variantCode = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VariantCode = variantCode;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
} 
