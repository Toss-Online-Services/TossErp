using POS.Domain.Repositories;
using POS.Domain.Common;
using POS.Domain.Exceptions;
using POS.Domain.Specifications;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Services
{
    public class CustomerDomainService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerSegmentRepository _segmentRepository;
        private readonly ICustomerLoyaltyProgramRepository _loyaltyProgramRepository;

        public CustomerDomainService(
            ICustomerRepository customerRepository,
            ICustomerSegmentRepository segmentRepository,
            ICustomerLoyaltyProgramRepository loyaltyProgramRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _segmentRepository = segmentRepository ?? throw new ArgumentNullException(nameof(segmentRepository));
            _loyaltyProgramRepository = loyaltyProgramRepository ?? throw new ArgumentNullException(nameof(loyaltyProgramRepository));
        }

        public async Task<Customer> CreateCustomerAsync(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string? preferredLanguage = null,
            string? preferredCurrency = null,
            CancellationToken cancellationToken = default)
        {
            if (await _customerRepository.ExistsByEmailAsync(email, cancellationToken))
                throw new DomainException($"Customer with email {email} already exists");

            if (await _customerRepository.ExistsByPhoneAsync(phoneNumber, cancellationToken))
                throw new DomainException($"Customer with phone number {phoneNumber} already exists");

            var customer = new Customer(firstName, lastName, email, phoneNumber);
            
            if (preferredLanguage != null || preferredCurrency != null)
            {
                customer.UpdatePreferences(
                    preferredLanguage: preferredLanguage ?? "en",
                    preferredCurrency: preferredCurrency ?? "USD"
                );
            }

            return await _customerRepository.AddAsync(customer, cancellationToken);
        }

        public async Task<CustomerSegment> CreateSegmentAsync(
            string name,
            string description,
            string createdBy,
            string? criteria = null,
            decimal? minimumSpend = null,
            int? minimumOrders = null,
            CancellationToken cancellationToken = default)
        {
            if (await _segmentRepository.ExistsByNameAsync(name, cancellationToken))
                throw new DomainException($"Segment with name {name} already exists");

            var segment = new CustomerSegment(
                name,
                description,
                createdBy,
                criteria,
                minimumSpend,
                minimumOrders);

            return await _segmentRepository.AddAsync(segment, cancellationToken);
        }

        public async Task<CustomerLoyaltyProgram> EnrollInLoyaltyProgramAsync(
            Guid customerId,
            string programName,
            string membershipNumber,
            string membershipTier,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
                throw new DomainException($"Customer with ID {customerId} not found");

            if (await _loyaltyProgramRepository.ExistsByMembershipNumberAsync(membershipNumber, cancellationToken))
                throw new DomainException($"Loyalty program with membership number {membershipNumber} already exists");

            var program = new CustomerLoyaltyProgram(
                programName,
                membershipNumber,
                membershipTier);

            return await _loyaltyProgramRepository.AddAsync(program, cancellationToken);
        }

        public async Task<IEnumerable<Customer>> GetCustomersBySegmentAsync(
            Guid segmentId,
            CancellationToken cancellationToken = default)
        {
            var segment = await _segmentRepository.GetByIdAsync(segmentId, cancellationToken);
            if (segment == null)
                throw new DomainException($"Segment with ID {segmentId} not found");

            return await _customerRepository.GetBySegmentAsync(segmentId, cancellationToken);
        }

        public async Task<IEnumerable<Customer>> GetHighValueCustomersAsync(
            CancellationToken cancellationToken = default)
        {
            return await _customerRepository.GetHighValueCustomersAsync(cancellationToken);
        }

        public async Task<IEnumerable<Customer>> GetLoyalCustomersAsync(
            CancellationToken cancellationToken = default)
        {
            return await _customerRepository.GetLoyalCustomersAsync(cancellationToken);
        }

        public async Task<IEnumerable<Customer>> GetAtRiskCustomersAsync(
            CancellationToken cancellationToken = default)
        {
            return await _customerRepository.GetAtRiskCustomersAsync(cancellationToken);
        }

        public async Task AddPointsToLoyaltyProgramAsync(
            Guid programId,
            decimal points,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var program = await _loyaltyProgramRepository.GetByIdAsync(programId, cancellationToken);
            if (program == null)
                throw new DomainException($"Loyalty program with ID {programId} not found");

            await _loyaltyProgramRepository.AddPointsAsync(programId, points, reason, cancellationToken);
        }

        public async Task RedeemPointsFromLoyaltyProgramAsync(
            Guid programId,
            decimal points,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var program = await _loyaltyProgramRepository.GetByIdAsync(programId, cancellationToken);
            if (program == null)
                throw new DomainException($"Loyalty program with ID {programId} not found");

            var currentBalance = await _loyaltyProgramRepository.GetPointsBalanceAsync(programId, cancellationToken);
            if (currentBalance < points)
                throw new DomainException($"Insufficient points balance. Current balance: {currentBalance}, Requested: {points}");

            await _loyaltyProgramRepository.RedeemPointsAsync(programId, points, reason, cancellationToken);
        }
    }
} 
