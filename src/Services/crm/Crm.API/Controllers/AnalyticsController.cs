using Microsoft.AspNetCore.Mvc;
using Crm.Application.DTOs;

namespace Crm.API.Controllers;

/// <summary>
/// API controller for customer analytics and reporting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(ILogger<AnalyticsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive customer analytics
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<CustomerAnalyticsDto>> GetCustomerAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation("Getting customer analytics");

            // Mock data - replace with actual service call using MediatR
            var analytics = new CustomerAnalyticsDto
            {
                TotalCustomers = 1247,
                ActiveCustomers = 1089,
                LapsedCustomers = 158,
                HighValueCustomers = 89,
                TotalRevenue = 234567.89m,
                AverageOrderValue = 187.43m,
                AverageCustomerValue = 215.67m,
                SegmentBreakdown = new CustomerSegmentBreakdown
                {
                    Regular = 856,
                    Silver = 245,
                    Gold = 98,
                    Premium = 48
                },
                TopCustomers = new List<TopCustomerDto>
                {
                    new TopCustomerDto
                    {
                        Id = Guid.NewGuid(),
                        FullName = "Premium Customer",
                        Email = "premium@email.com",
                        TotalSpent = 15000.00m,
                        PurchaseCount = 45,
                        Segment = "Premium"
                    },
                    new TopCustomerDto
                    {
                        Id = Guid.NewGuid(),
                        FullName = "Gold Customer",
                        Email = "gold@email.com",
                        TotalSpent = 8500.00m,
                        PurchaseCount = 32,
                        Segment = "Gold"
                    }
                }
            };

            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer analytics");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer segmentation analytics
    /// </summary>
    [HttpGet("segmentation")]
    public async Task<ActionResult<CustomerSegmentBreakdown>> GetSegmentationAnalytics()
    {
        try
        {
            _logger.LogInformation("Getting customer segmentation analytics");

            // Mock data - replace with actual service call using MediatR
            var segmentation = new CustomerSegmentBreakdown
            {
                Regular = 856,
                Silver = 245,
                Gold = 98,
                Premium = 48
            };

            return Ok(segmentation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting segmentation analytics");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get revenue analytics
    /// </summary>
    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueAnalyticsDto>> GetRevenueAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation("Getting revenue analytics");

            // Mock data - replace with actual service call using MediatR
            var revenue = new RevenueAnalyticsDto
            {
                TotalRevenue = 234567.89m,
                MonthlyRevenue = 19547.32m,
                AverageOrderValue = 187.43m,
                RevenueGrowth = 12.5m,
                MonthlyGrowth = 8.3m,
                RevenueBySegment = new Dictionary<string, decimal>
                {
                    ["Regular"] = 98765.43m,
                    ["Silver"] = 67890.12m,
                    ["Gold"] = 45678.90m,
                    ["Premium"] = 22233.44m
                }
            };

            return Ok(revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue analytics");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer retention analytics
    /// </summary>
    [HttpGet("retention")]
    public async Task<ActionResult<CustomerRetentionDto>> GetRetentionAnalytics()
    {
        try
        {
            _logger.LogInformation("Getting customer retention analytics");

            // Mock data - replace with actual service call using MediatR
            var retention = new CustomerRetentionDto
            {
                RetentionRate = 87.3m,
                ChurnRate = 12.7m,
                AverageCustomerLifespan = 18.5m,
                CustomerLifetimeValue = 1234.56m,
                LapsedCustomers = 158,
                ReactivatedCustomers = 23,
                NewCustomers = 89,
                RetentionTrends = new List<MonthlyRetentionDto>
                {
                    new MonthlyRetentionDto
                    {
                        Month = "2024-12",
                        RetentionRate = 87.3m,
                        ChurnRate = 12.7m,
                        NewCustomers = 89,
                        LostCustomers = 34
                    }
                }
            };

            return Ok(retention);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting retention analytics");
            return BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// DTO for customer analytics data
/// </summary>
public class CustomerAnalyticsDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int LapsedCustomers { get; set; }
    public int HighValueCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal AverageCustomerValue { get; set; }
    public CustomerSegmentBreakdown SegmentBreakdown { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class CustomerSegmentBreakdown
{
    public int Regular { get; set; }
    public int Silver { get; set; }
    public int Gold { get; set; }
    public int Premium { get; set; }
}

public class TopCustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int PurchaseCount { get; set; }
    public string Segment { get; set; } = string.Empty;
}

public class RevenueAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal RevenueGrowth { get; set; }
    public decimal MonthlyGrowth { get; set; }
    public Dictionary<string, decimal> RevenueBySegment { get; set; } = new();
}

public class CustomerRetentionDto
{
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
    public decimal AverageCustomerLifespan { get; set; }
    public decimal CustomerLifetimeValue { get; set; }
    public int LapsedCustomers { get; set; }
    public int ReactivatedCustomers { get; set; }
    public int NewCustomers { get; set; }
    public List<MonthlyRetentionDto> RetentionTrends { get; set; } = new();
}

public class MonthlyRetentionDto
{
    public string Month { get; set; } = string.Empty;
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
    public int NewCustomers { get; set; }
    public int LostCustomers { get; set; }
}
