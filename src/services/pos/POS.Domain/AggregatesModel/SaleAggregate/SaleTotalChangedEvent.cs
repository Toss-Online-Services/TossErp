using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleTotalChangedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public decimal OldTotal { get; }
        public decimal NewTotal { get; }

        public SaleTotalChangedEvent(Sale sale, decimal oldTotal, decimal newTotal)
        {
            Sale = sale;
            OldTotal = oldTotal;
            NewTotal = newTotal;
        }
    }
} 
