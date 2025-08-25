using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using Crm.Domain.Repositories;
using Crm.Domain.Exceptions;
using MediatR;

namespace Crm.Application.Queries;

public class GetCustomerAnalyticsQuery : IRequest<CustomerAnalyticsDto>
{
    public Guid CustomerId { get; set; }

    public GetCustomerAnalyticsQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}

public class GetCustomerAnalyticsQueryHandler : IRequestHandler<GetCustomerAnalyticsQuery, CustomerAnalyticsDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetCustomerAnalyticsQueryHandler(ICustomerRepository customerRepository, IAnalyticsRepository analyticsRepository)
    {
        _customerRepository = customerRepository;
        _analyticsRepository = analyticsRepository;
    }

    public async Task<CustomerAnalyticsDto> Handle(GetCustomerAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        
        if (customer == null)
        {
            throw new CustomerNotFoundException(request.CustomerId);
        }

        var analytics = await _analyticsRepository.GetCustomerAnalyticsDataAsync(request.CustomerId, cancellationToken);

        return new CustomerAnalyticsDto
        {
            CustomerId = customer.Id,
            CustomerName = customer.FullName,
            TotalSpent = customer.TotalSpent,
            PurchaseCount = customer.PurchaseCount,
            AverageOrderValue = customer.PurchaseCount > 0 ? customer.TotalSpent / customer.PurchaseCount : 0,
            LoyaltyPoints = customer.LoyaltyPoints,
            Status = customer.Status.ToString(),
            Segment = customer.Segment.ToString(),
            FirstPurchaseDate = analytics.FirstPurchaseDate,
            LastPurchaseDate = customer.LastPurchaseDate,
            DaysSinceLastPurchase = customer.LastPurchaseDate.HasValue 
                ? (int)(DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays 
                : null,
            IsLapsed = customer.IsLapsed,
            IsHighValue = customer.IsHighValue,
            MonthlyPurchaseTrend = analytics.MonthlyPurchaseTrend,
            PreferredPurchaseDays = analytics.PreferredPurchaseDays,
            SeasonalTrends = analytics.SeasonalTrends,
            RiskScore = CalculateRiskScore(customer, analytics),
            Recommendations = GenerateRecommendations(customer, analytics)
        };
    }

    private static decimal CalculateRiskScore(Customer customer, CustomerAnalyticsData analytics)
    {
        var score = 0m;
        
        // Recency factor (0-40 points)
        if (customer.LastPurchaseDate.HasValue)
        {
            var daysSince = (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays;
            score += Math.Max(0, 40 - (decimal)(daysSince / 2));
        }
        
        // Frequency factor (0-30 points)
        var daysSinceFirst = analytics.FirstPurchaseDate.HasValue 
            ? (DateTime.UtcNow - analytics.FirstPurchaseDate.Value).TotalDays 
            : 1;
        var purchaseFrequency = customer.PurchaseCount / Math.Max(1, daysSinceFirst / 30);
        score += Math.Min(30, (decimal)purchaseFrequency * 10);
        
        // Monetary factor (0-30 points)
        score += Math.Min(30, customer.TotalSpent / 1000 * 5);
        
        return Math.Min(100, Math.Max(0, score));
    }

    private static List<string> GenerateRecommendations(Customer customer, CustomerAnalyticsData analytics)
    {
        var recommendations = new List<string>();
        
        if (customer.IsLapsed)
        {
            recommendations.Add("Customer is lapsed - consider a win-back campaign");
        }
        
        if (customer.LoyaltyPoints > 500)
        {
            recommendations.Add("Customer has high loyalty points - suggest redemption offers");
        }
        
        if (customer.IsHighValue && customer.LastPurchaseDate.HasValue)
        {
            var daysSince = (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays;
            if (daysSince > 30)
            {
                recommendations.Add("High-value customer hasn't purchased recently - priority follow-up");
            }
        }
        
        if (customer.Segment == CustomerSegment.Regular && customer.TotalSpent > 800)
        {
            recommendations.Add("Customer is close to Silver tier - promote benefits");
        }
        
        return recommendations;
    }
}
