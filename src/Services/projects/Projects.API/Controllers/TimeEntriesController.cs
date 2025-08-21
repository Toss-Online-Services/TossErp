using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Projects.Application.Commands.CreateTimeEntry;
using Projects.Application.Commands.UpdateTimeEntry;
using Projects.Application.Commands.DeleteTimeEntry;
using Projects.Application.Commands.StartTimeTracking;
using Projects.Application.Commands.StopTimeTracking;
using Projects.Application.Commands.ApproveTimeEntry;
using Projects.Application.Commands.RejectTimeEntry;
using Projects.Application.Queries.GetTimeEntry;
using Projects.Application.Queries.GetTimeEntries;
using Projects.Application.Queries.GetTimeEntriesByProject;
using Projects.Application.Queries.GetTimeEntriesByUser;
using Projects.Application.Queries.GetTimeEntriesByTask;
using Projects.Application.Queries.GetTimeSheet;
using Projects.Application.DTOs;
using Projects.Domain.Enums;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Projects.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TimeEntriesController> _logger;

    public TimeEntriesController(IMediator mediator, ILogger<TimeEntriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all time entries with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntries(
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? taskId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] TimeEntryStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTimeEntriesQuery
            {
                ProjectId = projectId,
                TaskId = taskId,
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.TimeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries");
            return StatusCode(500, "An error occurred while retrieving time entries");
        }
    }

    /// <summary>
    /// Get time entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TimeEntryDto>> GetTimeEntry(Guid id)
    {
        try
        {
            var query = new GetTimeEntryQuery { Id = id };
            var timeEntry = await _mediator.Send(query);

            if (timeEntry == null)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            return Ok(timeEntry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entry {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while retrieving the time entry");
        }
    }

    /// <summary>
    /// Get time entries by project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByProject(
        Guid projectId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTimeEntriesByProjectQuery
            {
                ProjectId = projectId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.TimeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries for project {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving time entries by project");
        }
    }

    /// <summary>
    /// Get time entries by user
    /// </summary>
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByUser(
        Guid userId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTimeEntriesByUserQuery
            {
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ProjectId = projectId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.TimeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving time entries by user");
        }
    }

    /// <summary>
    /// Get time entries by task
    /// </summary>
    [HttpGet("by-task/{taskId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByTask(
        Guid taskId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTimeEntriesByTaskQuery
            {
                TaskId = taskId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.TimeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries for task {TaskId}", taskId);
            return StatusCode(500, "An error occurred while retrieving time entries by task");
        }
    }

    /// <summary>
    /// Get timesheet for a user and date range
    /// </summary>
    [HttpGet("timesheet")]
    public async Task<ActionResult<TimeSheetDto>> GetTimeSheet(
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? weekStarting = null)
    {
        try
        {
            // If no user specified, use current user
            if (!userId.HasValue)
            {
                var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var currentUserId))
                {
                    return BadRequest("Unable to determine user identity");
                }
                userId = currentUserId;
            }

            var query = new GetTimeSheetQuery
            {
                UserId = userId.Value,
                WeekStarting = weekStarting ?? DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek)
            };

            var timeSheet = await _mediator.Send(query);

            return Ok(timeSheet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timesheet for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the timesheet");
        }
    }

    /// <summary>
    /// Create a new time entry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TimeEntryDto>> CreateTimeEntry([FromBody] CreateTimeEntryCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEntry = await _mediator.Send(command);
            
            _logger.LogInformation("Time entry created: {TimeEntryId} - {Hours} hours", 
                timeEntry.Id, timeEntry.Hours);
            
            return CreatedAtAction(nameof(GetTimeEntry), new { id = timeEntry.Id }, timeEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid time entry creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating time entry");
            return StatusCode(500, "An error occurred while creating the time entry");
        }
    }

    /// <summary>
    /// Update an existing time entry
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TimeEntryDto>> UpdateTimeEntry(Guid id, [FromBody] UpdateTimeEntryCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Time entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEntry = await _mediator.Send(command);

            if (timeEntry == null)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            _logger.LogInformation("Time entry updated: {TimeEntryId} - {Hours} hours", 
                timeEntry.Id, timeEntry.Hours);

            return Ok(timeEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid time entry update attempt for {TimeEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating time entry {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while updating the time entry");
        }
    }

    /// <summary>
    /// Delete a time entry (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> DeleteTimeEntry(Guid id)
    {
        try
        {
            var command = new DeleteTimeEntryCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            _logger.LogInformation("Time entry deleted: {TimeEntryId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete time entry {TimeEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting time entry {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while deleting the time entry");
        }
    }

    /// <summary>
    /// Start time tracking for a task
    /// </summary>
    [HttpPost("start-tracking")]
    public async Task<ActionResult<TimeEntryDto>> StartTimeTracking([FromBody] StartTimeTrackingCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEntry = await _mediator.Send(command);
            
            _logger.LogInformation("Time tracking started: {TimeEntryId} for task {TaskId}", 
                timeEntry.Id, command.TaskId);
            
            return Ok(timeEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot start time tracking");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting time tracking");
            return StatusCode(500, "An error occurred while starting time tracking");
        }
    }

    /// <summary>
    /// Stop time tracking
    /// </summary>
    [HttpPost("{id}/stop-tracking")]
    public async Task<ActionResult<TimeEntryDto>> StopTimeTracking(Guid id, [FromBody] StopTimeTrackingCommand command)
    {
        try
        {
            if (id != command.TimeEntryId)
            {
                return BadRequest("Time entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEntry = await _mediator.Send(command);

            if (timeEntry == null)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            _logger.LogInformation("Time tracking stopped: {TimeEntryId} - {Hours} hours tracked", 
                timeEntry.Id, timeEntry.Hours);

            return Ok(timeEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot stop time tracking for {TimeEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping time tracking for {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while stopping time tracking");
        }
    }

    /// <summary>
    /// Approve a time entry
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ApproveTimeEntry(Guid id, [FromBody] ApproveTimeEntryCommand command)
    {
        try
        {
            if (id != command.TimeEntryId)
            {
                return BadRequest("Time entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            _logger.LogInformation("Time entry approved: {TimeEntryId}", id);

            return Ok(new { message = "Time entry approved successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot approve time entry {TimeEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving time entry {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while approving the time entry");
        }
    }

    /// <summary>
    /// Reject a time entry
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> RejectTimeEntry(Guid id, [FromBody] RejectTimeEntryCommand command)
    {
        try
        {
            if (id != command.TimeEntryId)
            {
                return BadRequest("Time entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Time entry with ID {id} not found");
            }

            _logger.LogInformation("Time entry rejected: {TimeEntryId}", id);

            return Ok(new { message = "Time entry rejected successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reject time entry {TimeEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting time entry {TimeEntryId}", id);
            return StatusCode(500, "An error occurred while rejecting the time entry");
        }
    }

    /// <summary>
    /// Export time entries to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportTimeEntriesToExcel(
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] TimeEntryStatus? status = null)
    {
        try
        {
            var query = new GetTimeEntriesQuery
            {
                ProjectId = projectId,
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = status,
                Page = 1,
                PageSize = int.MaxValue // Get all entries for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Time Entries");

            // Headers
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "User";
            worksheet.Cell(1, 3).Value = "Project";
            worksheet.Cell(1, 4).Value = "Task";
            worksheet.Cell(1, 5).Value = "Description";
            worksheet.Cell(1, 6).Value = "Start Time";
            worksheet.Cell(1, 7).Value = "End Time";
            worksheet.Cell(1, 8).Value = "Hours";
            worksheet.Cell(1, 9).Value = "Status";
            worksheet.Cell(1, 10).Value = "Billable";
            worksheet.Cell(1, 11).Value = "Created Date";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var entry in result.TimeEntries)
            {
                worksheet.Cell(row, 1).Value = entry.Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = entry.UserName;
                worksheet.Cell(row, 3).Value = entry.ProjectName;
                worksheet.Cell(row, 4).Value = entry.TaskTitle ?? "";
                worksheet.Cell(row, 5).Value = entry.Description ?? "";
                worksheet.Cell(row, 6).Value = entry.StartTime?.ToString("HH:mm") ?? "";
                worksheet.Cell(row, 7).Value = entry.EndTime?.ToString("HH:mm") ?? "";
                worksheet.Cell(row, 8).Value = entry.Hours;
                worksheet.Cell(row, 9).Value = entry.Status.ToString();
                worksheet.Cell(row, 10).Value = entry.IsBillable ? "Yes" : "No";
                worksheet.Cell(row, 11).Value = entry.CreatedAt;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Time_Entries_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {EntryCount} time entries to Excel", result.TimeEntries.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting time entries to Excel");
            return StatusCode(500, "An error occurred while exporting time entries");
        }
    }

    /// <summary>
    /// Get time entry statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<object>> GetTimeEntryStatistics(
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        try
        {
            var query = new GetTimeEntriesQuery
            {
                ProjectId = projectId,
                UserId = userId,
                DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-30),
                DateTo = dateTo ?? DateTime.UtcNow,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalEntries = result.TotalCount,
                TotalHours = result.TimeEntries.Sum(t => t.Hours),
                BillableHours = result.TimeEntries.Where(t => t.IsBillable).Sum(t => t.Hours),
                NonBillableHours = result.TimeEntries.Where(t => !t.IsBillable).Sum(t => t.Hours),
                EntriesByStatus = result.TimeEntries.GroupBy(t => t.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count(), Hours = g.Sum(t => t.Hours) })
                    .ToList(),
                HoursByProject = result.TimeEntries.GroupBy(t => t.ProjectName)
                    .Select(g => new { Project = g.Key, Hours = g.Sum(t => t.Hours), Entries = g.Count() })
                    .OrderByDescending(x => x.Hours)
                    .ToList(),
                HoursByUser = result.TimeEntries.GroupBy(t => t.UserName)
                    .Select(g => new { User = g.Key, Hours = g.Sum(t => t.Hours), Entries = g.Count() })
                    .OrderByDescending(x => x.Hours)
                    .ToList(),
                DailyHours = result.TimeEntries
                    .GroupBy(t => t.Date.Date)
                    .Select(g => new { Date = g.Key, Hours = g.Sum(t => t.Hours), Entries = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList(),
                AverageHoursPerEntry = result.TimeEntries.Any() ? result.TimeEntries.Average(t => t.Hours) : 0,
                PendingApprovalEntries = result.TimeEntries.Count(t => t.Status == TimeEntryStatus.Submitted)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entry statistics");
            return StatusCode(500, "An error occurred while retrieving time entry statistics");
        }
    }

    /// <summary>
    /// Get my time entries (for the authenticated user)
    /// </summary>
    [HttpGet("my-entries")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetMyTimeEntries(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] TimeEntryStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("Unable to determine user identity");
            }

            var query = new GetTimeEntriesByUserQuery
            {
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ProjectId = projectId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.TimeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving my time entries");
            return StatusCode(500, "An error occurred while retrieving your time entries");
        }
    }
}
