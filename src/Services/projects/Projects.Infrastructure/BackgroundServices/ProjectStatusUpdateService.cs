using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;

namespace Projects.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for automatically updating project statuses based on task completion and deadlines
/// </summary>
public class ProjectStatusUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ProjectStatusUpdateService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30); // Run every 30 minutes

    public ProjectStatusUpdateService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ProjectStatusUpdateService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Project Status Update Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessProjectStatusUpdates(stoppingToken);
                
                _logger.LogDebug("Project status update cycle completed. Next run in {Interval}", _interval);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Project Status Update Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Project Status Update Service");
                
                // Wait a shorter interval before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task ProcessProjectStatusUpdates(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

            _logger.LogDebug("Starting project status update process");

            // Get all active projects that might need status updates
            var activeProjects = await projectRepository.GetActiveProjectsAsync();

            foreach (var project in activeProjects)
            {
                try
                {
                    var projectTasks = await taskRepository.GetTasksByProjectIdAsync(project.Id);
                    var shouldUpdate = false;
                    var newStatus = project.Status;

                    // Check if project should be marked as completed
                    if (project.Status != ProjectStatus.Completed)
                    {
                        var allTasksCompleted = projectTasks.Any() && 
                                               projectTasks.All(t => t.Status == TaskStatus.Completed);
                        
                        if (allTasksCompleted)
                        {
                            newStatus = ProjectStatus.Completed;
                            shouldUpdate = true;
                            _logger.LogInformation("Project {ProjectId} ({ProjectName}) marked as completed - all tasks finished", 
                                project.Id, project.Name);
                        }
                    }

                    // Check if project should be marked as at risk
                    if (project.Status == ProjectStatus.InProgress)
                    {
                        var overdueTasks = projectTasks.Count(t => 
                            t.Status != TaskStatus.Completed && 
                            t.DueDate.HasValue && 
                            t.DueDate.Value < DateTime.UtcNow);

                        var projectOverdue = project.DueDate.HasValue && 
                                           project.DueDate.Value < DateTime.UtcNow &&
                                           project.Status != ProjectStatus.Completed;

                        if (overdueTasks > 0 || projectOverdue)
                        {
                            newStatus = ProjectStatus.AtRisk;
                            shouldUpdate = true;
                            _logger.LogInformation("Project {ProjectId} ({ProjectName}) marked as at risk - {OverdueTasks} overdue tasks, project overdue: {ProjectOverdue}", 
                                project.Id, project.Name, overdueTasks, projectOverdue);
                        }
                    }

                    // Check if project should be marked as on hold
                    if (project.Status == ProjectStatus.InProgress)
                    {
                        var blockedTasks = projectTasks.Count(t => 
                            t.Status == TaskStatus.Blocked || t.Status == TaskStatus.OnHold);

                        var activeTasksCount = projectTasks.Count(t => 
                            t.Status == TaskStatus.InProgress || t.Status == TaskStatus.ToDo);

                        // If more than 50% of tasks are blocked/on hold and no active tasks
                        if (blockedTasks > 0 && activeTasksCount == 0 && blockedTasks >= projectTasks.Count / 2)
                        {
                            newStatus = ProjectStatus.OnHold;
                            shouldUpdate = true;
                            _logger.LogInformation("Project {ProjectId} ({ProjectName}) marked as on hold - {BlockedTasks} blocked tasks, no active tasks", 
                                project.Id, project.Name, blockedTasks);
                        }
                    }

                    // Update project status if needed
                    if (shouldUpdate && newStatus != project.Status)
                    {
                        project.UpdateStatus(newStatus);
                        project.SetLastModified(DateTime.UtcNow, "system"); // System user for automated updates
                        
                        await projectRepository.UpdateAsync(project);
                        
                        _logger.LogInformation("Updated project {ProjectId} status from {OldStatus} to {NewStatus}", 
                            project.Id, project.Status, newStatus);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating status for project {ProjectId}", project.Id);
                    // Continue with other projects
                }
            }

            // Save all changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug("Project status update process completed for {ProjectCount} projects", activeProjects.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessProjectStatusUpdates");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project Status Update Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
