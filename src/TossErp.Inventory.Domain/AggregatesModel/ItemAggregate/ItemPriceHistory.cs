using System;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Exceptions;

namespace TossErp.Inventory.Domain.AggregatesModel.ItemAggregate
{
    public class ItemPriceHistory : Entity
    {
        public decimal Price { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public string? Reason { get; private set; }

        protected ItemPriceHistory() { }

        public ItemPriceHistory(decimal price, DateTime effectiveDate, string? reason = null)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            Price = price;
            EffectiveDate = effectiveDate;
            Reason = reason;
        }
    }
} 
