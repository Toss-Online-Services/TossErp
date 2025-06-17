using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.CustomerAggregate.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class Customer : AggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public Address? Address { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? Notes { get; private set; }
        public CustomerPreferences Preferences { get; private set; }

        // New properties for advanced customer management
        public decimal CreditLimit { get; private set; }
        public int PaymentTerms { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime? LastPurchaseDate { get; private set; }
        public decimal TotalPurchases { get; private set; }
        public int PurchaseCount { get; private set; }
        public CustomerType CustomerType { get; private set; }
        public List<PriceList> PriceLists { get; private set; }
        public LoyaltyProgram? LoyaltyProgram { get; private set; }
        public List<CustomerContact> Contacts { get; private set; }
        public List<CustomerDocument> Documents { get; private set; }
        public List<CustomerNote> CustomerNotes { get; private set; }

        private Customer()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            PriceLists = new List<PriceList>();
            Contacts = new List<CustomerContact>();
            Documents = new List<CustomerDocument>();
            CustomerNotes = new List<CustomerNote>();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Preferences = new CustomerPreferences();
        }

        public Customer(string firstName, string lastName, string email, string phoneNumber) : this()
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            CustomerType = CustomerType.Regular;

            AddDomainEvent(new CustomerCreatedDomainEvent(Id, firstName, lastName, email));
        }

        public void UpdateContactInfo(string firstName, string lastName, string email, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerContactInfoUpdatedDomainEvent(Id, firstName, lastName, email, phoneNumber, LastModifiedAt.Value));
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerAddressUpdatedDomainEvent(
                Id,
                Guid.NewGuid(),
                address.Street,
                address.City,
                address.State,
                address.Country,
                address.ZipCode,
                "Primary",
                LastModifiedAt.Value));
        }

        public void SetCreditLimit(decimal creditLimit)
        {
            if (creditLimit < 0)
                throw new DomainException("Credit limit cannot be negative");

            var oldCreditLimit = CreditLimit;
            CreditLimit = creditLimit;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerCreditLimitUpdatedDomainEvent(Id, oldCreditLimit, creditLimit, "System", LastModifiedAt.Value));
        }

        public void SetPaymentTerms(int days)
        {
            if (days < 0)
                throw new DomainException("Payment terms cannot be negative");

            var oldPaymentTerms = PaymentTerms;
            PaymentTerms = days;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPaymentTermsUpdatedDomainEvent(Id, oldPaymentTerms.ToString(), days.ToString(), "System", LastModifiedAt.Value));
        }

        public void UpdateBalance(decimal amount)
        {
            var oldBalance = Balance;
            Balance += amount;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerBalanceUpdatedDomainEvent(Id, oldBalance, Balance, "System", "USD", LastModifiedAt.Value));
        }

        public void RecordPurchase(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Purchase amount must be greater than zero");

            LastPurchaseDate = DateTime.UtcNow;
            TotalPurchases += amount;
            PurchaseCount++;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPurchaseRecordedDomainEvent(Id, Guid.NewGuid(), amount, "USD", LastModifiedAt.Value));
        }

        public void SetCustomerType(CustomerType type)
        {
            var oldType = CustomerType;
            CustomerType = type;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerTypeUpdatedDomainEvent(Id, oldType.ToString(), type.ToString(), "System", LastModifiedAt.Value));
        }

        public void AddPriceList(PriceList priceList)
        {
            if (PriceLists.Any(p => p.Id == priceList.Id))
                throw new DomainException("Price list already exists");

            PriceLists.Add(priceList);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPriceListAddedDomainEvent(Id, priceList.Id, priceList.Name, false, "System", LastModifiedAt.Value));
        }

        public void RemovePriceList(Guid priceListId)
        {
            var priceList = PriceLists.FirstOrDefault(p => p.Id == priceListId);
            if (priceList == null)
                throw new DomainException("Price list not found");

            PriceLists.Remove(priceList);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPriceListRemovedDomainEvent(Id, priceListId, priceList.Name, "System", LastModifiedAt.Value));
        }

        public void EnrollInLoyaltyProgram(LoyaltyProgram program)
        {
            if (LoyaltyProgram != null)
                throw new DomainException("Customer is already enrolled in a loyalty program");

            LoyaltyProgram = program;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerEnrolledInLoyaltyProgramDomainEvent(Id, program.Id, program.Name, program.MembershipNumber, program.MembershipTier, "System", LastModifiedAt.Value));
        }

        public void AddContact(CustomerContact contact)
        {
            if (Contacts.Any(c => c.Email == contact.Email))
                throw new DomainException("Contact with this email already exists");

            Contacts.Add(contact);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerContactAddedDomainEvent(
                Id,
                contact.Id,
                contact.FirstName,
                contact.LastName,
                contact.Email,
                contact.PhoneNumber,
                contact.Title ?? "General",
                "System",
                LastModifiedAt.Value));
        }

        public void RemoveContact(Guid contactId)
        {
            var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null)
                throw new DomainException("Contact not found");

            Contacts.Remove(contact);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerContactRemovedDomainEvent(Id, contactId, contact.FirstName, contact.LastName, "System", LastModifiedAt.Value));
        }

        public void AddDocument(CustomerDocument document)
        {
            if (Documents.Any(d => d.Name == document.Name))
                throw new DomainException("Document with this name already exists");

            Documents.Add(document);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerDocumentAddedDomainEvent(
                Id,
                document.Id,
                document.Name,
                document.FileType,
                document.FilePath,
                document.UploadedBy,
                document.Description ?? string.Empty,
                LastModifiedAt.Value
            ));
        }

        public void RemoveDocument(Guid documentId)
        {
            var document = Documents.FirstOrDefault(d => d.Id == documentId);
            if (document == null)
                throw new DomainException("Document not found");

            Documents.Remove(document);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerDocumentRemovedDomainEvent(Id, documentId, document.Name, document.FileType, "System", LastModifiedAt.Value));
        }

        public void AddNote(string note, string createdBy)
        {
            var customerNote = new CustomerNote(note, createdBy);
            CustomerNotes.Add(customerNote);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerNoteAddedDomainEvent(
                Id,
                customerNote.Id,
                "Customer Note",
                note,
                "General",
                createdBy,
                LastModifiedAt.Value));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Customer is already deactivated");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerDeactivatedDomainEvent(Id, LastModifiedAt.Value));
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new DomainException("Customer is already active");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerReactivatedDomainEvent(Id, LastModifiedAt.Value));
        }

        public void UpdatePreferences(
            bool? receiveEmailNotifications = null,
            bool? receiveSMSNotifications = null,
            bool? receivePostalMail = null,
            string? preferredLanguage = null,
            string? preferredCurrency = null,
            string? preferredPaymentMethod = null,
            string? preferredShippingMethod = null,
            bool? optInMarketing = null,
            bool? optInThirdParty = null,
            string? dietaryRestrictions = null,
            string? specialInstructions = null)
        {
            var oldPreferences = Preferences;
            
            if (receiveEmailNotifications.HasValue)
                Preferences.UpdateNotificationPreferences(emailNotifications: receiveEmailNotifications);
            if (receiveSMSNotifications.HasValue)
                Preferences.UpdateNotificationPreferences(smsNotifications: receiveSMSNotifications);
            if (receivePostalMail.HasValue)
                Preferences.UpdateNotificationPreferences(postalMail: receivePostalMail);
            if (preferredLanguage != null)
                Preferences.UpdateLanguage(preferredLanguage);
            if (preferredCurrency != null)
                Preferences.UpdateCurrency(preferredCurrency);
            if (preferredPaymentMethod != null)
                Preferences.SetPreferredPaymentMethod(preferredPaymentMethod);
            if (preferredShippingMethod != null)
                Preferences.SetPreferredShippingMethod(preferredShippingMethod);
            if (optInMarketing.HasValue || optInThirdParty.HasValue)
                Preferences.UpdateMarketingPreferences(optInMarketing, optInThirdParty);
            if (dietaryRestrictions != null)
                Preferences.SetDietaryRestrictions(dietaryRestrictions);
            if (specialInstructions != null)
                Preferences.SetSpecialInstructions(specialInstructions);

            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPreferencesUpdatedDomainEvent(
                Id,
                oldPreferences.ReceiveEmailNotifications,
                oldPreferences.ReceiveSMSNotifications,
                oldPreferences.ReceivePostalMail,
                oldPreferences.PreferredLanguage,
                oldPreferences.PreferredCurrency,
                oldPreferences.PreferredPaymentMethod ?? string.Empty,
                oldPreferences.DietaryRestrictions ?? string.Empty,
                LastModifiedAt.Value));
        }

        public string FullName => $"{FirstName} {LastName}";
        public bool HasCreditAvailable => Balance < CreditLimit;
        public decimal AvailableCredit => CreditLimit - Balance;
        public bool IsOverdue => Balance > 0 && LastPurchaseDate.HasValue && 
            (DateTime.UtcNow - LastPurchaseDate.Value).TotalDays > PaymentTerms;
    }
} 
