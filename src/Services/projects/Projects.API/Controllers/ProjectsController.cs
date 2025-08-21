using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Projects.Application.Commands.CreateProject;
using Projects.Application.Commands.UpdateProject;
using Projects.Application.Commands.DeleteProject;
using Projects.Application.Commands.StartProject;
using Projects.Application.Commands.CompleteProject;
using Projects.Application.Commands.SuspendProject;
using Projects.Application.Commands.ResumeProject;
using Projects.Application.Commands.ArchiveProject;
using Projects.Application.Queries.GetProject;
using Projects.Application.Queries.GetProjects;
using Projects.Application.Queries.GetProjectsByStatus;
using Projects.Application.Queries.GetProjectsByManager;
using Projects.Application.Queries.GetProjectStatistics;
using Projects.Application.Queries.GetProjectTimeline;
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
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IMediator mediator, ILogger<ProjectsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(
        [FromQuery] ProjectStatus? status = null,
        [FromQuery] ProjectPriority? priority = null,
        [FromQuery] Guid? managerId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null,
        [FromQuery] DateTime? endDateFrom = null,
        [FromQuery] DateTime? endDateTo = null,
        [FromQuery] decimal? budgetMin = null,
        [FromQuery] decimal? budgetMax = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetProjectsQuery
            {
                Status = status,
                Priority = priority,
                ManagerId = managerId,
                SearchTerm = searchTerm,
                StartDateFrom = startDateFrom,
                StartDateTo = startDateTo,
                EndDateFrom = endDateFrom,
                EndDateTo = endDateTo,
                BudgetMin = budgetMin,
                BudgetMax = budgetMax,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, "An error occurred while retrieving projects");
        }
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        try
        {
            var query = new GetProjectQuery { Id = id };
            var project = await _mediator.Send(query);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found");
            }

            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
            return StatusCode(500, "An error occurred while retrieving the project");
        }
    }

    /// <summary>
    /// Get projects by status
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByStatus(
        ProjectStatus status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetProjectsByStatusQuery
            {
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

            return Ok(result.Projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects by status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving projects by status");
        }
    }

    /// <summary>
    /// Get projects by project manager
    /// </summary>
    [HttpGet("by-manager/{managerId}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByManager(
        Guid managerId,
        [FromQuery] ProjectStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetProjectsByManagerQuery
            {
                ManagerId = managerId,
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

            return Ok(result.Projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects by manager {ManagerId}", managerId);
            return StatusCode(500, "An error occurred while retrieving projects by manager");
        }
    }

    /// <summary>
    /// Get project timeline
    /// </summary>
    [HttpGet("{id}/timeline")]
    public async Task<ActionResult<ProjectTimelineDto>> GetProjectTimeline(Guid id)
    {
        try
        {
            var query = new GetProjectTimelineQuery { ProjectId = id };
            var timeline = await _mediator.Send(query);

            if (timeline == null)
            {
                return NotFound($"Project with ID {id} not found");
            }

            return Ok(timeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timeline for project {ProjectId}", id);
            return StatusCode(500, "An error occurred while retrieving project timeline");
        }
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _mediator.Send(command);
            
            _logger.LogInformation("Project created: {ProjectId} - {ProjectName}", project.Id, project.Name);
            
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid project creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, "An error occurred while creating the project");
        }
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Project ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _mediator.Send(command);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project updated: {ProjectId} - {ProjectName}", project.Id, project.Name);

            return Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid project update attempt for {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId}", id);
            return StatusCode(500, "An error occurred while updating the project");
        }
    }

    /// <summary>
    /// Delete a project (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        try
        {
            var command = new DeleteProjectCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project deleted: {ProjectId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId}", id);
            return StatusCode(500, "An error occurred while deleting the project");
        }
    }

    /// <summary>
    /// Start a project
    /// </summary>
    [HttpPost("{id}/start")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> StartProject(Guid id)
    {
        try
        {
            var command = new StartProjectCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project started: {ProjectId}", id);

            return Ok(new { message = "Project started successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot start project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting project {ProjectId}", id);
            return StatusCode(500, "An error occurred while starting the project");
        }
    }

    /// <summary>
    /// Complete a project
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> CompleteProject(Guid id, [FromBody] CompleteProjectCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Project ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project completed: {ProjectId}", id);

            return Ok(new { message = "Project completed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot complete project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing project {ProjectId}", id);
            return StatusCode(500, "An error occurred while completing the project");
        }
    }

    /// <summary>
    /// Suspend a project
    /// </summary>
    [HttpPost("{id}/suspend")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> SuspendProject(Guid id, [FromBody] SuspendProjectCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Project ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project suspended: {ProjectId}", id);

            return Ok(new { message = "Project suspended successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot suspend project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending project {ProjectId}", id);
            return StatusCode(500, "An error occurred while suspending the project");
        }
    }

    /// <summary>
    /// Resume a suspended project
    /// </summary>
    [HttpPost("{id}/resume")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> ResumeProject(Guid id)
    {
        try
        {
            var command = new ResumeProjectCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project resumed: {ProjectId}", id);

            return Ok(new { message = "Project resumed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot resume project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming project {ProjectId}", id);
            return StatusCode(500, "An error occurred while resuming the project");
        }
    }

    /// <summary>
    /// Archive a project
    /// </summary>
    [HttpPost("{id}/archive")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> ArchiveProject(Guid id, [FromBody] ArchiveProjectCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Project ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project with ID {id} not found");
            }

            _logger.LogInformation("Project archived: {ProjectId}", id);

            return Ok(new { message = "Project archived successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot archive project {ProjectId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving project {ProjectId}", id);
            return StatusCode(500, "An error occurred while archiving the project");
        }
    }

    /// <summary>
    /// Export projects to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> ExportProjectsToExcel(
        [FromQuery] ProjectStatus? status = null,
        [FromQuery] ProjectPriority? priority = null,
        [FromQuery] Guid? managerId = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var query = new GetProjectsQuery
            {
                Status = status,
                Priority = priority,
                ManagerId = managerId,
                SearchTerm = searchTerm,
                Page = 1,
                PageSize = int.MaxValue // Get all projects for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Projects");

            // Headers
            worksheet.Cell(1, 1).Value = "Project Code";
            worksheet.Cell(1, 2).Value = "Project Name";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Priority";
            worksheet.Cell(1, 5).Value = "Manager";
            worksheet.Cell(1, 6).Value = "Start Date";
            worksheet.Cell(1, 7).Value = "End Date";
            worksheet.Cell(1, 8).Value = "Budget";
            worksheet.Cell(1, 9).Value = "Actual Cost";
            worksheet.Cell(1, 10).Value = "Progress %";
            worksheet.Cell(1, 11).Value = "Created Date";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var project in result.Projects)
            {
                worksheet.Cell(row, 1).Value = project.Code;
                worksheet.Cell(row, 2).Value = project.Name;
                worksheet.Cell(row, 3).Value = project.Status.ToString();
                worksheet.Cell(row, 4).Value = project.Priority.ToString();
                worksheet.Cell(row, 5).Value = project.ManagerName;
                worksheet.Cell(row, 6).Value = project.StartDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 7).Value = project.EndDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 8).Value = project.Budget;
                worksheet.Cell(row, 9).Value = project.ActualCost;
                worksheet.Cell(row, 10).Value = project.CompletionPercentage;
                worksheet.Cell(row, 11).Value = project.CreatedAt;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Projects_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {ProjectCount} projects to Excel", result.Projects.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting projects to Excel");
            return StatusCode(500, "An error occurred while exporting projects");
        }
    }

    /// <summary>
    /// Get project statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<ActionResult<ProjectStatisticsDto>> GetProjectStatistics()
    {
        try
        {
            var query = new GetProjectStatisticsQuery();
            var statistics = await _mediator.Send(query);

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project statistics");
            return StatusCode(500, "An error occurred while retrieving project statistics");
        }
    }

    /// <summary>
    /// Get dashboard data for projects
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<ActionResult<object>> GetProjectsDashboard()
    {
        try
        {
            var query = new GetProjectsQuery
            {
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var dashboard = new
            {
                TotalProjects = result.TotalCount,
                ProjectsByStatus = result.Projects.GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                ProjectsByPriority = result.Projects.GroupBy(p => p.Priority)
                    .Select(g => new { Priority = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                ActiveProjects = result.Projects.Count(p => p.Status == ProjectStatus.InProgress),
                CompletedProjects = result.Projects.Count(p => p.Status == ProjectStatus.Completed),
                OverdueProjects = result.Projects.Count(p => p.EndDate.HasValue && p.EndDate < DateTime.UtcNow && p.Status != ProjectStatus.Completed),
                TotalBudget = result.Projects.Sum(p => p.Budget ?? 0),
                TotalActualCost = result.Projects.Sum(p => p.ActualCost ?? 0),
                AverageCompletion = result.Projects.Any() ? result.Projects.Average(p => p.CompletionPercentage) : 0,
                ProjectsStartingThisMonth = result.Projects.Count(p => p.StartDate.HasValue && 
                    p.StartDate.Value.Year == DateTime.UtcNow.Year && 
                    p.StartDate.Value.Month == DateTime.UtcNow.Month),
                ProjectsEndingThisMonth = result.Projects.Count(p => p.EndDate.HasValue && 
                    p.EndDate.Value.Year == DateTime.UtcNow.Year && 
                    p.EndDate.Value.Month == DateTime.UtcNow.Month)
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects dashboard");
            return StatusCode(500, "An error occurred while retrieving projects dashboard");
        }
    }
}
