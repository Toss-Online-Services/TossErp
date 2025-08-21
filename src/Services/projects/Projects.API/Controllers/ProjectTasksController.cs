using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Projects.Application.Commands.CreateProjectTask;
using Projects.Application.Commands.UpdateProjectTask;
using Projects.Application.Commands.DeleteProjectTask;
using Projects.Application.Commands.StartTask;
using Projects.Application.Commands.CompleteTask;
using Projects.Application.Commands.AssignTask;
using Projects.Application.Commands.UnassignTask;
using Projects.Application.Queries.GetProjectTask;
using Projects.Application.Queries.GetProjectTasks;
using Projects.Application.Queries.GetTasksByProject;
using Projects.Application.Queries.GetTasksByAssignee;
using Projects.Application.Queries.GetTasksByStatus;
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
public class ProjectTasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectTasksController> _logger;

    public ProjectTasksController(IMediator mediator, ILogger<ProjectTasksController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all project tasks with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetProjectTasks(
        [FromQuery] Guid? projectId = null,
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] Guid? assignedTo = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetProjectTasksQuery
            {
                ProjectId = projectId,
                Status = status,
                Priority = priority,
                AssignedTo = assignedTo,
                SearchTerm = searchTerm,
                DueDateFrom = dueDateFrom,
                DueDateTo = dueDateTo,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project tasks");
            return StatusCode(500, "An error occurred while retrieving project tasks");
        }
    }

    /// <summary>
    /// Get project task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectTaskDto>> GetProjectTask(Guid id)
    {
        try
        {
            var query = new GetProjectTaskQuery { Id = id };
            var task = await _mediator.Send(query);

            if (task == null)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project task {TaskId}", id);
            return StatusCode(500, "An error occurred while retrieving the project task");
        }
    }

    /// <summary>
    /// Get tasks by project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasksByProject(
        Guid projectId,
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTasksByProjectQuery
            {
                ProjectId = projectId,
                Status = status,
                Priority = priority,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for project {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving tasks by project");
        }
    }

    /// <summary>
    /// Get tasks by assignee
    /// </summary>
    [HttpGet("by-assignee/{assigneeId}")]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasksByAssignee(
        Guid assigneeId,
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTasksByAssigneeQuery
            {
                AssigneeId = assigneeId,
                Status = status,
                Priority = priority,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for assignee {AssigneeId}", assigneeId);
            return StatusCode(500, "An error occurred while retrieving tasks by assignee");
        }
    }

    /// <summary>
    /// Get tasks by status
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasksByStatus(
        TaskStatus status,
        [FromQuery] Guid? projectId = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTasksByStatusQuery
            {
                Status = status,
                ProjectId = projectId,
                Priority = priority,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks by status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving tasks by status");
        }
    }

    /// <summary>
    /// Create a new project task
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<ProjectTaskDto>> CreateProjectTask([FromBody] CreateProjectTaskCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _mediator.Send(command);
            
            _logger.LogInformation("Project task created: {TaskId} - {TaskTitle}", task.Id, task.Title);
            
            return CreatedAtAction(nameof(GetProjectTask), new { id = task.Id }, task);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid project task creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project task");
            return StatusCode(500, "An error occurred while creating the project task");
        }
    }

    /// <summary>
    /// Update an existing project task
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<ProjectTaskDto>> UpdateProjectTask(Guid id, [FromBody] UpdateProjectTaskCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Task ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _mediator.Send(command);

            if (task == null)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Project task updated: {TaskId} - {TaskTitle}", task.Id, task.Title);

            return Ok(task);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid project task update attempt for {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project task {TaskId}", id);
            return StatusCode(500, "An error occurred while updating the project task");
        }
    }

    /// <summary>
    /// Delete a project task (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public async Task<IActionResult> DeleteProjectTask(Guid id)
    {
        try
        {
            var command = new DeleteProjectTaskCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Project task deleted: {TaskId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete project task {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project task {TaskId}", id);
            return StatusCode(500, "An error occurred while deleting the project task");
        }
    }

    /// <summary>
    /// Start a task
    /// </summary>
    [HttpPost("{id}/start")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead,TeamMember")]
    public async Task<IActionResult> StartTask(Guid id)
    {
        try
        {
            var command = new StartTaskCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Task started: {TaskId}", id);

            return Ok(new { message = "Task started successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot start task {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting task {TaskId}", id);
            return StatusCode(500, "An error occurred while starting the task");
        }
    }

    /// <summary>
    /// Complete a task
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead,TeamMember")]
    public async Task<IActionResult> CompleteTask(Guid id, [FromBody] CompleteTaskCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Task ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Task completed: {TaskId}", id);

            return Ok(new { message = "Task completed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot complete task {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task {TaskId}", id);
            return StatusCode(500, "An error occurred while completing the task");
        }
    }

    /// <summary>
    /// Assign a task to a user
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskCommand command)
    {
        try
        {
            if (id != command.TaskId)
            {
                return BadRequest("Task ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Task assigned: {TaskId} to {UserId}", id, command.AssignedToId);

            return Ok(new { message = "Task assigned successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot assign task {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task {TaskId}", id);
            return StatusCode(500, "An error occurred while assigning the task");
        }
    }

    /// <summary>
    /// Unassign a task from a user
    /// </summary>
    [HttpPost("{id}/unassign")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> UnassignTask(Guid id)
    {
        try
        {
            var command = new UnassignTaskCommand { TaskId = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Project task with ID {id} not found");
            }

            _logger.LogInformation("Task unassigned: {TaskId}", id);

            return Ok(new { message = "Task unassigned successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot unassign task {TaskId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning task {TaskId}", id);
            return StatusCode(500, "An error occurred while unassigning the task");
        }
    }

    /// <summary>
    /// Export project tasks to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportProjectTasksToExcel(
        [FromQuery] Guid? projectId = null,
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] Guid? assignedTo = null)
    {
        try
        {
            var query = new GetProjectTasksQuery
            {
                ProjectId = projectId,
                Status = status,
                Priority = priority,
                AssignedTo = assignedTo,
                Page = 1,
                PageSize = int.MaxValue // Get all tasks for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Project Tasks");

            // Headers
            worksheet.Cell(1, 1).Value = "Task Title";
            worksheet.Cell(1, 2).Value = "Project";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Priority";
            worksheet.Cell(1, 5).Value = "Assigned To";
            worksheet.Cell(1, 6).Value = "Start Date";
            worksheet.Cell(1, 7).Value = "Due Date";
            worksheet.Cell(1, 8).Value = "Completed Date";
            worksheet.Cell(1, 9).Value = "Estimated Hours";
            worksheet.Cell(1, 10).Value = "Actual Hours";
            worksheet.Cell(1, 11).Value = "Progress %";
            worksheet.Cell(1, 12).Value = "Created Date";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 12);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var task in result.Tasks)
            {
                worksheet.Cell(row, 1).Value = task.Title;
                worksheet.Cell(row, 2).Value = task.ProjectName;
                worksheet.Cell(row, 3).Value = task.Status.ToString();
                worksheet.Cell(row, 4).Value = task.Priority.ToString();
                worksheet.Cell(row, 5).Value = task.AssignedToName ?? "";
                worksheet.Cell(row, 6).Value = task.StartDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 7).Value = task.DueDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 8).Value = task.CompletedDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 9).Value = task.EstimatedHours;
                worksheet.Cell(row, 10).Value = task.ActualHours;
                worksheet.Cell(row, 11).Value = task.CompletionPercentage;
                worksheet.Cell(row, 12).Value = task.CreatedAt;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Project_Tasks_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {TaskCount} project tasks to Excel", result.Tasks.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting project tasks to Excel");
            return StatusCode(500, "An error occurred while exporting project tasks");
        }
    }

    /// <summary>
    /// Get task statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<object>> GetTaskStatistics(
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? assigneeId = null)
    {
        try
        {
            var query = new GetProjectTasksQuery
            {
                ProjectId = projectId,
                AssignedTo = assigneeId,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalTasks = result.TotalCount,
                TasksByStatus = result.Tasks.GroupBy(t => t.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                TasksByPriority = result.Tasks.GroupBy(t => t.Priority)
                    .Select(g => new { Priority = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                CompletedTasks = result.Tasks.Count(t => t.Status == TaskStatus.Completed),
                OverdueTasks = result.Tasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                UnassignedTasks = result.Tasks.Count(t => !t.AssignedToId.HasValue),
                AverageCompletion = result.Tasks.Any() ? result.Tasks.Average(t => t.CompletionPercentage) : 0,
                TotalEstimatedHours = result.Tasks.Sum(t => t.EstimatedHours ?? 0),
                TotalActualHours = result.Tasks.Sum(t => t.ActualHours ?? 0),
                TasksDueThisWeek = result.Tasks.Count(t => t.DueDate.HasValue && 
                    t.DueDate >= DateTime.UtcNow.Date && 
                    t.DueDate < DateTime.UtcNow.Date.AddDays(7))
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task statistics");
            return StatusCode(500, "An error occurred while retrieving task statistics");
        }
    }

    /// <summary>
    /// Get my tasks (for the authenticated user)
    /// </summary>
    [HttpGet("my-tasks")]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetMyTasks(
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
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

            var query = new GetTasksByAssigneeQuery
            {
                AssigneeId = userId,
                Status = status,
                Priority = priority,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving my tasks");
            return StatusCode(500, "An error occurred while retrieving your tasks");
        }
    }
}
