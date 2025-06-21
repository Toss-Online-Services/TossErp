using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Enums;
using TossErp.Domain.Events;
using TossErp.Domain.Exceptions;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class Cooperative : Entity, IAggregateRoot
    {
        public string CooperativeName { get; private set; } = string.Empty;
        public string? TradingName { get; private set; }
        public CooperativeType CooperativeType { get; private set; }
        public string? Description { get; private set; }
        public Address Address { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public BankAccountDetails? BankAccountDetails { get; private set; }
        public decimal InitialShareValue { get; private set; }
        public int MinimumMembers { get; private set; }
        public int MaximumMembers { get; private set; }
        public bool IsRegistered { get; private set; }
        public string? RegistrationNumber { get; private set; }
        public string? TaxNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime EstablishedDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<CooperativeMember> Members { get; private set; }
        public List<CooperativeDocument> Documents { get; private set; }
        public List<CooperativeMeeting> Meetings { get; private set; }

        protected Cooperative()
        {
            Address = new Address("", "", "", "");
            ContactInfo = new ContactInfo("");
            Members = new List<CooperativeMember>();
            Documents = new List<CooperativeDocument>();
            Meetings = new List<CooperativeMeeting>();
        }

        public Cooperative(
            string cooperativeName,
            string? tradingName,
            CooperativeType cooperativeType,
            string? description,
            Address address,
            ContactInfo contactInfo,
            BankAccountDetails? bankAccountDetails,
            decimal initialShareValue,
            int minimumMembers,
            int maximumMembers,
            DateTime? establishedDate = null)
        {
            CooperativeName = cooperativeName ?? throw new ArgumentNullException(nameof(cooperativeName));
            TradingName = tradingName;
            CooperativeType = cooperativeType;
            Description = description;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            BankAccountDetails = bankAccountDetails;
            InitialShareValue = initialShareValue;
            MinimumMembers = minimumMembers;
            MaximumMembers = maximumMembers;
            IsRegistered = false;
            IsActive = true;
            EstablishedDate = establishedDate ?? DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Members = new List<CooperativeMember>();
            Documents = new List<CooperativeDocument>();
            Meetings = new List<CooperativeMeeting>();

            AddDomainEvent(new CooperativeCreatedDomainEvent(this));
        }

        public void UpdateTradingName(string? tradingName)
        {
            TradingName = tradingName;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateContactInfo(ContactInfo contactInfo)
        {
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateBusinessRegistrationNumber(string registrationNumber)
        {
            RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateTaxRegistrationNumber(string? taxNumber)
        {
            TaxNumber = taxNumber;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void UpdateBankAccountDetails(BankAccountDetails? bankAccountDetails)
        {
            BankAccountDetails = bankAccountDetails;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CooperativeUpdatedDomainEvent(this));
        }

        public void Register()
        {
            if (IsRegistered)
                throw new TossErpDomainException("Cooperative is already registered.");

            IsRegistered = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CooperativeRegisteredDomainEvent(this));
        }

        public void Activate()
        {
            if (IsActive)
                throw new TossErpDomainException("Cooperative is already active.");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CooperativeActivatedDomainEvent(this));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new TossErpDomainException("Cooperative is already inactive.");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CooperativeDeactivatedDomainEvent(this));
        }

        public CooperativeMember AddMember(
            string firstName,
            string lastName,
            string idNumber,
            string phoneNumber,
            string email,
            decimal shareValue,
            string role)
        {
            if (Members.Any(m => m.IdNumber == idNumber))
                throw new TossErpDomainException($"Member with ID number '{idNumber}' is already a member of this cooperative.");

            var member = new CooperativeMember(firstName, lastName, idNumber, phoneNumber, email, shareValue, role);
            Members.Add(member);

            AddDomainEvent(new CooperativeMemberAddedDomainEvent(this, member));
            return member;
        }

        public CooperativeMember UpdateMember(
            Guid memberId,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            string address)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
                throw new TossErpDomainException($"Member with ID '{memberId}' not found in cooperative.");

            member.UpdateDetails(firstName, lastName, phoneNumber, email, address);
            AddDomainEvent(new CooperativeMemberUpdatedDomainEvent(this, member));
            return member;
        }

        public void RemoveMember(Guid memberId)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
                throw new TossErpDomainException($"Member with ID '{memberId}' not found in cooperative.");

            member.ExitCooperative(DateTime.UtcNow);
            AddDomainEvent(new CooperativeMemberRemovedDomainEvent(this, memberId, null));
        }

        public CooperativeDocument AddDocument(
            string documentType,
            string documentNumber,
            DateTime issueDate,
            DateTime? expiryDate,
            string? documentUrl)
        {
            var document = new CooperativeDocument(documentType, documentNumber, issueDate, expiryDate, documentUrl);
            Documents.Add(document);

            AddDomainEvent(new CooperativeDocumentAddedDomainEvent(this, document));
            return document;
        }

        public CooperativeMeeting ScheduleMeeting(
            string meetingType,
            string subject,
            string description,
            DateTime scheduledDate,
            string? location = null)
        {
            var meeting = new CooperativeMeeting(meetingType, subject, description, scheduledDate, location);
            Meetings.Add(meeting);

            AddDomainEvent(new CooperativeMeetingScheduledDomainEvent(this, meeting));
            return meeting;
        }

        public int GetActiveMemberCount()
        {
            return Members.Count(m => m.IsActive);
        }

        public decimal GetTotalShareValue()
        {
            return Members.Where(m => m.IsActive).Sum(m => m.ShareValue);
        }

        public bool HasMinimumMembers(int minimumMembers = 5)
        {
            return GetActiveMemberCount() >= minimumMembers;
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
