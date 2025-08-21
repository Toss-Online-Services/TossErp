using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Projects.Application.Commands.CreateMilestone;
using Projects.Application.Commands.UpdateMilestone;
using Projects.Application.Commands.DeleteMilestone;
using Projects.Application.Commands.CompleteMilestone;
using Projects.Application.Commands.ReorderMilestones;
using Projects.Application.Queries.GetMilestone;
using Projects.Application.Queries.GetMilestones;
using Projects.Application.Queries.GetMilestonesByProject;
using Projects.Application.DTOs;
using Projects.Domain.Enums;
using ClosedXML.Excel;

namespace Projects.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class MilestonesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MilestonesController> _logger;

    public MilestonesController(IMediator mediator, ILogger<MilestonesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all milestones with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestones(
        [FromQuery] Guid? projectId = null,
        [FromQuery] MilestoneStatus? status = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] bool? isOverdue = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetMilestonesQuery
            {
                ProjectId = projectId,
                Status = status,
                DueDateFrom = dueDateFrom,
                DueDateTo = dueDateTo,
                IsOverdue = isOverdue,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Milestones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving milestones");
            return StatusCode(500, "An error occurred while retrieving milestones");
        }
    }

    /// <summary>
    /// Get milestone by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MilestoneDto>> GetMilestone(Guid id)
    {
        try
        {
            var query = new GetMilestoneQuery { Id = id };
            var milestone = await _mediator.Send(query);

            if (milestone == null)
            {
                return NotFound($"Milestone with ID {id} not found");
            }

            return Ok(milestone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving milestone {MilestoneId}", id);
            return StatusCode(500, "An error occurred while retrieving the milestone");
        }
    }

    /// <summary>
    /// Get milestones by project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestonesByProject(
        Guid projectId,
        [FromQuery] MilestoneStatus? status = null,
        [FromQuery] bool includeCompleted = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetMilestonesByProjectQuery
            {
                ProjectId = projectId,
                Status = status,
                IncludeCompleted = includeCompleted,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Milestones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving milestones for project {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving milestones by project");
        }
    }

    /// <summary>
    /// Create a new milestone
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<MilestoneDto>> CreateMilestone([FromBody] CreateMilestoneCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var milestone = await _mediator.Send(command);
            
            _logger.LogInformation("Milestone created: {MilestoneId} - {Title}", 
                milestone.Id, milestone.Title);
            
            return CreatedAtAction(nameof(GetMilestone), new { id = milestone.Id }, milestone);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid milestone creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating milestone");
            return StatusCode(500, "An error occurred while creating the milestone");
        }
    }

    /// <summary>
    /// Update an existing milestone
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<MilestoneDto>> UpdateMilestone(Guid id, [FromBody] UpdateMilestoneCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Milestone ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var milestone = await _mediator.Send(command);

            if (milestone == null)
            {
                return NotFound($"Milestone with ID {id} not found");
            }

            _logger.LogInformation("Milestone updated: {MilestoneId} - {Title}", 
                milestone.Id, milestone.Title);

            return Ok(milestone);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid milestone update attempt for {MilestoneId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating milestone {MilestoneId}", id);
            return StatusCode(500, "An error occurred while updating the milestone");
        }
    }

    /// <summary>
    /// Delete a milestone (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> DeleteMilestone(Guid id)
    {
        try
        {
            var command = new DeleteMilestoneCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Milestone with ID {id} not found");
            }

            _logger.LogInformation("Milestone deleted: {MilestoneId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete milestone {MilestoneId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting milestone {MilestoneId}", id);
            return StatusCode(500, "An error occurred while deleting the milestone");
        }
    }

    /// <summary>
    /// Complete a milestone
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<MilestoneDto>> CompleteMilestone(Guid id, [FromBody] CompleteMilestoneCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Milestone ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var milestone = await _mediator.Send(command);

            if (milestone == null)
            {
                return NotFound($"Milestone with ID {id} not found");
            }

            _logger.LogInformation("Milestone completed: {MilestoneId} - {Title}", 
                milestone.Id, milestone.Title);

            return Ok(milestone);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot complete milestone {MilestoneId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing milestone {MilestoneId}", id);
            return StatusCode(500, "An error occurred while completing the milestone");
        }
    }

    /// <summary>
    /// Reorder milestones within a project
    /// </summary>
    [HttpPost("reorder")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ReorderMilestones([FromBody] ReorderMilestonesCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return BadRequest("Failed to reorder milestones");
            }

            _logger.LogInformation("Milestones reordered in project {ProjectId}", command.ProjectId);

            return Ok(new { message = "Milestones reordered successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reorder milestones");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering milestones");
            return StatusCode(500, "An error occurred while reordering milestones");
        }
    }

    /// <summary>
    /// Get upcoming milestones (due within specified days)
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetUpcomingMilestones(
        [FromQuery] int daysAhead = 30,
        [FromQuery] Guid? projectId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetMilestonesQuery
            {
                ProjectId = projectId,
                Status = MilestoneStatus.InProgress,
                DueDateFrom = DateTime.UtcNow,
                DueDateTo = DateTime.UtcNow.AddDays(daysAhead),
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Milestones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming milestones");
            return StatusCode(500, "An error occurred while retrieving upcoming milestones");
        }
    }

    /// <summary>
    /// Get overdue milestones
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetOverdueMilestones(
        [FromQuery] Guid? projectId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetMilestonesQuery
            {
                ProjectId = projectId,
                IsOverdue = true,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Milestones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue milestones");
            return StatusCode(500, "An error occurred while retrieving overdue milestones");
        }
    }

    /// <summary>
    /// Export milestones to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportMilestonesToExcel(
        [FromQuery] Guid? projectId = null,
        [FromQuery] MilestoneStatus? status = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null)
    {
        try
        {
            var query = new GetMilestonesQuery
            {
                ProjectId = projectId,
                Status = status,
                DueDateFrom = dueDateFrom,
                DueDateTo = dueDateTo,
                Page = 1,
                PageSize = int.MaxValue // Get all milestones for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Milestones");

            // Headers
            worksheet.Cell(1, 1).Value = "Project";
            worksheet.Cell(1, 2).Value = "Title";
            worksheet.Cell(1, 3).Value = "Description";
            worksheet.Cell(1, 4).Value = "Due Date";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Progress %";
            worksheet.Cell(1, 7).Value = "Order";
            worksheet.Cell(1, 8).Value = "Created Date";
            worksheet.Cell(1, 9).Value = "Completed Date";
            worksheet.Cell(1, 10).Value = "Days Until Due";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var milestone in result.Milestones)
            {
                worksheet.Cell(row, 1).Value = milestone.ProjectName;
                worksheet.Cell(row, 2).Value = milestone.Title;
                worksheet.Cell(row, 3).Value = milestone.Description ?? "";
                worksheet.Cell(row, 4).Value = milestone.DueDate;
                worksheet.Cell(row, 5).Value = milestone.Status.ToString();
                worksheet.Cell(row, 6).Value = milestone.CompletionPercentage;
                worksheet.Cell(row, 7).Value = milestone.Order;
                worksheet.Cell(row, 8).Value = milestone.CreatedAt;
                worksheet.Cell(row, 9).Value = milestone.CompletedAt;
                
                // Calculate days until due
                var daysUntilDue = milestone.DueDate.HasValue ? 
                    (milestone.DueDate.Value.Date - DateTime.UtcNow.Date).Days : (int?)null;
                worksheet.Cell(row, 10).Value = daysUntilDue;

                // Color-code overdue milestones
                if (daysUntilDue < 0 && milestone.Status != MilestoneStatus.Completed)
                {
                    var range = worksheet.Range(row, 1, row, 10);
                    range.Style.Fill.BackgroundColor = XLColor.LightPink;
                }
                else if (daysUntilDue >= 0 && daysUntilDue <= 3 && milestone.Status != MilestoneStatus.Completed)
                {
                    var range = worksheet.Range(row, 1, row, 10);
                    range.Style.Fill.BackgroundColor = XLColor.LightYellow;
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Milestones_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {MilestoneCount} milestones to Excel", result.Milestones.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting milestones to Excel");
            return StatusCode(500, "An error occurred while exporting milestones");
        }
    }

    /// <summary>
    /// Get milestone statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<object>> GetMilestoneStatistics(
        [FromQuery] Guid? projectId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        try
        {
            var query = new GetMilestonesQuery
            {
                ProjectId = projectId,
                DueDateFrom = dateFrom,
                DueDateTo = dateTo,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalMilestones = result.TotalCount,
                CompletedMilestones = result.Milestones.Count(m => m.Status == MilestoneStatus.Completed),
                InProgressMilestones = result.Milestones.Count(m => m.Status == MilestoneStatus.InProgress),
                NotStartedMilestones = result.Milestones.Count(m => m.Status == MilestoneStatus.NotStarted),
                OverdueMilestones = result.Milestones.Count(m => 
                    m.DueDate.HasValue && m.DueDate.Value < DateTime.UtcNow && m.Status != MilestoneStatus.Completed),
                UpcomingMilestones = result.Milestones.Count(m => 
                    m.DueDate.HasValue && m.DueDate.Value >= DateTime.UtcNow && 
                    m.DueDate.Value <= DateTime.UtcNow.AddDays(7) && m.Status != MilestoneStatus.Completed),
                AverageCompletionPercentage = result.Milestones.Any() ? 
                    result.Milestones.Average(m => m.CompletionPercentage) : 0,
                MilestonesByProject = result.Milestones.GroupBy(m => m.ProjectName)
                    .Select(g => new { 
                        Project = g.Key, 
                        Total = g.Count(), 
                        Completed = g.Count(m => m.Status == MilestoneStatus.Completed),
                        Overdue = g.Count(m => m.DueDate.HasValue && m.DueDate.Value < DateTime.UtcNow && 
                                            m.Status != MilestoneStatus.Completed)
                    })
                    .ToList(),
                CompletionTrend = result.Milestones
                    .Where(m => m.CompletedAt.HasValue)
                    .GroupBy(m => m.CompletedAt.Value.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList(),
                OnTimeCompletionRate = result.Milestones.Count(m => 
                    m.Status == MilestoneStatus.Completed && m.CompletedAt.HasValue && 
                    m.DueDate.HasValue && m.CompletedAt.Value <= m.DueDate.Value) / 
                    (double)Math.Max(1, result.Milestones.Count(m => m.Status == MilestoneStatus.Completed))
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving milestone statistics");
            return StatusCode(500, "An error occurred while retrieving milestone statistics");
        }
    }

    /// <summary>
    /// Get milestone timeline for a project
    /// </summary>
    [HttpGet("timeline/{projectId}")]
    public async Task<ActionResult<object>> GetMilestoneTimeline(Guid projectId)
    {
        try
        {
            var query = new GetMilestonesByProjectQuery
            {
                ProjectId = projectId,
                IncludeCompleted = true,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var timeline = result.Milestones
                .OrderBy(m => m.Order)
                .ThenBy(m => m.DueDate)
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.DueDate,
                    m.Status,
                    m.CompletionPercentage,
                    m.Order,
                    m.CreatedAt,
                    m.CompletedAt,
                    IsOverdue = m.DueDate.HasValue && m.DueDate.Value < DateTime.UtcNow && m.Status != MilestoneStatus.Completed,
                    DaysUntilDue = m.DueDate.HasValue ? (m.DueDate.Value.Date - DateTime.UtcNow.Date).Days : (int?)null
                })
                .ToList();

            return Ok(timeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving milestone timeline for project {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the milestone timeline");
        }
    }
}
