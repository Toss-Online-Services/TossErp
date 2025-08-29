using AutoMapper;

namespace TossErp.Accounts.Application.Common;

/// <summary>
/// AutoMapper profile for Accounts domain entities to DTOs mapping
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateCustomerMappings();
        CreateInvoiceMappings();
        CreatePaymentMappings();
        CreateSubscriptionMappings();
        CreateChartOfAccountsMappings();
        CreateJournalEntryMappings();
        CreateFinancialReportingMappings();
    }

    private void CreateCustomerMappings()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.TotalInvoices, opt => opt.MapFrom(src => src.Invoices.Count))
            .ForMember(dest => dest.TotalOutstanding, opt => opt.MapFrom(src => 
                src.Invoices.Where(i => i.Status == InvoiceStatus.Sent || i.Status == InvoiceStatus.Overdue)
                           .Sum(i => i.BalanceAmount.Amount)))
            .ForMember(dest => dest.LastInvoiceDate, opt => opt.MapFrom(src => 
                src.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault() != null 
                    ? src.Invoices.OrderByDescending(i => i.CreatedAt).First().CreatedAt 
                    : (DateTime?)null));

        CreateMap<Customer, CustomerSummaryDto>()
            .ForMember(dest => dest.TotalInvoices, opt => opt.MapFrom(src => src.Invoices.Count))
            .ForMember(dest => dest.TotalPaidAmount, opt => opt.MapFrom(src => 
                src.Invoices.Sum(i => i.PaidAmount.Amount)))
            .ForMember(dest => dest.TotalOutstandingAmount, opt => opt.MapFrom(src => 
                src.Invoices.Where(i => i.Status == InvoiceStatus.Sent || i.Status == InvoiceStatus.Overdue)
                           .Sum(i => i.BalanceAmount.Amount)))
            .ForMember(dest => dest.OverdueAmount, opt => opt.MapFrom(src => 
                src.Invoices.Where(i => i.Status == InvoiceStatus.Overdue)
                           .Sum(i => i.BalanceAmount.Amount)))
            .ForMember(dest => dest.LastPaymentDate, opt => opt.MapFrom(src => 
                src.Payments.OrderByDescending(p => p.PaymentDate).FirstOrDefault() != null 
                    ? src.Payments.OrderByDescending(p => p.PaymentDate).First().PaymentDate.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.ActiveSubscriptions, opt => opt.MapFrom(src => 
                src.Subscriptions.Count(s => s.Status == SubscriptionStatus.Active)));

        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<Address, AddressDto>().ReverseMap();
    }

    private void CreateInvoiceMappings()
    {
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems));

        CreateMap<InvoiceLineItem, InvoiceLineItemDto>();

        CreateMap<CreateInvoiceCommand, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InvoiceStatus.Draft))
            .ForMember(dest => dest.SubtotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
            .ForMember(dest => dest.BalanceAmount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress));

        CreateMap<CreateInvoiceLineItemDto, InvoiceLineItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
            .ForMember(dest => dest.Invoice, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => new Money(src.UnitPrice, src.Currency)))
            .ForMember(dest => dest.LineTotal, opt => opt.Ignore());
    }

    private void CreatePaymentMappings()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.AllocatedAmount, opt => opt.MapFrom(src => 
                src.PaymentAllocations.Sum(pa => pa.AllocatedAmount.Amount)))
            .ForMember(dest => dest.UnallocatedAmount, opt => opt.MapFrom(src => 
                src.Amount.Amount - src.PaymentAllocations.Sum(pa => pa.AllocatedAmount.Amount)));

        CreateMap<ProcessPaymentCommand, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PaymentStatus.Pending))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.UtcNow)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentAllocations, opt => opt.Ignore());

        CreateMap<PaymentAllocation, PaymentAllocationDto>()
            .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Invoice.InvoiceNumber));
    }

    private void CreateSubscriptionMappings()
    {
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.DaysUntilRenewal, opt => opt.MapFrom(src => 
                src.NextBillingDate.HasValue 
                    ? (src.NextBillingDate.Value.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber)
                    : (int?)null));

        CreateMap<CreateSubscriptionCommand, Subscription>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SubscriptionNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => SubscriptionStatus.Active))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow)))
            .ForMember(dest => dest.PlanPrice, opt => opt.MapFrom(src => new Money(src.PlanPrice, src.Currency)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());
    }

    private void CreateChartOfAccountsMappings()
    {
        CreateMap<ChartOfAccounts, ChartOfAccountsDto>()
            .ForMember(dest => dest.ParentAccountName, opt => opt.MapFrom(src => 
                src.ParentAccount != null ? src.ParentAccount.AccountName : null))
            .ForMember(dest => dest.ChildAccounts, opt => opt.MapFrom(src => src.ChildAccounts))
            .ForMember(dest => dest.CurrentBalance, opt => opt.MapFrom(src => 
                src.JournalEntries.Sum(je => je.DebitAmount.Amount - je.CreditAmount.Amount)));

        CreateMap<CreateChartOfAccountsCommand, ChartOfAccounts>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ParentAccount, opt => opt.Ignore())
            .ForMember(dest => dest.ChildAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntries, opt => opt.Ignore());
    }

    private void CreateJournalEntryMappings()
    {
        CreateMap<JournalEntry, JournalEntryDto>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.AccountName))
            .ForMember(dest => dest.AccountCode, opt => opt.MapFrom(src => src.Account.AccountCode));

        CreateMap<CreateJournalEntryCommand, JournalEntry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EntryNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => JournalEntryStatus.Draft))
            .ForMember(dest => dest.DebitAmount, opt => opt.MapFrom(src => new Money(src.DebitAmount, src.Currency)))
            .ForMember(dest => dest.CreditAmount, opt => opt.MapFrom(src => new Money(src.CreditAmount, src.Currency)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());
    }

    private void CreateFinancialReportingMappings()
    {
        // Financial reporting DTOs are typically created directly from aggregated data
        // rather than mapped from domain entities
    }
}

