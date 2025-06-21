using System;
using MediatR;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;

namespace TossErp.Domain.Events
{
    public class TownshipEnterpriseCreatedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }

        public TownshipEnterpriseCreatedDomainEvent(TownshipEnterprise townshipEnterprise)
        {
            TownshipEnterprise = townshipEnterprise;
        }
    }

    public class TownshipEnterpriseUpdatedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }

        public TownshipEnterpriseUpdatedDomainEvent(TownshipEnterprise townshipEnterprise)
        {
            TownshipEnterprise = townshipEnterprise;
        }
    }

    public class TownshipEnterpriseRegisteredDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }

        public TownshipEnterpriseRegisteredDomainEvent(TownshipEnterprise townshipEnterprise)
        {
            TownshipEnterprise = townshipEnterprise;
        }
    }

    public class TownshipEnterpriseActivatedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }

        public TownshipEnterpriseActivatedDomainEvent(TownshipEnterprise townshipEnterprise)
        {
            TownshipEnterprise = townshipEnterprise;
        }
    }

    public class TownshipEnterpriseDeactivatedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }

        public TownshipEnterpriseDeactivatedDomainEvent(TownshipEnterprise townshipEnterprise)
        {
            TownshipEnterprise = townshipEnterprise;
        }
    }

    public class BusinessLicenseAddedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }
        public BusinessLicense License { get; }

        public BusinessLicenseAddedDomainEvent(TownshipEnterprise townshipEnterprise, BusinessLicense license)
        {
            TownshipEnterprise = townshipEnterprise;
            License = license;
        }
    }

    public class BusinessDocumentAddedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }
        public BusinessDocument Document { get; }

        public BusinessDocumentAddedDomainEvent(TownshipEnterprise townshipEnterprise, BusinessDocument document)
        {
            TownshipEnterprise = townshipEnterprise;
            Document = document;
        }
    }

    public class BusinessContactAddedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }
        public BusinessContact Contact { get; }

        public BusinessContactAddedDomainEvent(TownshipEnterprise townshipEnterprise, BusinessContact contact)
        {
            TownshipEnterprise = townshipEnterprise;
            Contact = contact;
        }
    }

    public class BusinessContactRemovedDomainEvent : INotification
    {
        public TownshipEnterprise TownshipEnterprise { get; }
        public Guid ContactId { get; }

        public BusinessContactRemovedDomainEvent(TownshipEnterprise townshipEnterprise, Guid contactId)
        {
            TownshipEnterprise = townshipEnterprise;
            ContactId = contactId;
        }
    }
} 
