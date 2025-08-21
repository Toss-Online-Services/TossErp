using Microsoft.AspNetCore.Mvc;
using Financial.Application.DTOs;

namespace Financial.API.Controllers;

/// <summary>
/// API controller for insurance policy and claims management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Insurance")]
public class InsuranceController : ControllerBase
{
    private readonly ILogger<InsuranceController> _logger;

    public InsuranceController(ILogger<InsuranceController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all insurance policies for the current tenant
    /// </summary>
    [HttpGet("policies")]
    public async Task<ActionResult<IEnumerable<InsurancePolicyDto>>> GetPolicies(
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting insurance policies - Type: {Type}, Status: {Status}, Page: {Page}", 
                type, status, page);

            // Mock data for now - replace with actual service call
            var mockPolicies = new List<InsurancePolicyDto>
            {
                new InsurancePolicyDto
                {
                    Id = Guid.NewGuid(),
                    PolicyNumber = "POL-001",
                    Type = Financial.Domain.Enums.InsuranceType.BusinessLiability,
                    Status = Financial.Domain.Enums.PolicyStatus.Active,
                    PolicyName = "General Business Liability",
                    Description = "Comprehensive business liability coverage",
                    CoverageAmount = 1000000,
                    PremiumAmount = 2500,
                    Deductible = 1000,
                    Currency = "USD",
                    EffectiveDate = DateTime.Now.AddDays(-30),
                    ExpiryDate = DateTime.Now.AddDays(335),
                    InsurerName = "SafeGuard Insurance Co.",
                    AgentName = "Jane Smith",
                    PaymentFrequency = "Quarterly",
                    NextPaymentDate = DateTime.Now.AddDays(60),
                    OutstandingPremium = 625,
                    AutoRenewal = true,
                    ClaimsCount = 1,
                    TotalClaimsAmount = 5000
                }
            };

            return Ok(mockPolicies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting insurance policies");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific insurance policy by ID
    /// </summary>
    [HttpGet("policies/{id}")]
    public async Task<ActionResult<InsurancePolicyDto>> GetPolicy(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting insurance policy {PolicyId}", id);

            // Mock data - replace with actual service call
            var policy = new InsurancePolicyDto
            {
                Id = id,
                PolicyNumber = "POL-001",
                Type = Financial.Domain.Enums.InsuranceType.BusinessLiability,
                Status = Financial.Domain.Enums.PolicyStatus.Active,
                PolicyName = "General Business Liability",
                Description = "Comprehensive business liability coverage",
                CoverageAmount = 1000000,
                PremiumAmount = 2500,
                Deductible = 1000,
                Currency = "USD",
                EffectiveDate = DateTime.Now.AddDays(-30),
                ExpiryDate = DateTime.Now.AddDays(335),
                InsurerName = "SafeGuard Insurance Co.",
                AgentName = "Jane Smith",
                PaymentFrequency = "Quarterly",
                NextPaymentDate = DateTime.Now.AddDays(60),
                OutstandingPremium = 625,
                AutoRenewal = true,
                ClaimsCount = 1,
                TotalClaimsAmount = 5000
            };

            return Ok(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting insurance policy {PolicyId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new insurance policy application
    /// </summary>
    [HttpPost("policies")]
    public async Task<ActionResult<InsurancePolicyDto>> CreatePolicy([FromBody] CreateInsurancePolicyDto request)
    {
        try
        {
            _logger.LogInformation("Creating insurance policy application for {PolicyName}", request.PolicyName);

            // Mock response - replace with actual service call
            var newPolicy = new InsurancePolicyDto
            {
                Id = Guid.NewGuid(),
                PolicyNumber = "POL-" + DateTime.Now.Ticks.ToString()[^6..],
                Type = request.Type,
                Status = Financial.Domain.Enums.PolicyStatus.Pending,
                PolicyName = request.PolicyName,
                Description = request.Description,
                CoverageAmount = request.CoverageAmount,
                PremiumAmount = CalculatePremium(request.Type, request.CoverageAmount),
                Deductible = 1000, // Default deductible
                Currency = request.Currency,
                EffectiveDate = request.EffectiveDate,
                ExpiryDate = request.ExpiryDate,
                InsurerName = request.InsurerName,
                AgentName = request.AgentName,
                PaymentFrequency = request.PaymentFrequency,
                AutoRenewal = request.AutoRenewal,
                ClaimsCount = 0,
                TotalClaimsAmount = 0
            };

            return CreatedAtAction(nameof(GetPolicy), new { id = newPolicy.Id }, newPolicy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating insurance policy");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all insurance claims for the current tenant
    /// </summary>
    [HttpGet("claims")]
    public async Task<ActionResult<IEnumerable<InsuranceClaimDto>>> GetClaims(
        [FromQuery] string? status = null,
        [FromQuery] Guid? policyId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting insurance claims - Status: {Status}, PolicyId: {PolicyId}, Page: {Page}", 
                status, policyId, page);

            // Mock data - replace with actual service call
            var mockClaims = new List<InsuranceClaimDto>
            {
                new InsuranceClaimDto
                {
                    Id = Guid.NewGuid(),
                    PolicyId = policyId ?? Guid.NewGuid(),
                    ClaimNumber = "CLM-001",
                    Status = Financial.Domain.Enums.ClaimStatus.UnderReview,
                    IncidentDate = DateTime.Now.AddDays(-10),
                    ClaimDate = DateTime.Now.AddDays(-8),
                    IncidentDescription = "Equipment damage due to power surge",
                    IncidentLocation = "Main office building",
                    IncidentType = "Property damage",
                    ClaimedAmount = 15000,
                    ApprovedAmount = 0,
                    PaidAmount = 0,
                    Deductible = 1000,
                    AssignedAdjuster = "Mike Johnson",
                    PolicyName = "Business Property Insurance",
                    PolicyNumber = "POL-002"
                }
            };

            return Ok(mockClaims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting insurance claims");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific insurance claim by ID
    /// </summary>
    [HttpGet("claims/{id}")]
    public async Task<ActionResult<InsuranceClaimDto>> GetClaim(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting insurance claim {ClaimId}", id);

            // Mock data - replace with actual service call
            var claim = new InsuranceClaimDto
            {
                Id = id,
                PolicyId = Guid.NewGuid(),
                ClaimNumber = "CLM-001",
                Status = Financial.Domain.Enums.ClaimStatus.UnderReview,
                IncidentDate = DateTime.Now.AddDays(-10),
                ClaimDate = DateTime.Now.AddDays(-8),
                IncidentDescription = "Equipment damage due to power surge",
                IncidentLocation = "Main office building",
                IncidentType = "Property damage",
                ClaimedAmount = 15000,
                ApprovedAmount = 0,
                PaidAmount = 0,
                Deductible = 1000,
                AssignedAdjuster = "Mike Johnson",
                PolicyName = "Business Property Insurance",
                PolicyNumber = "POL-002"
            };

            return Ok(claim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting insurance claim {ClaimId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// File a new insurance claim
    /// </summary>
    [HttpPost("claims")]
    public async Task<ActionResult<InsuranceClaimDto>> FileClaim([FromBody] CreateInsuranceClaimDto request)
    {
        try
        {
            _logger.LogInformation("Filing insurance claim for policy {PolicyId}", request.PolicyId);

            // Mock response - replace with actual service call
            var newClaim = new InsuranceClaimDto
            {
                Id = Guid.NewGuid(),
                PolicyId = request.PolicyId,
                ClaimNumber = "CLM-" + DateTime.Now.Ticks.ToString()[^6..],
                Status = Financial.Domain.Enums.ClaimStatus.Submitted,
                IncidentDate = request.IncidentDate,
                ClaimDate = DateTime.UtcNow,
                IncidentDescription = request.IncidentDescription,
                IncidentLocation = request.IncidentLocation,
                IncidentType = request.IncidentType,
                ClaimedAmount = request.ClaimedAmount,
                ApprovedAmount = 0,
                PaidAmount = 0,
                Deductible = 1000, // Retrieved from policy
                AssignedAdjuster = "Auto-assigned Adjuster",
                PolicyName = "Retrieved from policy",
                PolicyNumber = "Retrieved from policy"
            };

            return CreatedAtAction(nameof(GetClaim), new { id = newClaim.Id }, newClaim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filing insurance claim");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get policy payments for a specific policy
    /// </summary>
    [HttpGet("policies/{id}/payments")]
    public async Task<ActionResult<IEnumerable<PolicyPaymentDto>>> GetPolicyPayments(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting payments for policy {PolicyId}", id);

            // Mock data - replace with actual service call
            var payments = new List<PolicyPaymentDto>
            {
                new PolicyPaymentDto
                {
                    Id = Guid.NewGuid(),
                    PolicyId = id,
                    Amount = 625,
                    Currency = "USD",
                    PaymentDate = DateTime.Now.AddDays(-90),
                    DueDate = DateTime.Now.AddDays(-90),
                    CoverageStartDate = DateTime.Now.AddDays(-90),
                    CoverageEndDate = DateTime.Now.AddDays(0),
                    PaymentMethod = "Credit Card",
                    TransactionReference = "TXN001",
                    PaymentStatus = "Completed",
                    IsLate = false,
                    DaysLate = 0,
                    LateFee = 0,
                    ReceiptNumber = "RCP001"
                }
            };

            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting policy payments for {PolicyId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    private static decimal CalculatePremium(Financial.Domain.Enums.InsuranceType type, decimal coverageAmount)
    {
        // Simple premium calculation based on type and coverage
        var rate = type switch
        {
            Financial.Domain.Enums.InsuranceType.BusinessLiability => 0.0025m,
            Financial.Domain.Enums.InsuranceType.Property => 0.005m,
            Financial.Domain.Enums.InsuranceType.Vehicle => 0.008m,
            Financial.Domain.Enums.InsuranceType.Health => 0.12m,
            _ => 0.003m
        };

        return coverageAmount * rate;
    }
}
