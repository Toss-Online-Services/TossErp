using Microsoft.AspNetCore.Mvc;
using Financial.Application.DTOs;

namespace Financial.API.Controllers;

/// <summary>
/// API controller for microfinance loan management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("MicroLoans")]
public class MicroLoansController : ControllerBase
{
    private readonly ILogger<MicroLoansController> _logger;

    public MicroLoansController(ILogger<MicroLoansController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all micro loans for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MicroLoanDto>>> GetLoans(
        [FromQuery] string? status = null,
        [FromQuery] string? borrowerName = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting loans with filters - Status: {Status}, Borrower: {BorrowerName}, Page: {Page}", 
                status, borrowerName, page);

            // Mock data for now - replace with actual service call
            var mockLoans = new List<MicroLoanDto>
            {
                new MicroLoanDto
                {
                    Id = Guid.NewGuid(),
                    BorrowerName = "John Doe",
                    BusinessName = "Doe Enterprises",
                    BusinessType = "Retail",
                    PrincipalAmount = 25000,
                    InterestRate = 0.12m,
                    TermMonths = 24,
                    MonthlyPayment = 1177.68m,
                    ApplicationDate = DateTime.Now.AddDays(-30),
                    ApprovalDate = DateTime.Now.AddDays(-25),
                    DisbursementDate = DateTime.Now.AddDays(-20),
                    Status = Financial.Domain.Enums.LoanStatus.Active,
                    Purpose = "Working capital for inventory",
                    OutstandingBalance = 20000,
                    TotalPaid = 5000,
                    PaymentsMade = 4,
                    PaymentsMissed = 0,
                    NextPaymentDate = DateTime.Now.AddDays(10),
                    CreditScore = 750,
                    RiskRating = "Medium"
                }
            };

            return Ok(mockLoans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loans");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific loan by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MicroLoanDto>> GetLoan(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting loan {LoanId}", id);

            // Mock data - replace with actual service call
            var loan = new MicroLoanDto
            {
                Id = id,
                BorrowerName = "John Doe",
                BusinessName = "Doe Enterprises",
                BusinessType = "Retail",
                PrincipalAmount = 25000,
                InterestRate = 0.12m,
                TermMonths = 24,
                MonthlyPayment = 1177.68m,
                ApplicationDate = DateTime.Now.AddDays(-30),
                ApprovalDate = DateTime.Now.AddDays(-25),
                DisbursementDate = DateTime.Now.AddDays(-20),
                Status = Financial.Domain.Enums.LoanStatus.Active,
                Purpose = "Working capital for inventory",
                OutstandingBalance = 20000,
                TotalPaid = 5000,
                PaymentsMade = 4,
                PaymentsMissed = 0,
                NextPaymentDate = DateTime.Now.AddDays(10),
                CreditScore = 750,
                RiskRating = "Medium"
            };

            return Ok(loan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loan {LoanId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new loan application
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MicroLoanDto>> CreateLoanApplication([FromBody] CreateLoanApplicationDto request)
    {
        try
        {
            _logger.LogInformation("Creating loan application for {BorrowerName}", request.BorrowerName);

            // Mock response - replace with actual service call
            var newLoan = new MicroLoanDto
            {
                Id = Guid.NewGuid(),
                BorrowerName = request.BorrowerName,
                BusinessName = request.BusinessName,
                BusinessType = request.BusinessType,
                PrincipalAmount = request.RequestedAmount,
                InterestRate = 0.12m, // Calculated based on risk assessment
                TermMonths = request.TermMonths,
                MonthlyPayment = CalculateMonthlyPayment(request.RequestedAmount, 0.12m, request.TermMonths),
                ApplicationDate = DateTime.UtcNow,
                Status = Financial.Domain.Enums.LoanStatus.Pending,
                Purpose = request.Purpose,
                OutstandingBalance = request.RequestedAmount,
                TotalPaid = 0,
                PaymentsMade = 0,
                PaymentsMissed = 0,
                CreditScore = 700, // Mock credit score
                RiskRating = "Medium"
            };

            return CreatedAtAction(nameof(GetLoan), new { id = newLoan.Id }, newLoan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating loan application");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get loan payments for a specific loan
    /// </summary>
    [HttpGet("{id}/payments")]
    public async Task<ActionResult<IEnumerable<LoanPaymentDto>>> GetLoanPayments(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting payments for loan {LoanId}", id);

            // Mock data - replace with actual service call
            var payments = new List<LoanPaymentDto>
            {
                new LoanPaymentDto
                {
                    Id = Guid.NewGuid(),
                    LoanId = id,
                    Amount = 1177.68m,
                    PrincipalAmount = 1000,
                    InterestAmount = 177.68m,
                    PenaltyAmount = 0,
                    PaymentDate = DateTime.Now.AddDays(-30),
                    DueDate = DateTime.Now.AddDays(-30),
                    PaymentMethod = "Bank Transfer",
                    TransactionReference = "TXN001",
                    IsLate = false,
                    DaysLate = 0,
                    RemainingBalance = 24000
                }
            };

            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loan payments for {LoanId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Record a loan payment
    /// </summary>
    [HttpPost("{id}/payments")]
    public async Task<ActionResult<LoanPaymentDto>> RecordPayment(Guid id, [FromBody] RecordLoanPaymentDto request)
    {
        try
        {
            _logger.LogInformation("Recording payment for loan {LoanId}", id);

            // Mock response - replace with actual service call
            var payment = new LoanPaymentDto
            {
                Id = Guid.NewGuid(),
                LoanId = id,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                TransactionReference = request.TransactionReference,
                IsLate = false,
                DaysLate = 0
            };

            return CreatedAtAction(nameof(GetLoanPayments), new { id }, payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment for loan {LoanId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update loan status (approve, reject, etc.)
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateLoanStatus(Guid id, [FromBody] UpdateLoanStatusDto request)
    {
        try
        {
            _logger.LogInformation("Updating status for loan {LoanId} to {Status}", id, request.Status);

            // Mock response - replace with actual service call
            return Ok(new { message = "Loan status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating loan status for {LoanId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    private static decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int termMonths)
    {
        var monthlyRate = annualRate / 12;
        var numerator = principal * monthlyRate * Math.Pow(1 + (double)monthlyRate, termMonths);
        var denominator = Math.Pow(1 + (double)monthlyRate, termMonths) - 1;
        return (decimal)(numerator / denominator);
    }
}

/// <summary>
/// DTO for updating loan status
/// </summary>
public class UpdateLoanStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
