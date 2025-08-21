using AutoMapper;
using TossErp.Projects.Domain.Entities;
using TossErp.Projects.Domain.Enums;
using TossErp.Projects.Application.DTOs;

namespace TossErp.Projects.Application.Mapping;

/// <summary>
/// AutoMapper profile for Projects Application
/// </summary>
public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateProjectMappings();
        CreateProjectTaskMappings();
        CreateTimeEntryMappings();
        CreateResourceMappings();
        CreateMilestoneMappings();
        CreateReportingMappings();
    }

    private void CreateProjectMappings()
    {
        // Project mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ProgressPercentage, opt => opt.MapFrom(s => CalculateProjectProgress(s)))
            .ForMember(d => d.TotalHours, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Sum(te => te.Hours)))
            .ForMember(d => d.TotalTasks, opt => opt.MapFrom(s => s.Tasks.Count))
            .ForMember(d => d.CompletedTasks, opt => opt.MapFrom(s => s.Tasks.Count(t => t.Status == TaskStatus.Completed)))
            .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.EndDate.HasValue && s.EndDate.Value < DateTime.UtcNow && s.Status != ProjectStatus.Completed))
            .ForMember(d => d.DaysRemaining, opt => opt.MapFrom(s => s.EndDate.HasValue ? (int)(s.EndDate.Value - DateTime.UtcNow).TotalDays : (int?)null))
            .ForMember(d => d.SpentAmount, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Where(te => te.BillableRate.HasValue).Sum(te => te.Hours * te.BillableRate.Value)))
            .ForMember(d => d.ResourceCount, opt => opt.MapFrom(s => s.ResourceAssignments.Count));

        CreateMap<Project, ProjectSummaryDto>()
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ProgressPercentage, opt => opt.MapFrom(s => CalculateProjectProgress(s)))
            .ForMember(d => d.TotalHours, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Sum(te => te.Hours)))
            .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.EndDate.HasValue && s.EndDate.Value < DateTime.UtcNow && s.Status != ProjectStatus.Completed));

        CreateMap<ProjectTemplate, ProjectTemplateDto>()
            .ForMember(d => d.TaskTemplates, opt => opt.MapFrom(s => s.TaskTemplates));

        CreateMap<ProjectTaskTemplate, ProjectTaskTemplateDto>();

        // Reverse mappings for create/update commands
        CreateMap<CreateProjectCommand, Project>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(s => ProjectStatus.Planning))
            .ForMember(d => d.Tasks, opt => opt.Ignore())
            .ForMember(d => d.ResourceAssignments, opt => opt.Ignore())
            .ForMember(d => d.Milestones, opt => opt.Ignore());

        CreateMap<UpdateProjectCommand, Project>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.Tasks, opt => opt.Ignore())
            .ForMember(d => d.ResourceAssignments, opt => opt.Ignore())
            .ForMember(d => d.Milestones, opt => opt.Ignore());
    }

    private void CreateProjectTaskMappings()
    {
        // ProjectTask mappings
        CreateMap<ProjectTask, ProjectTaskDto>()
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PriorityName, opt => opt.MapFrom(s => s.Priority.ToString()))
            .ForMember(d => d.ProgressPercentage, opt => opt.MapFrom(s => s.ProgressPercentage))
            .ForMember(d => d.TotalHours, opt => opt.MapFrom(s => s.TimeEntries.Sum(te => te.Hours)))
            .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow && s.Status != TaskStatus.Completed))
            .ForMember(d => d.DaysRemaining, opt => opt.MapFrom(s => s.DueDate.HasValue ? (int)(s.DueDate.Value - DateTime.UtcNow).TotalDays : (int?)null))
            .ForMember(d => d.SubtaskCount, opt => opt.MapFrom(s => s.Subtasks.Count))
            .ForMember(d => d.CompletedSubtasks, opt => opt.MapFrom(s => s.Subtasks.Count(st => st.Status == TaskStatus.Completed)))
            .ForMember(d => d.HasSubtasks, opt => opt.MapFrom(s => s.Subtasks.Any()));

        // Reverse mappings
        CreateMap<CreateProjectTaskCommand, ProjectTask>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(s => TaskStatus.NotStarted))
            .ForMember(d => d.ProgressPercentage, opt => opt.MapFrom(s => 0))
            .ForMember(d => d.Project, opt => opt.Ignore())
            .ForMember(d => d.ParentTask, opt => opt.Ignore())
            .ForMember(d => d.Subtasks, opt => opt.Ignore())
            .ForMember(d => d.TimeEntries, opt => opt.Ignore())
            .ForMember(d => d.Dependencies, opt => opt.Ignore())
            .ForMember(d => d.DependentTasks, opt => opt.Ignore());

        CreateMap<UpdateProjectTaskCommand, ProjectTask>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.ProjectId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Project, opt => opt.Ignore())
            .ForMember(d => d.ParentTask, opt => opt.Ignore())
            .ForMember(d => d.Subtasks, opt => opt.Ignore())
            .ForMember(d => d.TimeEntries, opt => opt.Ignore())
            .ForMember(d => d.Dependencies, opt => opt.Ignore())
            .ForMember(d => d.DependentTasks, opt => opt.Ignore());
    }

    private void CreateTimeEntryMappings()
    {
        // TimeEntry mappings
        CreateMap<TimeEntry, TimeEntryDto>()
            .ForMember(d => d.BillableAmount, opt => opt.MapFrom(s => s.BillableRate.HasValue ? s.Hours * s.BillableRate.Value : (decimal?)null));

        CreateMap<LogTimeEntryCommand, TimeEntry>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Project, opt => opt.Ignore())
            .ForMember(d => d.Task, opt => opt.Ignore());
    }

    private void CreateResourceMappings()
    {
        // Resource mappings
        CreateMap<Resource, ResourceDto>()
            .ForMember(d => d.TypeName, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.AvailabilityStatusName, opt => opt.MapFrom(s => s.AvailabilityStatus.ToString()))
            .ForMember(d => d.ProjectCount, opt => opt.MapFrom(s => s.ProjectAssignments.Count))
            .ForMember(d => d.CurrentUtilization, opt => opt.MapFrom(s => CalculateResourceUtilization(s)));

        CreateMap<ResourceAssignment, ResourceAssignmentDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role.ToString()));

        // Reverse mappings
        CreateMap<CreateResourceCommand, Resource>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.AvailabilityStatus, opt => opt.MapFrom(s => ResourceAvailabilityStatus.Available))
            .ForMember(d => d.ProjectAssignments, opt => opt.Ignore());

        CreateMap<UpdateResourceCommand, Resource>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.ProjectAssignments, opt => opt.Ignore());
    }

    private void CreateMilestoneMappings()
    {
        // Milestone mappings
        CreateMap<Milestone, MilestoneDto>()
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.DueDate < DateTime.UtcNow && s.Status != MilestoneStatus.Completed))
            .ForMember(d => d.DaysRemaining, opt => opt.MapFrom(s => (int)(s.DueDate - DateTime.UtcNow).TotalDays))
            .ForMember(d => d.DependentTaskCount, opt => opt.MapFrom(s => s.DependentTasks.Count))
            .ForMember(d => d.CompletedTaskCount, opt => opt.MapFrom(s => s.DependentTasks.Count(t => t.Status == TaskStatus.Completed)));

        // Reverse mappings
        CreateMap<CreateMilestoneCommand, Milestone>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(s => MilestoneStatus.NotStarted))
            .ForMember(d => d.Project, opt => opt.Ignore())
            .ForMember(d => d.DependentTasks, opt => opt.Ignore());

        CreateMap<UpdateMilestoneCommand, Milestone>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.ProjectId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Project, opt => opt.Ignore())
            .ForMember(d => d.DependentTasks, opt => opt.Ignore());
    }

    private void CreateReportingMappings()
    {
        // Reporting DTOs - these are usually calculated/aggregated
        CreateMap<Project, ProjectProgressReportDto>()
            .ForMember(d => d.ProjectId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.ProjectName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ProgressPercentage, opt => opt.MapFrom(s => CalculateProjectProgress(s)))
            .ForMember(d => d.TotalTasks, opt => opt.MapFrom(s => s.Tasks.Count))
            .ForMember(d => d.CompletedTasks, opt => opt.MapFrom(s => s.Tasks.Count(t => t.Status == TaskStatus.Completed)))
            .ForMember(d => d.InProgressTasks, opt => opt.MapFrom(s => s.Tasks.Count(t => t.Status == TaskStatus.InProgress)))
            .ForMember(d => d.PendingTasks, opt => opt.MapFrom(s => s.Tasks.Count(t => t.Status == TaskStatus.NotStarted)))
            .ForMember(d => d.OverdueTasks, opt => opt.MapFrom(s => s.Tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != TaskStatus.Completed)))
            .ForMember(d => d.TotalHours, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Sum(te => te.Hours)))
            .ForMember(d => d.BillableHours, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Where(te => te.IsBillable).Sum(te => te.Hours)))
            .ForMember(d => d.BillableAmount, opt => opt.MapFrom(s => s.Tasks.SelectMany(t => t.TimeEntries).Where(te => te.BillableRate.HasValue).Sum(te => te.Hours * te.BillableRate.Value)))
            .ForMember(d => d.IsOnTrack, opt => opt.MapFrom(s => CalculateProjectOnTrack(s)))
            .ForMember(d => d.EstimatedCompletionDate, opt => opt.MapFrom(s => CalculateEstimatedCompletion(s)));

        CreateMap<TimeEntry, TimeTrackingReportItemDto>()
            .ForMember(d => d.ProjectName, opt => opt.MapFrom(s => s.Project.Name))
            .ForMember(d => d.TaskName, opt => opt.MapFrom(s => s.Task != null ? s.Task.Title : "General"))
            .ForMember(d => d.BillableAmount, opt => opt.MapFrom(s => s.BillableRate.HasValue ? s.Hours * s.BillableRate.Value : (decimal?)null));

        CreateMap<Resource, ResourceUtilizationItemDto>()
            .ForMember(d => d.ResourceId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.ResourceName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.ResourceType, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.ProjectCount, opt => opt.MapFrom(s => s.ProjectAssignments.Count))
            .ForMember(d => d.UtilizationPercentage, opt => opt.MapFrom(s => CalculateResourceUtilization(s)))
            .ForMember(d => d.AvailableHours, opt => opt.MapFrom(s => s.HourlyRate.HasValue ? 40 : 0)) // Default 40 hours per week
            .ForMember(d => d.AllocatedHours, opt => opt.MapFrom(s => s.ProjectAssignments.Sum(pa => pa.AllocationPercentage) / 100 * 40));
    }

    // Helper methods for calculated properties
    private static decimal CalculateProjectProgress(Project project)
    {
        if (!project.Tasks.Any()) return 0;
        
        var totalTasks = project.Tasks.Count;
        var completedTasks = project.Tasks.Count(t => t.Status == TaskStatus.Completed);
        
        return totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;
    }

    private static decimal CalculateResourceUtilization(Resource resource)
    {
        if (!resource.ProjectAssignments.Any()) return 0;
        
        return resource.ProjectAssignments.Sum(pa => pa.AllocationPercentage);
    }

    private static bool CalculateProjectOnTrack(Project project)
    {
        if (!project.EndDate.HasValue) return true;
        
        var progress = CalculateProjectProgress(project);
        var totalDays = (project.EndDate.Value - project.StartDate).TotalDays;
        var elapsedDays = (DateTime.UtcNow - project.StartDate).TotalDays;
        var expectedProgress = totalDays > 0 ? (decimal)(elapsedDays / totalDays * 100) : 0;
        
        return progress >= expectedProgress * 0.9m; // 90% tolerance
    }

    private static DateTime? CalculateEstimatedCompletion(Project project)
    {
        if (!project.Tasks.Any()) return project.EndDate;
        
        var progress = CalculateProjectProgress(project);
        if (progress == 0) return project.EndDate;
        
        var elapsedDays = (DateTime.UtcNow - project.StartDate).TotalDays;
        var estimatedTotalDays = elapsedDays / ((double)progress / 100);
        
        return project.StartDate.AddDays(estimatedTotalDays);
    }
}
