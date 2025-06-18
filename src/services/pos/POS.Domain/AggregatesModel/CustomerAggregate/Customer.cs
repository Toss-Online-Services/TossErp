using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.CustomerAggregate.Events;
using POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class Customer : AggregateRoot
    {
        private CustomerName _name;
        private ContactInfo _contactInfo;
        private CreditLimit _creditLimit;
        private PaymentTerms _paymentTerms;
        private CustomerBalance _balance;
        private readonly List<PriceList> _priceLists = new();
        private readonly List<CustomerContact> _contacts = new();
        private readonly List<CustomerDocument> _documents = new();
        private readonly List<CustomerNote> _customerNotes = new();

        public string FirstName => _name.FirstName;
        public string LastName => _name.LastName;
        public string Email => _contactInfo.Email;
        public string PhoneNumber => _contactInfo.PhoneNumber;
        public Address? Address { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? Notes { get; private set; }
        public CustomerPreferences Preferences { get; private set; }

        // New properties for advanced customer management
        public decimal CreditLimit => _creditLimit.Amount;
        public int PaymentTerms => _paymentTerms.Days;
        public decimal Balance => _balance.Amount;
        public DateTime? LastPurchaseDate { get; private set; }
        public decimal TotalPurchases { get; private set; }
        public int PurchaseCount { get; private set; }
        public CustomerType CustomerType { get; private set; }
        public IReadOnlyCollection<PriceList> PriceLists => _priceLists.AsReadOnly();
        public LoyaltyProgram? LoyaltyProgram { get; private set; }
        public IReadOnlyCollection<CustomerContact> Contacts => _contacts.AsReadOnly();
        public IReadOnlyCollection<CustomerDocument> Documents => _documents.AsReadOnly();
        public IReadOnlyCollection<CustomerNote> CustomerNotes => _customerNotes.AsReadOnly();

        private Customer()
        {
            _name = new CustomerName(string.Empty, string.Empty);
            _contactInfo = new ContactInfo(string.Empty, string.Empty);
            _creditLimit = new CreditLimit(0);
            _paymentTerms = new PaymentTerms(30, "Net 30");
            _balance = new CustomerBalance(0);
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Preferences = new CustomerPreferences();
        }

        public Customer(string firstName, string lastName, string email, string phoneNumber) : this()
        {
            _name = CustomerName.Create(firstName, lastName);
            _contactInfo = ContactInfo.Create(email, phoneNumber);
            CustomerType = CustomerType.Regular;

            AddDomainEvent(new CustomerCreatedDomainEvent(Id, firstName, lastName, email));
        }

        public void UpdateContactInfo(string firstName, string lastName, string email, string phoneNumber)
        {
            var newName = CustomerName.Create(firstName, lastName);
            var newContactInfo = ContactInfo.Create(email, phoneNumber);

            _name = newName;
            _contactInfo = newContactInfo;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerContactInfoUpdatedDomainEvent(
                Id, 
                firstName, 
                lastName, 
                email, 
                phoneNumber, 
                LastModifiedAt.Value));
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
            var oldCreditLimit = _creditLimit;
            _creditLimit = new CreditLimit(creditLimit);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerCreditLimitUpdatedDomainEvent(
                Id, 
                oldCreditLimit.Amount, 
                creditLimit, 
                "System", 
                LastModifiedAt.Value));
        }

        public void SetPaymentTerms(int days, string description)
        {
            var oldPaymentTerms = _paymentTerms;
            _paymentTerms = new PaymentTerms(days, description);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPaymentTermsUpdatedDomainEvent(
                Id, 
                oldPaymentTerms.Days.ToString(), 
                days.ToString(), 
                "System", 
                LastModifiedAt.Value));
        }

        public void UpdateBalance(decimal amount)
        {
            var oldBalance = _balance;
            _balance = amount > 0 ? _balance.Add(amount) : _balance.Subtract(Math.Abs(amount));
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerBalanceUpdatedDomainEvent(
                Id, 
                oldBalance.Amount, 
                _balance.Amount, 
                "System", 
                _balance.Currency, 
                LastModifiedAt.Value));
        }

        public void RecordPurchase(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Purchase amount must be greater than zero");

            LastPurchaseDate = DateTime.UtcNow;
            TotalPurchases += amount;
            PurchaseCount++;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPurchaseRecordedDomainEvent(
                Id, 
                Guid.NewGuid(), 
                amount, 
                _balance.Currency, 
                LastModifiedAt.Value));
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
            if (_priceLists.Any(p => p.Id == priceList.Id))
                throw new DomainException("Price list already exists");

            _priceLists.Add(priceList);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPriceListAddedDomainEvent(
                Id, 
                priceList.Id, 
                priceList.Name, 
                false, 
                "System", 
                LastModifiedAt.Value));
        }

        public void RemovePriceList(Guid priceListId)
        {
            var priceList = _priceLists.FirstOrDefault(p => p.Id == priceListId);
            if (priceList == null)
                throw new DomainException("Price list not found");

            _priceLists.Remove(priceList);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerPriceListRemovedDomainEvent(
                Id, 
                priceListId, 
                priceList.Name, 
                "System", 
                LastModifiedAt.Value));
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
            if (_contacts.Any(c => c.Email == contact.Email))
                throw new DomainException("Contact with this email already exists");

            _contacts.Add(contact);
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
            var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null)
                throw new DomainException("Contact not found");

            _contacts.Remove(contact);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerContactRemovedDomainEvent(
                Id, 
                contactId, 
                contact.FirstName, 
                contact.LastName, 
                "System", 
                LastModifiedAt.Value));
        }

        public void AddDocument(CustomerDocument document)
        {
            if (_documents.Any(d => d.Name == document.Name))
                throw new DomainException("Document with this name already exists");

            _documents.Add(document);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerDocumentAddedDomainEvent(
                Id,
                document.Id,
                document.Name,
                document.FileType,
                document.FilePath,
                document.UploadedBy,
                document.Description ?? string.Empty,
                LastModifiedAt.Value));
        }

        public void RemoveDocument(Guid documentId)
        {
            var document = _documents.FirstOrDefault(d => d.Id == documentId);
            if (document == null)
                throw new DomainException("Document not found");

            _documents.Remove(document);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerDocumentRemovedDomainEvent(
                Id,
                documentId,
                document.FileType,
                document.Name,
                "System",
                LastModifiedAt.Value));
        }

        public void AddNote(string note, string createdBy)
        {
            var customerNote = new CustomerNote(note, createdBy);
            _customerNotes.Add(customerNote);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerNoteAddedDomainEvent(
                Id, 
                customerNote.Id, 
                note, 
                createdBy, 
                "System", 
                "General", 
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

        public string FullName => _name.FullName;
        public bool HasCreditAvailable => _creditLimit.HasAvailableCredit(_balance.Amount);
        public decimal AvailableCredit => _creditLimit.GetAvailableCredit(_balance.Amount);
        public bool IsOverdue => _balance.IsPositive && LastPurchaseDate.HasValue && 
            _paymentTerms.IsOverdue(LastPurchaseDate.Value);

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
} 