/// <summary>
/// Extension methods for AutoMapper
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Map to paginated result
    /// </summary>
    public static PaginatedResult<TDestination> MapToPaginatedResult<TSource, TDestination>(
        this IMapper mapper, 
        PaginatedResult<TSource> source)
    {
        return new PaginatedResult<TDestination>
        {
            Items = mapper.Map<IEnumerable<TDestination>>(source.Items),
            TotalCount = source.TotalCount,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalPages = source.TotalPages
        };
    }

    /// <summary>
    /// Map collection to DTOs
    /// </summary>
    public static IEnumerable<TDestination> MapCollection<TSource, TDestination>(
        this IMapper mapper,
        IEnumerable<TSource> source)
    {
        return mapper.Map<IEnumerable<TDestination>>(source);
    }

    /// <summary>
    /// Map with null safety
    /// </summary>
    public static TDestination? MapSafely<TSource, TDestination>(
        this IMapper mapper,
        TSource? source) where TDestination : class
    {
        return source == null ? null : mapper.Map<TDestination>(source);
    }

    /// <summary>
    /// Map Money value object
    /// </summary>
    public static Money MapToMoney(decimal amount, string currency)
    {
        var currencyCode = Enum.TryParse<CurrencyCode>(currency, true, out var result) ? result : CurrencyCode.ZAR;
        return new Money(amount, currencyCode);
    }

    /// <summary>
    /// Map Address value object
    /// </summary>
    public static Address MapToAddress(AddressDto? addressDto)
    {
        if (addressDto == null) return null!;

        return new Address(
            addressDto.Street,
            addressDto.City,
            addressDto.Province,
            addressDto.PostalCode,
            addressDto.Country);
    }

    /// <summary>
    /// Map DateOnly to DateTime
    /// </summary>
    public static DateTime ToDateTime(this DateOnly dateOnly)
    {
        return dateOnly.ToDateTime(TimeOnly.MinValue);
    }

    /// <summary>
    /// Map DateTime to DateOnly
    /// </summary>
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }
}
