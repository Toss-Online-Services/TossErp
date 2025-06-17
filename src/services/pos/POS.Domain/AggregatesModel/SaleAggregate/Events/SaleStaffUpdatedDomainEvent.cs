using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleStaffUpdatedDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public Guid StaffId { get; }
        public string StaffName { get; }
        public SaleStaffUpdatedDomainEvent(Guid saleId, Guid staffId, string staffName)
        {
            SaleId = saleId;
            StaffId = staffId;
            StaffName = staffName;
        }
    }
} 
