using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TossErp.Accounts.Application.Commands.Company;
using TossErp.Accounts.Application.Queries.Company;
using TossErp.Accounts.Application.DTOs;
using ClosedXML.Excel;
using System.Text.Json;

namespace TossErp.Accounts.API.Controllers;

/// <summary>
/// Company management controller for ERPNext-compliant company operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(IMediator mediator, ILogger<CompaniesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all companies with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isGroup = null,
        [FromQuery] string? currency = null,
        [FromQuery] string? country = null,
        [FromQuery] Guid? parentCompanyId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetCompaniesQuery
            {
                SearchTerm = searchTerm,
                IsActive = isActive,
                IsGroup = isGroup,
                Currency = currency,
                Country = country,
                ParentCompanyId = parentCompanyId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Companies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, "An error occurred while retrieving companies");
        }
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
    {
        try
        {
            var query = new GetCompanyQuery { Id = id };
            var company = await _mediator.Send(query);

            if (company == null)
            {
                return NotFound($"Company with ID {id} not found");
            }

            return Ok(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company {CompanyId}", id);
            return StatusCode(500, "An error occurred while retrieving the company");
        }
    }

    /// <summary>
    /// Get companies summary for dropdown lists
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<CompanySummaryDto>>> GetCompaniesSummary(
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isGroup = null)
    {
        try
        {
            var query = new GetCompaniesSummaryQuery
            {
                IsActive = isActive,
                IsGroup = isGroup
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies summary");
            return StatusCode(500, "An error occurred while retrieving companies summary");
        }
    }

    /// <summary>
    /// Get group companies (parent companies)
    /// </summary>
    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetGroupCompanies(
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var query = new GetGroupCompaniesQuery
            {
                IsActive = isActive
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving group companies");
            return StatusCode(500, "An error occurred while retrieving group companies");
        }
    }

    /// <summary>
    /// Get child companies of a parent company
    /// </summary>
    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetChildCompanies(Guid parentId)
    {
        try
        {
            var query = new GetChildCompaniesQuery { ParentCompanyId = parentId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child companies for parent {ParentCompanyId}", parentId);
            return StatusCode(500, "An error occurred while retrieving child companies");
        }
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _mediator.Send(command);
            
            _logger.LogInformation("Company created: {CompanyId} - {CompanyName}", company.Id, company.Name);
            
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid company data provided");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Company creation failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, "An error occurred while creating the company");
        }
    }

    /// <summary>
    /// Update an existing company
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Company ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _mediator.Send(command);

            if (company == null)
            {
                return NotFound($"Company with ID {id} not found");
            }

            _logger.LogInformation("Company updated: {CompanyId} - {CompanyName}", company.Id, company.Name);

            return Ok(company);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid company data provided for update");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Company update failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company {CompanyId}", id);
            return StatusCode(500, "An error occurred while updating the company");
        }
    }

    /// <summary>
    /// Delete a company (soft delete by deactivation)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        try
        {
            var command = new DeleteCompanyCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Company with ID {id} not found");
            }

            _logger.LogInformation("Company deleted: {CompanyId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete company {CompanyId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company {CompanyId}", id);
            return StatusCode(500, "An error occurred while deleting the company");
        }
    }

    /// <summary>
    /// Activate a company
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> ActivateCompany(Guid id)
    {
        try
        {
            var command = new ActivateCompanyCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Company with ID {id} not found");
            }

            _logger.LogInformation("Company activated: {CompanyId}", id);

            return Ok(new { message = "Company activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating company {CompanyId}", id);
            return StatusCode(500, "An error occurred while activating the company");
        }
    }

    /// <summary>
    /// Deactivate a company
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> DeactivateCompany(Guid id)
    {
        try
        {
            var command = new DeactivateCompanyCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Company with ID {id} not found");
            }

            _logger.LogInformation("Company deactivated: {CompanyId}", id);

            return Ok(new { message = "Company deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating company {CompanyId}", id);
            return StatusCode(500, "An error occurred while deactivating the company");
        }
    }

    /// <summary>
    /// Get company statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<ActionResult<object>> GetCompanyStatistics()
    {
        try
        {
            var query = new GetCompanyStatisticsQuery();
            var stats = await _mediator.Send(query);

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company statistics");
            return StatusCode(500, "An error occurred while retrieving company statistics");
        }
    }

    /// <summary>
    /// Export companies to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> ExportCompaniesToExcel(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isGroup = null,
        [FromQuery] string? currency = null,
        [FromQuery] string? country = null)
    {
        try
        {
            var query = new GetCompaniesQuery
            {
                SearchTerm = searchTerm,
                IsActive = isActive,
                IsGroup = isGroup,
                Currency = currency,
                Country = country,
                Page = 1,
                PageSize = int.MaxValue // Get all companies for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Companies");

            // Headers
            worksheet.Cell(1, 1).Value = "Company Name";
            worksheet.Cell(1, 2).Value = "Abbreviation";
            worksheet.Cell(1, 3).Value = "Domain";
            worksheet.Cell(1, 4).Value = "Is Group";
            worksheet.Cell(1, 5).Value = "Is Active";
            worksheet.Cell(1, 6).Value = "Currency";
            worksheet.Cell(1, 7).Value = "Country";
            worksheet.Cell(1, 8).Value = "Parent Company";
            worksheet.Cell(1, 9).Value = "Email";
            worksheet.Cell(1, 10).Value = "Phone";
            worksheet.Cell(1, 11).Value = "Created Date";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var company in result.Companies)
            {
                worksheet.Cell(row, 1).Value = company.Name;
                worksheet.Cell(row, 2).Value = company.Abbreviation;
                worksheet.Cell(row, 3).Value = company.Domain ?? "";
                worksheet.Cell(row, 4).Value = company.IsGroup ? "Yes" : "No";
                worksheet.Cell(row, 5).Value = company.IsActive ? "Yes" : "No";
                worksheet.Cell(row, 6).Value = company.Currency;
                worksheet.Cell(row, 7).Value = company.Country;
                worksheet.Cell(row, 8).Value = company.ParentCompanyName ?? "";
                worksheet.Cell(row, 9).Value = company.Email ?? "";
                worksheet.Cell(row, 10).Value = company.Phone ?? "";
                worksheet.Cell(row, 11).Value = company.CreatedAt;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Companies_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {CompanyCount} companies to Excel", result.Companies.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting companies to Excel");
            return StatusCode(500, "An error occurred while exporting companies");
        }
    }

    /// <summary>
    /// Validate company name uniqueness
    /// </summary>
    [HttpGet("validate/name")]
    public async Task<ActionResult<object>> ValidateCompanyName(
        [FromQuery] string name,
        [FromQuery] Guid? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Company name is required");
            }

            var query = new ValidateCompanyNameQuery 
            { 
                Name = name.Trim(),
                ExcludeId = excludeId
            };

            var isAvailable = await _mediator.Send(query);

            return Ok(new { 
                name = name.Trim(),
                isAvailable = isAvailable,
                message = isAvailable ? "Company name is available" : "Company name is already in use"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating company name");
            return StatusCode(500, "An error occurred while validating company name");
        }
    }

    /// <summary>
    /// Validate company abbreviation uniqueness
    /// </summary>
    [HttpGet("validate/abbreviation")]
    public async Task<ActionResult<object>> ValidateCompanyAbbreviation(
        [FromQuery] string abbreviation,
        [FromQuery] Guid? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                return BadRequest("Company abbreviation is required");
            }

            var query = new ValidateCompanyAbbreviationQuery 
            { 
                Abbreviation = abbreviation.Trim().ToUpper(),
                ExcludeId = excludeId
            };

            var isAvailable = await _mediator.Send(query);

            return Ok(new { 
                abbreviation = abbreviation.Trim().ToUpper(),
                isAvailable = isAvailable,
                message = isAvailable ? "Company abbreviation is available" : "Company abbreviation is already in use"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating company abbreviation");
            return StatusCode(500, "An error occurred while validating company abbreviation");
        }
    }

    /// <summary>
    /// Get company hierarchy tree
    /// </summary>
    [HttpGet("hierarchy")]
    public async Task<ActionResult<object>> GetCompanyHierarchy([FromQuery] bool? includeInactive = false)
    {
        try
        {
            var query = new GetCompanyHierarchyQuery 
            { 
                IncludeInactive = includeInactive ?? false
            };

            var hierarchy = await _mediator.Send(query);
            return Ok(hierarchy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company hierarchy");
            return StatusCode(500, "An error occurred while retrieving company hierarchy");
        }
    }

    /// <summary>
    /// Update company settings (JSON field)
    /// </summary>
    [HttpPatch("{id}/settings")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> UpdateCompanySettings(Guid id, [FromBody] JsonElement settings)
    {
        try
        {
            var command = new UpdateCompanySettingsCommand 
            { 
                Id = id,
                Settings = settings
            };

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Company with ID {id} not found");
            }

            _logger.LogInformation("Company settings updated: {CompanyId}", id);

            return Ok(new { message = "Company settings updated successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid settings data for company {CompanyId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company settings {CompanyId}", id);
            return StatusCode(500, "An error occurred while updating company settings");
        }
    }
}
