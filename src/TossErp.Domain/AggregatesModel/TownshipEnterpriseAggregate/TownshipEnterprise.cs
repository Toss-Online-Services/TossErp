using System;
using System.Collections.Generic;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Enums;
using TossErp.Domain.Events;
using TossErp.Domain.Exceptions;

namespace TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate
{
    public class TownshipEnterprise : Entity, IAggregateRoot
    {
        public string BusinessName { get; private set; } = string.Empty;
        public string? TradingName { get; private set; }
        public BusinessType BusinessType { get; private set; }
        public string? BusinessDescription { get; private set; }
        public Address Address { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsRegistered { get; private set; }
        public string? RegistrationNumber { get; private set; }
        public string? TaxNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime EstablishedDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<BusinessLicense> Licenses { get; private set; }
        public List<BusinessDocument> Documents { get; private set; }
        public List<BusinessContact> Contacts { get; private set; }

        protected TownshipEnterprise()
        {
            Address = new Address("", "", "", "");
            ContactInfo = new ContactInfo("");
            Licenses = new List<BusinessLicense>();
            Documents = new List<BusinessDocument>();
            Contacts = new List<BusinessContact>();
        }

        public TownshipEnterprise(
            string businessName,
            string? tradingName,
            BusinessType businessType,
            string? businessDescription,
            Address address,
            ContactInfo contactInfo,
            Guid ownerId,
            DateTime? establishedDate = null)
        {
            BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
            TradingName = tradingName;
            BusinessType = businessType;
            BusinessDescription = businessDescription;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            OwnerId = ownerId;
            IsRegistered = false;
            IsActive = true;
            EstablishedDate = establishedDate ?? DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Licenses = new List<BusinessLicense>();
            Documents = new List<BusinessDocument>();
            Contacts = new List<BusinessContact>();

            AddDomainEvent(new TownshipEnterpriseCreatedDomainEvent(this));
        }

        public void UpdateBusinessName(string businessName)
        {
            BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new TownshipEnterpriseUpdatedDomainEvent(this));
        }

        public void UpdateTradingName(string? tradingName)
        {
            TradingName = tradingName;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new TownshipEnterpriseUpdatedDomainEvent(this));
        }

        public void UpdateBusinessDescription(string? businessDescription)
        {
            BusinessDescription = businessDescription;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new TownshipEnterpriseUpdatedDomainEvent(this));
        }

        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new TownshipEnterpriseUpdatedDomainEvent(this));
        }

        public void UpdateContactInfo(ContactInfo contactInfo)
        {
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new TownshipEnterpriseUpdatedDomainEvent(this));
        }

        public void Register(string registrationNumber, string? taxNumber = null)
        {
            if (IsRegistered)
                throw new TossErpDomainException("Business is already registered.");

            RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
            TaxNumber = taxNumber;
            IsRegistered = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new TownshipEnterpriseRegisteredDomainEvent(this));
        }

        public void Activate()
        {
            if (IsActive)
                throw new TossErpDomainException("Business is already active.");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new TownshipEnterpriseActivatedDomainEvent(this));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new TossErpDomainException("Business is already inactive.");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new TownshipEnterpriseDeactivatedDomainEvent(this));
        }

        public BusinessLicense AddLicense(string licenseType, string licenseNumber, DateTime issueDate, DateTime expiryDate, string? issuingAuthority = null, string? notes = null)
        {
            var license = new BusinessLicense(licenseType, licenseNumber, issueDate, expiryDate, issuingAuthority, notes);
            Licenses.Add(license);

            AddDomainEvent(new BusinessLicenseAddedDomainEvent(this, license));
            return license;
        }

        public BusinessDocument AddDocument(string documentType, string documentName, string? documentUrl = null, string? description = null, long? fileSize = null, string? fileType = null)
        {
            var document = new BusinessDocument(documentType, documentName, documentUrl, description, fileSize, fileType);
            Documents.Add(document);

            AddDomainEvent(new BusinessDocumentAddedDomainEvent(this, document));
            return document;
        }

        public BusinessContact AddContact(string contactName, string contactNumber, string? emailAddress = null, string? relationship = null, string? notes = null)
        {
            var contact = new BusinessContact(contactName, contactNumber, emailAddress, relationship, notes);
            Contacts.Add(contact);

            AddDomainEvent(new BusinessContactAddedDomainEvent(this, contact));
            return contact;
        }

        public void RemoveContact(Guid contactId)
        {
            var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact != null)
            {
                Contacts.Remove(contact);
                AddDomainEvent(new BusinessContactRemovedDomainEvent(this, contactId));
            }
        }

        public bool HasValidLicense(string licenseType)
        {
            return Licenses.Any(l => l.LicenseType == licenseType && l.IsValid);
        }

        public bool IsInTownship(string townshipName)
        {
            return Address.Township.Equals(townshipName, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsInProvince(string provinceName)
        {
            return Address.Province.Equals(provinceName, StringComparison.OrdinalIgnoreCase);
        }
    }
} 
