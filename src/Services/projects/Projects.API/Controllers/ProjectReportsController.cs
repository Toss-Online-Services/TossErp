using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Projects.Application.Queries.GetProjectSummary;
using Projects.Application.Queries.GetProjectProgress;
using Projects.Application.Queries.GetProjectTimeReport;
using Projects.Application.Queries.GetProjectBudgetReport;
using Projects.Application.Queries.GetProjectTaskReport;
using Projects.Application.Queries.GetProjectMilestoneReport;
using Projects.Application.Queries.GetProjectTeamPerformance;
using Projects.Application.Queries.GetProjectDashboard;
using Projects.Application.DTOs;
using Projects.Domain.Enums;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Projects.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/project-reports")]
[ApiVersion("1.0")]
[Authorize]
public class ProjectReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectReportsController> _logger;

    public ProjectReportsController(IMediator mediator, ILogger<ProjectReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive project dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ProjectDashboardDto>> GetProjectDashboard(
        [FromQuery] Guid? projectId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        try
        {
            var query = new GetProjectDashboardQuery
            {
                ProjectId = projectId,
                DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-30),
                DateTo = dateTo ?? DateTime.UtcNow
            };

            var dashboard = await _mediator.Send(query);

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project dashboard");
            return StatusCode(500, "An error occurred while retrieving the project dashboard");
        }
    }

    /// <summary>
    /// Get project summary report
    /// </summary>
    [HttpGet("summary/{projectId}")]
    public async Task<ActionResult<ProjectSummaryDto>> GetProjectSummary(Guid projectId)
    {
        try
        {
            var query = new GetProjectSummaryQuery { ProjectId = projectId };
            var summary = await _mediator.Send(query);

            if (summary == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project summary for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project summary");
        }
    }

    /// <summary>
    /// Get project progress report
    /// </summary>
    [HttpGet("progress/{projectId}")]
    public async Task<ActionResult<ProjectProgressDto>> GetProjectProgress(Guid projectId)
    {
        try
        {
            var query = new GetProjectProgressQuery { ProjectId = projectId };
            var progress = await _mediator.Send(query);

            if (progress == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project progress for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project progress");
        }
    }

    /// <summary>
    /// Get project time tracking report
    /// </summary>
    [HttpGet("time/{projectId}")]
    public async Task<ActionResult<ProjectTimeReportDto>> GetProjectTimeReport(
        Guid projectId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? userId = null)
    {
        try
        {
            var query = new GetProjectTimeReportQuery
            {
                ProjectId = projectId,
                DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-30),
                DateTo = dateTo ?? DateTime.UtcNow,
                UserId = userId
            };

            var timeReport = await _mediator.Send(query);

            if (timeReport == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(timeReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project time report for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project time report");
        }
    }

    /// <summary>
    /// Get project budget report
    /// </summary>
    [HttpGet("budget/{projectId}")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<ProjectBudgetReportDto>> GetProjectBudgetReport(Guid projectId)
    {
        try
        {
            var query = new GetProjectBudgetReportQuery { ProjectId = projectId };
            var budgetReport = await _mediator.Send(query);

            if (budgetReport == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(budgetReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project budget report for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project budget report");
        }
    }

    /// <summary>
    /// Get project task report
    /// </summary>
    [HttpGet("tasks/{projectId}")]
    public async Task<ActionResult<ProjectTaskReportDto>> GetProjectTaskReport(
        Guid projectId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        try
        {
            var query = new GetProjectTaskReportQuery
            {
                ProjectId = projectId,
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            var taskReport = await _mediator.Send(query);

            if (taskReport == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(taskReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project task report for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project task report");
        }
    }

    /// <summary>
    /// Get project milestone report
    /// </summary>
    [HttpGet("milestones/{projectId}")]
    public async Task<ActionResult<ProjectMilestoneReportDto>> GetProjectMilestoneReport(Guid projectId)
    {
        try
        {
            var query = new GetProjectMilestoneReportQuery { ProjectId = projectId };
            var milestoneReport = await _mediator.Send(query);

            if (milestoneReport == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(milestoneReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project milestone report for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project milestone report");
        }
    }

    /// <summary>
    /// Get project team performance report
    /// </summary>
    [HttpGet("team-performance/{projectId}")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<ActionResult<ProjectTeamPerformanceDto>> GetProjectTeamPerformance(
        Guid projectId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        try
        {
            var query = new GetProjectTeamPerformanceQuery
            {
                ProjectId = projectId,
                DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-30),
                DateTo = dateTo ?? DateTime.UtcNow
            };

            var performance = await _mediator.Send(query);

            if (performance == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            return Ok(performance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project team performance for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving the project team performance");
        }
    }

    /// <summary>
    /// Export project summary to PDF
    /// </summary>
    [HttpGet("export/summary/{projectId}/pdf")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportProjectSummaryToPdf(Guid projectId)
    {
        try
        {
            var query = new GetProjectSummaryQuery { ProjectId = projectId };
            var summary = await _mediator.Send(query);

            if (summary == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var title = new Paragraph($"Project Summary Report\n{summary.ProjectName}", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Project Details
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            document.Add(new Paragraph("Project Information", headerFont) { SpacingAfter = 10 });

            var table = new PdfPTable(2) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 30, 70 });

            table.AddCell(new PdfPCell(new Phrase("Project Name:", headerFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(summary.ProjectName, normalFont)) { Border = Rectangle.NO_BORDER });

            table.AddCell(new PdfPCell(new Phrase("Status:", headerFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(summary.Status.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

            table.AddCell(new PdfPCell(new Phrase("Progress:", headerFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase($"{summary.CompletionPercentage:F1}%", normalFont)) { Border = Rectangle.NO_BORDER });

            table.AddCell(new PdfPCell(new Phrase("Start Date:", headerFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(summary.StartDate?.ToString("yyyy-MM-dd") ?? "Not set", normalFont)) { Border = Rectangle.NO_BORDER });

            table.AddCell(new PdfPCell(new Phrase("End Date:", headerFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(summary.EndDate?.ToString("yyyy-MM-dd") ?? "Not set", normalFont)) { Border = Rectangle.NO_BORDER });

            if (summary.Budget.HasValue)
            {
                table.AddCell(new PdfPCell(new Phrase("Budget:", headerFont)) { Border = Rectangle.NO_BORDER });
                table.AddCell(new PdfPCell(new Phrase($"${summary.Budget:N2}", normalFont)) { Border = Rectangle.NO_BORDER });
            }

            document.Add(table);

            // Tasks Summary
            document.Add(new Paragraph("\nTask Summary", headerFont) { SpacingAfter = 10 });

            var taskTable = new PdfPTable(2) { WidthPercentage = 100 };
            taskTable.SetWidths(new float[] { 50, 50 });

            taskTable.AddCell(new PdfPCell(new Phrase("Total Tasks:", normalFont)) { Border = Rectangle.NO_BORDER });
            taskTable.AddCell(new PdfPCell(new Phrase(summary.TotalTasks.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

            taskTable.AddCell(new PdfPCell(new Phrase("Completed Tasks:", normalFont)) { Border = Rectangle.NO_BORDER });
            taskTable.AddCell(new PdfPCell(new Phrase(summary.CompletedTasks.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

            taskTable.AddCell(new PdfPCell(new Phrase("In Progress Tasks:", normalFont)) { Border = Rectangle.NO_BORDER });
            taskTable.AddCell(new PdfPCell(new Phrase(summary.InProgressTasks.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

            taskTable.AddCell(new PdfPCell(new Phrase("Pending Tasks:", normalFont)) { Border = Rectangle.NO_BORDER });
            taskTable.AddCell(new PdfPCell(new Phrase(summary.PendingTasks.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

            if (summary.OverdueTasks > 0)
            {
                taskTable.AddCell(new PdfPCell(new Phrase("Overdue Tasks:", normalFont)) { Border = Rectangle.NO_BORDER });
                taskTable.AddCell(new PdfPCell(new Phrase(summary.OverdueTasks.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });
            }

            document.Add(taskTable);

            // Time Summary
            if (summary.TotalHoursLogged > 0)
            {
                document.Add(new Paragraph("\nTime Summary", headerFont) { SpacingAfter = 10 });

                var timeTable = new PdfPTable(2) { WidthPercentage = 100 };
                timeTable.SetWidths(new float[] { 50, 50 });

                timeTable.AddCell(new PdfPCell(new Phrase("Total Hours Logged:", normalFont)) { Border = Rectangle.NO_BORDER });
                timeTable.AddCell(new PdfPCell(new Phrase($"{summary.TotalHoursLogged:F1}", normalFont)) { Border = Rectangle.NO_BORDER });

                timeTable.AddCell(new PdfPCell(new Phrase("Billable Hours:", normalFont)) { Border = Rectangle.NO_BORDER });
                timeTable.AddCell(new PdfPCell(new Phrase($"{summary.BillableHours:F1}", normalFont)) { Border = Rectangle.NO_BORDER });

                document.Add(timeTable);
            }

            // Footer
            document.Add(new Paragraph($"\nGenerated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC", FontFactory.GetFont(FontFactory.HELVETICA, 8))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 20
            });

            document.Close();

            var fileName = $"Project_Summary_{summary.ProjectName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

            _logger.LogInformation("Exported project summary to PDF for project {ProjectId}", projectId);

            return File(stream.ToArray(), "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting project summary to PDF for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while exporting the project summary");
        }
    }

    /// <summary>
    /// Export project progress to Excel
    /// </summary>
    [HttpGet("export/progress/{projectId}/excel")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportProjectProgressToExcel(Guid projectId)
    {
        try
        {
            var progressQuery = new GetProjectProgressQuery { ProjectId = projectId };
            var progress = await _mediator.Send(progressQuery);

            if (progress == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            var taskQuery = new GetProjectTaskReportQuery { ProjectId = projectId };
            var taskReport = await _mediator.Send(taskQuery);

            using var workbook = new XLWorkbook();

            // Project Overview Sheet
            var overviewSheet = workbook.Worksheets.Add("Project Overview");

            overviewSheet.Cell(1, 1).Value = "Project Name";
            overviewSheet.Cell(1, 2).Value = progress.ProjectName;
            overviewSheet.Cell(2, 1).Value = "Overall Progress";
            overviewSheet.Cell(2, 2).Value = $"{progress.OverallProgress:F1}%";
            overviewSheet.Cell(3, 1).Value = "Status";
            overviewSheet.Cell(3, 2).Value = progress.Status.ToString();
            overviewSheet.Cell(4, 1).Value = "Start Date";
            overviewSheet.Cell(4, 2).Value = progress.StartDate;
            overviewSheet.Cell(5, 1).Value = "End Date";
            overviewSheet.Cell(5, 2).Value = progress.EndDate;

            // Format headers
            overviewSheet.Range(1, 1, 5, 1).Style.Font.Bold = true;
            overviewSheet.Range(1, 1, 5, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Tasks by Status Sheet
            if (taskReport?.TasksByStatus != null)
            {
                var tasksSheet = workbook.Worksheets.Add("Tasks by Status");

                tasksSheet.Cell(1, 1).Value = "Status";
                tasksSheet.Cell(1, 2).Value = "Count";
                tasksSheet.Cell(1, 3).Value = "Percentage";

                var headerRange = tasksSheet.Range(1, 1, 1, 3);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var statusGroup in taskReport.TasksByStatus)
                {
                    tasksSheet.Cell(row, 1).Value = statusGroup.Status.ToString();
                    tasksSheet.Cell(row, 2).Value = statusGroup.Count;
                    tasksSheet.Cell(row, 3).Value = $"{statusGroup.Percentage:F1}%";
                    row++;
                }
            }

            // Milestones Sheet
            if (progress.Milestones?.Any() == true)
            {
                var milestonesSheet = workbook.Worksheets.Add("Milestones");

                milestonesSheet.Cell(1, 1).Value = "Title";
                milestonesSheet.Cell(1, 2).Value = "Due Date";
                milestonesSheet.Cell(1, 3).Value = "Status";
                milestonesSheet.Cell(1, 4).Value = "Progress %";

                var headerRange = milestonesSheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var milestone in progress.Milestones)
                {
                    milestonesSheet.Cell(row, 1).Value = milestone.Title;
                    milestonesSheet.Cell(row, 2).Value = milestone.DueDate;
                    milestonesSheet.Cell(row, 3).Value = milestone.Status.ToString();
                    milestonesSheet.Cell(row, 4).Value = milestone.CompletionPercentage;
                    row++;
                }
            }

            // Auto-fit columns
            foreach (var worksheet in workbook.Worksheets)
            {
                worksheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Project_Progress_{progress.ProjectName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            _logger.LogInformation("Exported project progress to Excel for project {ProjectId}", projectId);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting project progress to Excel for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while exporting the project progress");
        }
    }

    /// <summary>
    /// Export comprehensive project report to Excel
    /// </summary>
    [HttpGet("export/comprehensive/{projectId}/excel")]
    [Authorize(Roles = "Admin,ProjectManager,TeamLead")]
    public async Task<IActionResult> ExportComprehensiveProjectReport(Guid projectId)
    {
        try
        {
            // Get all report data
            var summaryQuery = new GetProjectSummaryQuery { ProjectId = projectId };
            var summary = await _mediator.Send(summaryQuery);

            if (summary == null)
            {
                return NotFound($"Project with ID {projectId} not found");
            }

            var progressQuery = new GetProjectProgressQuery { ProjectId = projectId };
            var progress = await _mediator.Send(progressQuery);

            var timeQuery = new GetProjectTimeReportQuery { ProjectId = projectId };
            var timeReport = await _mediator.Send(timeQuery);

            var taskQuery = new GetProjectTaskReportQuery { ProjectId = projectId };
            var taskReport = await _mediator.Send(taskQuery);

            var milestoneQuery = new GetProjectMilestoneReportQuery { ProjectId = projectId };
            var milestoneReport = await _mediator.Send(milestoneQuery);

            using var workbook = new XLWorkbook();

            // Summary Sheet
            var summarySheet = workbook.Worksheets.Add("Project Summary");
            
            summarySheet.Cell(1, 1).Value = "Project Comprehensive Report";
            summarySheet.Cell(1, 1).Style.Font.Bold = true;
            summarySheet.Cell(1, 1).Style.Font.FontSize = 16;

            summarySheet.Cell(3, 1).Value = "Project Name";
            summarySheet.Cell(3, 2).Value = summary.ProjectName;
            summarySheet.Cell(4, 1).Value = "Status";
            summarySheet.Cell(4, 2).Value = summary.Status.ToString();
            summarySheet.Cell(5, 1).Value = "Progress";
            summarySheet.Cell(5, 2).Value = $"{summary.CompletionPercentage:F1}%";
            summarySheet.Cell(6, 1).Value = "Start Date";
            summarySheet.Cell(6, 2).Value = summary.StartDate;
            summarySheet.Cell(7, 1).Value = "End Date";
            summarySheet.Cell(7, 2).Value = summary.EndDate;

            if (summary.Budget.HasValue)
            {
                summarySheet.Cell(8, 1).Value = "Budget";
                summarySheet.Cell(8, 2).Value = summary.Budget.Value;
                summarySheet.Cell(8, 2).Style.NumberFormat.Format = "$#,##0.00";
            }

            // Time Report Sheet
            if (timeReport != null)
            {
                var timeSheet = workbook.Worksheets.Add("Time Report");
                
                timeSheet.Cell(1, 1).Value = "Total Hours";
                timeSheet.Cell(1, 2).Value = timeReport.TotalHours;
                timeSheet.Cell(2, 1).Value = "Billable Hours";
                timeSheet.Cell(2, 2).Value = timeReport.BillableHours;
                timeSheet.Cell(3, 1).Value = "Non-Billable Hours";
                timeSheet.Cell(3, 2).Value = timeReport.NonBillableHours;

                if (timeReport.TimeByUser?.Any() == true)
                {
                    timeSheet.Cell(5, 1).Value = "Time by User";
                    timeSheet.Cell(5, 1).Style.Font.Bold = true;
                    
                    timeSheet.Cell(6, 1).Value = "User";
                    timeSheet.Cell(6, 2).Value = "Hours";
                    
                    int row = 7;
                    foreach (var userTime in timeReport.TimeByUser)
                    {
                        timeSheet.Cell(row, 1).Value = userTime.UserName;
                        timeSheet.Cell(row, 2).Value = userTime.Hours;
                        row++;
                    }
                }
            }

            // Task Statistics Sheet
            if (taskReport != null)
            {
                var taskSheet = workbook.Worksheets.Add("Task Statistics");
                
                taskSheet.Cell(1, 1).Value = "Total Tasks";
                taskSheet.Cell(1, 2).Value = taskReport.TotalTasks;
                taskSheet.Cell(2, 1).Value = "Completed Tasks";
                taskSheet.Cell(2, 2).Value = taskReport.CompletedTasks;
                taskSheet.Cell(3, 1).Value = "In Progress Tasks";
                taskSheet.Cell(3, 2).Value = taskReport.InProgressTasks;
                taskSheet.Cell(4, 1).Value = "Pending Tasks";
                taskSheet.Cell(4, 2).Value = taskReport.PendingTasks;
                taskSheet.Cell(5, 1).Value = "Overdue Tasks";
                taskSheet.Cell(5, 2).Value = taskReport.OverdueTasks;
            }

            // Milestone Report Sheet
            if (milestoneReport != null)
            {
                var milestoneSheet = workbook.Worksheets.Add("Milestone Report");
                
                milestoneSheet.Cell(1, 1).Value = "Total Milestones";
                milestoneSheet.Cell(1, 2).Value = milestoneReport.TotalMilestones;
                milestoneSheet.Cell(2, 1).Value = "Completed Milestones";
                milestoneSheet.Cell(2, 2).Value = milestoneReport.CompletedMilestones;
                milestoneSheet.Cell(3, 1).Value = "Pending Milestones";
                milestoneSheet.Cell(3, 2).Value = milestoneReport.PendingMilestones;
                milestoneSheet.Cell(4, 1).Value = "Overdue Milestones";
                milestoneSheet.Cell(4, 2).Value = milestoneReport.OverdueMilestones;
            }

            // Auto-fit columns
            foreach (var worksheet in workbook.Worksheets)
            {
                worksheet.Columns().AdjustToContents();
                
                // Format headers
                var usedRange = worksheet.RangeUsed();
                if (usedRange != null)
                {
                    var headerColumn = worksheet.Column(1);
                    headerColumn.Style.Font.Bold = true;
                    headerColumn.Style.Fill.BackgroundColor = XLColor.LightGray;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Comprehensive_Project_Report_{summary.ProjectName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            _logger.LogInformation("Exported comprehensive project report to Excel for project {ProjectId}", projectId);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting comprehensive project report for {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while exporting the comprehensive project report");
        }
    }
}
