using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Subscription entity for managing recurring billing in TOSS ERP
/// Designed for South African township SMME context with community features
/// </summary>
[Table("Subscriptions")]
public class Subscription : Entity
{
    [Required]
    [StringLength(50)]
    public string SubscriptionNumber { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    [StringLength(200)]
    public string? CustomerName { get; private set; }

    [Required]
    [StringLength(200)]
    public string PlanName { get; private set; } = string.Empty;

    [StringLength(500)]
    public string? PlanDescription { get; private set; }

    public Money MonthlyAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money AnnualAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public BillingFrequency BillingFrequency { get; private set; } = BillingFrequency.Monthly;

    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;

    [StringLength(3)]
    public string Currency { get; private set; } = "ZAR";

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public DateTime? CancelledDate { get; private set; }

    public DateTime NextBillingDate { get; private set; }

    public DateTime? LastBillingDate { get; private set; }

    public int BillingCycle { get; private set; } = 1; // How many periods (1 = monthly, 3 = quarterly, etc.)

    // Trial information
    public bool HasTrialPeriod { get; private set; } = false;

    public DateTime? TrialStartDate { get; private set; }

    public DateTime? TrialEndDate { get; private set; }

    public int? TrialDays { get; private set; }

    // Payment information
    public PaymentMethod PreferredPaymentMethod { get; private set; } = PaymentMethod.BankTransfer;

    [StringLength(100)]
    public string? PaymentMethodDetails { get; private set; }

    public Money? SetupFee { get; private set; }

    public bool SetupFeePaid { get; private set; } = false;

    // Community/Township specific features
    public bool IsCommunitySubscription { get; private set; } = false;

    [StringLength(200)]
    public string? CommunityGroupName { get; private set; }

    public int? CommunityMembers { get; private set; }

    public Money? CommunityDiscount { get; private set; }

    public bool AllowSharedAccess { get; private set; } = false;

    // Usage tracking for usage-based billing
    public decimal? UsageLimit { get; private set; }

    public decimal? CurrentUsage { get; private set; } = 0;

    [StringLength(50)]
    public string? UsageUnit { get; private set; } // e.g., "transactions", "users", "GB"

    public Money? OverageRate { get; private set; } // Rate per unit over limit

    // Discount information
    public decimal? DiscountPercentage { get; private set; }

    public Money? DiscountAmount { get; private set; }

    [StringLength(100)]
    public string? PromoCode { get; private set; }

    public DateTime? DiscountEndDate { get; private set; }

    // Auto-renewal settings
    public bool AutoRenew { get; private set; } = true;

    public int? AutoRenewNotificationDays { get; private set; } = 7;

    // Grace period for late payments
    public int? GracePeriodDays { get; private set; } = 7;

    public DateTime? GracePeriodEndDate { get; private set; }

    // Tax information
    public TaxRate? TaxRate { get; private set; }

    public bool TaxInclusive { get; private set; } = true;

    // Contract information
    [StringLength(100)]
    public string? ContractNumber { get; private set; }

    public DateTime? ContractStartDate { get; private set; }

    public DateTime? ContractEndDate { get; private set; }

    public int? MinimumTermMonths { get; private set; }

    [StringLength(1000)]
    public string? TermsAndConditions { get; private set; }

    // Audit fields
    public new DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public new string? CreatedBy { get; private set; }

    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    [StringLength(100)]
    public string? CancelledBy { get; private set; }

    [StringLength(500)]
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;

    private readonly List<Invoice> _invoices = new();
    public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

    private Subscription() : base() { } // EF Core

    public Subscription(
        Guid id,
        string tenantId,
        string subscriptionNumber,
        Guid customerId,
        string? customerName,
        string planName,
        Money monthlyAmount,
        BillingFrequency billingFrequency,
        DateTime startDate,
        string createdBy,
        string? planDescription = null,
        PaymentMethod preferredPaymentMethod = PaymentMethod.BankTransfer) : base(id, tenantId)
    {
        SubscriptionNumber = subscriptionNumber?.Trim() ?? throw new ArgumentException("Subscription number cannot be empty");
        CustomerId = customerId;
        CustomerName = customerName?.Trim();
        PlanName = planName?.Trim() ?? throw new ArgumentException("Plan name cannot be empty");
        PlanDescription = planDescription?.Trim();
        MonthlyAmount = monthlyAmount ?? throw new ArgumentNullException(nameof(monthlyAmount));
        BillingFrequency = billingFrequency;
        StartDate = startDate.Date;
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        PreferredPaymentMethod = preferredPaymentMethod;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
        Currency = monthlyAmount.Currency.ToString();

        CalculateNextBillingDate();
        CalculateAnnualAmount();
    }

    public static Subscription Create(
        string tenantId,
        string subscriptionNumber,
        Guid customerId,
        string? customerName,
        string planName,
        Money monthlyAmount,
        BillingFrequency billingFrequency,
        DateTime startDate,
        string createdBy,
        string? planDescription = null,
        PaymentMethod preferredPaymentMethod = PaymentMethod.BankTransfer)
    {
        return new Subscription(Guid.NewGuid(), tenantId, subscriptionNumber, customerId, customerName,
            planName, monthlyAmount, billingFrequency, startDate, createdBy, planDescription, preferredPaymentMethod);
    }

    public void StartTrial(int trialDays, string modifiedBy)
    {
        if (Status != SubscriptionStatus.PendingActivation)
            throw new InvalidOperationException("Only pending subscriptions can start trial");

        HasTrialPeriod = true;
        TrialDays = trialDays;
        TrialStartDate = DateTime.UtcNow.Date;
        TrialEndDate = DateTime.UtcNow.Date.AddDays(trialDays);
        Status = SubscriptionStatus.Trial;
        
        NextBillingDate = TrialEndDate.Value.AddDays(1);
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Activate(string modifiedBy)
    {
        if (Status == SubscriptionStatus.Active)
            return;

        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot activate cancelled subscription");

        Status = SubscriptionStatus.Active;
        
        if (!HasTrialPeriod || (TrialEndDate.HasValue && DateTime.UtcNow.Date >= TrialEndDate.Value))
        {
            CalculateNextBillingDate();
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Suspend(string reason, string modifiedBy)
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot suspend cancelled subscription");

        Status = SubscriptionStatus.Suspended;
        CancellationReason = reason?.Trim();
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Cancel(string reason, string cancelledBy)
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
        CancelledBy = cancelledBy?.Trim() ?? throw new ArgumentException("CancelledBy cannot be empty");
        CancellationReason = reason?.Trim();
        EndDate = DateTime.UtcNow.Date;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = cancelledBy;
    }

    public void MarkAsPastDue(string modifiedBy)
    {
        if (Status == SubscriptionStatus.Active || Status == SubscriptionStatus.Trial)
        {
            Status = SubscriptionStatus.PastDue;
            GracePeriodEndDate = DateTime.UtcNow.Date.AddDays(GracePeriodDays ?? 7);
            
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
        }
    }

    public void RecordPayment(Money amount, DateTime paymentDate, string modifiedBy)
    {
        if (Status == SubscriptionStatus.PastDue || Status == SubscriptionStatus.Suspended)
        {
            Status = SubscriptionStatus.Active;
            GracePeriodEndDate = null;
        }

        LastBillingDate = paymentDate.Date;
        CalculateNextBillingDate();
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdatePlan(string planName, Money newMonthlyAmount, string? planDescription, string modifiedBy)
    {
        PlanName = planName?.Trim() ?? throw new ArgumentException("Plan name cannot be empty");
        MonthlyAmount = newMonthlyAmount ?? throw new ArgumentNullException(nameof(newMonthlyAmount));
        PlanDescription = planDescription?.Trim();
        
        CalculateAnnualAmount();
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateBillingFrequency(BillingFrequency billingFrequency, string modifiedBy)
    {
        BillingFrequency = billingFrequency;
        CalculateNextBillingDate();
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetCommunitySubscription(
        string communityGroupName, 
        int communityMembers, 
        Money? communityDiscount, 
        bool allowSharedAccess,
        string modifiedBy)
    {
        IsCommunitySubscription = true;
        CommunityGroupName = communityGroupName?.Trim() ?? throw new ArgumentException("Community group name cannot be empty");
        CommunityMembers = communityMembers > 0 ? communityMembers : throw new ArgumentException("Community members must be positive");
        CommunityDiscount = communityDiscount;
        AllowSharedAccess = allowSharedAccess;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetUsageLimits(decimal usageLimit, string usageUnit, Money? overageRate, string modifiedBy)
    {
        UsageLimit = usageLimit > 0 ? usageLimit : throw new ArgumentException("Usage limit must be positive");
        UsageUnit = usageUnit?.Trim() ?? throw new ArgumentException("Usage unit cannot be empty");
        OverageRate = overageRate;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateUsage(decimal currentUsage, string modifiedBy)
    {
        CurrentUsage = currentUsage >= 0 ? currentUsage : throw new ArgumentException("Current usage cannot be negative");
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void ApplyDiscount(decimal? discountPercentage, Money? discountAmount, string? promoCode, DateTime? discountEndDate, string modifiedBy)
    {
        if (discountPercentage.HasValue && (discountPercentage < 0 || discountPercentage > 100))
            throw new ArgumentException("Discount percentage must be between 0 and 100");

        DiscountPercentage = discountPercentage;
        DiscountAmount = discountAmount;
        PromoCode = promoCode?.Trim();
        DiscountEndDate = discountEndDate;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetAutoRenewal(bool autoRenew, int? notificationDays, string modifiedBy)
    {
        AutoRenew = autoRenew;
        AutoRenewNotificationDays = notificationDays;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdatePaymentMethod(PaymentMethod paymentMethod, string? paymentMethodDetails, string modifiedBy)
    {
        PreferredPaymentMethod = paymentMethod;
        PaymentMethodDetails = paymentMethodDetails?.Trim();
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetTaxRate(TaxRate? taxRate, bool taxInclusive, string modifiedBy)
    {
        TaxRate = taxRate;
        TaxInclusive = taxInclusive;
        
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    private void CalculateNextBillingDate()
    {
        var baseDate = LastBillingDate ?? StartDate;

        NextBillingDate = BillingFrequency switch
        {
            BillingFrequency.Daily => baseDate.AddDays(BillingCycle),
            BillingFrequency.Weekly => baseDate.AddDays(7 * BillingCycle),
            BillingFrequency.Monthly => baseDate.AddMonths(BillingCycle),
            BillingFrequency.Quarterly => baseDate.AddMonths(3 * BillingCycle),
            BillingFrequency.SemiAnnually => baseDate.AddMonths(6 * BillingCycle),
            BillingFrequency.Annually => baseDate.AddYears(BillingCycle),
            BillingFrequency.Biennial => baseDate.AddYears(2 * BillingCycle),
            _ => baseDate.AddMonths(BillingCycle)
        };
    }

    private void CalculateAnnualAmount()
    {
        AnnualAmount = BillingFrequency switch
        {
            BillingFrequency.Monthly => MonthlyAmount.Multiply(12),
            BillingFrequency.Quarterly => MonthlyAmount.Multiply(4),
            BillingFrequency.SemiAnnually => MonthlyAmount.Multiply(2),
            BillingFrequency.Annually => MonthlyAmount,
            BillingFrequency.Biennial => MonthlyAmount.Divide(2),
            _ => MonthlyAmount.Multiply(12)
        };
    }

    public Money CalculateCurrentBillAmount()
    {
        var baseAmount = BillingFrequency switch
        {
            BillingFrequency.Daily => MonthlyAmount.Divide(30),
            BillingFrequency.Weekly => MonthlyAmount.Divide(4),
            BillingFrequency.Monthly => MonthlyAmount,
            BillingFrequency.Quarterly => MonthlyAmount.Multiply(3),
            BillingFrequency.SemiAnnually => MonthlyAmount.Multiply(6),
            BillingFrequency.Annually => MonthlyAmount.Multiply(12),
            BillingFrequency.Biennial => MonthlyAmount.Multiply(24),
            _ => MonthlyAmount
        };

        // Apply community discount
        if (CommunityDiscount != null && CommunityDiscount.Amount > 0)
        {
            baseAmount = baseAmount.Subtract(CommunityDiscount);
        }

        // Apply percentage discount
        if (DiscountPercentage.HasValue && DiscountPercentage > 0)
        {
            var discountAmount = baseAmount.Multiply(DiscountPercentage.Value / 100);
            baseAmount = baseAmount.Subtract(discountAmount);
        }

        // Apply fixed discount
        if (DiscountAmount != null && DiscountAmount.Amount > 0)
        {
            baseAmount = baseAmount.Subtract(DiscountAmount);
        }

        // Add overage charges
        if (UsageLimit.HasValue && CurrentUsage.HasValue && CurrentUsage > UsageLimit && OverageRate != null)
        {
            var overage = (decimal)(CurrentUsage - UsageLimit);
            var overageAmount = OverageRate.Multiply(overage);
            baseAmount = baseAmount.Add(overageAmount);
        }

        // Add setup fee if not paid
        if (SetupFee != null && !SetupFeePaid)
        {
            baseAmount = baseAmount.Add(SetupFee);
        }

        return baseAmount;
    }

    public bool IsDueForBilling => NextBillingDate.Date <= DateTime.UtcNow.Date;

    public bool IsInTrial => Status == SubscriptionStatus.Trial && TrialEndDate.HasValue && DateTime.UtcNow.Date <= TrialEndDate.Value;

    public bool IsTrialExpired => HasTrialPeriod && TrialEndDate.HasValue && DateTime.UtcNow.Date > TrialEndDate.Value;

    public bool IsActive => Status == SubscriptionStatus.Active || Status == SubscriptionStatus.Trial;

    public bool IsPastDue => Status == SubscriptionStatus.PastDue;

    public bool IsCancelled => Status == SubscriptionStatus.Cancelled;

    public bool IsSuspended => Status == SubscriptionStatus.Suspended;

    public bool IsInGracePeriod => GracePeriodEndDate.HasValue && DateTime.UtcNow.Date <= GracePeriodEndDate.Value;

    public bool IsOverUsage => UsageLimit.HasValue && CurrentUsage.HasValue && CurrentUsage > UsageLimit;

    public int DaysUntilNextBilling => (NextBillingDate.Date - DateTime.UtcNow.Date).Days;

    public int DaysInGracePeriod => IsInGracePeriod && GracePeriodEndDate.HasValue 
        ? (GracePeriodEndDate.Value - DateTime.UtcNow.Date).Days 
        : 0;
}
