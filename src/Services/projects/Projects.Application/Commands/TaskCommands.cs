namespace TossErp.Projects.Application.Commands;

/// <summary>
/// Command to create a new project task
/// </summary>
public record CreateProjectTaskCommand(
    Guid ProjectId,
    string Title,
    string Description,
    TaskPriority Priority,
    Guid? AssigneeId = null,
    Guid? ParentTaskId = null,
    DateOnly? StartDate = null,
    DateOnly? DueDate = null,
    decimal EstimatedHours = 0,
    List<string>? Tags = null
) : IRequest<Guid>;

/// <summary>
/// Validator for CreateProjectTaskCommand
/// </summary>
public class CreateProjectTaskCommandValidator : AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Task description is required")
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleFor(x => x.DueDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue)
            .WithMessage("Due date must be after start date");

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated hours cannot be negative");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters")
            .When(x => x.Tags != null);
    }
}

/// <summary>
/// Handler for CreateProjectTaskCommand
/// </summary>
public class CreateProjectTaskCommandHandler : IRequestHandler<CreateProjectTaskCommand, Guid>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<CreateProjectTaskCommandHandler> _logger;

    public CreateProjectTaskCommandHandler(
        IProjectTaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<CreateProjectTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProjectTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new project task: {TaskTitle} for project: {ProjectId}", 
            request.Title, request.ProjectId);

        // Verify project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        // Verify parent task exists if specified
        ProjectTask? parentTask = null;
        if (request.ParentTaskId.HasValue)
        {
            parentTask = await _taskRepository.GetByIdAsync(request.ParentTaskId.Value, cancellationToken);
            if (parentTask == null)
            {
                throw new NotFoundException(nameof(ProjectTask), request.ParentTaskId.Value);
            }

            if (parentTask.ProjectId != request.ProjectId)
            {
                throw new InvalidOperationException("Parent task must belong to the same project");
            }
        }

        // Create task entity
        var task = new ProjectTask(
            request.Title,
            request.Description,
            request.ProjectId,
            request.Priority,
            request.AssigneeId,
            request.ParentTaskId,
            request.StartDate,
            request.DueDate,
            request.EstimatedHours);

        // Add tags if provided
        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                task.AddTag(tag);
            }
        }

        // Add task to repository
        await _taskRepository.AddAsync(task, cancellationToken);

        // Publish domain event
        task.AddDomainEvent(new TaskCreatedEvent(task));

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification if task is assigned
        if (request.AssigneeId.HasValue)
        {
            await _notificationService.SendTaskAssignedNotificationAsync(task, cancellationToken);
        }

        _logger.LogInformation("Project task created successfully with ID: {TaskId}", task.Id);

        return task.Id;
    }
}

/// <summary>
/// Command to update a project task
/// </summary>
public record UpdateProjectTaskCommand(
    Guid Id,
    string Title,
    string Description,
    TaskPriority Priority,
    Guid? AssigneeId = null,
    DateOnly? StartDate = null,
    DateOnly? DueDate = null,
    decimal EstimatedHours = 0,
    int ProgressPercentage = 0,
    List<string>? Tags = null
) : IRequest;

/// <summary>
/// Validator for UpdateProjectTaskCommand
/// </summary>
public class UpdateProjectTaskCommandValidator : AbstractValidator<UpdateProjectTaskCommand>
{
    public UpdateProjectTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Task description is required")
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleFor(x => x.DueDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue)
            .WithMessage("Due date must be after start date");

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated hours cannot be negative");

        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters")
            .When(x => x.Tags != null);
    }
}

/// <summary>
/// Handler for UpdateProjectTaskCommand
/// </summary>
public class UpdateProjectTaskCommandHandler : IRequestHandler<UpdateProjectTaskCommand>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<UpdateProjectTaskCommandHandler> _logger;

    public UpdateProjectTaskCommandHandler(
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<UpdateProjectTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(UpdateProjectTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating project task: {TaskId}", request.Id);

        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(ProjectTask), request.Id);
        }

        var oldAssigneeId = task.AssigneeId;

        // Update task properties
        task.UpdateDetails(
            request.Title,
            request.Description,
            request.Priority,
            request.StartDate,
            request.DueDate,
            request.EstimatedHours);

        // Update assignee if changed
        if (request.AssigneeId != oldAssigneeId)
        {
            task.AssignTo(request.AssigneeId);
        }

        // Update progress
        task.UpdateProgress(request.ProgressPercentage);

        // Update tags
        task.ClearTags();
        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                task.AddTag(tag);
            }
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification if assignee changed
        if (request.AssigneeId != oldAssigneeId && request.AssigneeId.HasValue)
        {
            await _notificationService.SendTaskAssignedNotificationAsync(task, cancellationToken);
        }

        _logger.LogInformation("Project task updated successfully: {TaskId}", request.Id);
    }
}

/// <summary>
/// Command to change task status
/// </summary>
public record ChangeTaskStatusCommand(
    Guid Id,
    TaskStatus Status,
    string? Comment = null
) : IRequest;

/// <summary>
/// Validator for ChangeTaskStatusCommand
/// </summary>
public class ChangeTaskStatusCommandValidator : AbstractValidator<ChangeTaskStatusCommand>
{
    public ChangeTaskStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}

/// <summary>
/// Handler for ChangeTaskStatusCommand
/// </summary>
public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeTaskStatusCommandHandler> _logger;

    public ChangeTaskStatusCommandHandler(
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeTaskStatusCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing task status: {TaskId} to {Status}", request.Id, request.Status);

        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(ProjectTask), request.Id);
        }

        // Update status based on new status
        switch (request.Status)
        {
            case TaskStatus.InProgress:
                task.Start();
                break;
            case TaskStatus.Completed:
                task.Complete();
                break;
            case TaskStatus.Cancelled:
                task.Cancel(request.Comment);
                break;
            case TaskStatus.OnHold:
                task.Pause(request.Comment);
                break;
            default:
                task.ChangeStatus(request.Status);
                break;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task status changed successfully: {TaskId} to {Status}", request.Id, request.Status);
    }
}

/// <summary>
/// Command to assign task to user
/// </summary>
public record AssignTaskCommand(
    Guid TaskId,
    Guid? AssigneeId
) : IRequest;

/// <summary>
/// Validator for AssignTaskCommand
/// </summary>
public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");
    }
}

/// <summary>
/// Handler for AssignTaskCommand
/// </summary>
public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<AssignTaskCommandHandler> _logger;

    public AssignTaskCommandHandler(
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<AssignTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning task: {TaskId} to user: {AssigneeId}", 
            request.TaskId, request.AssigneeId);

        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(ProjectTask), request.TaskId);
        }

        task.AssignTo(request.AssigneeId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification if task is assigned to someone
        if (request.AssigneeId.HasValue)
        {
            await _notificationService.SendTaskAssignedNotificationAsync(task, cancellationToken);
        }

        _logger.LogInformation("Task assigned successfully: {TaskId}", request.TaskId);
    }
}

/// <summary>
/// Command to log time entry for a task
/// </summary>
public record LogTimeEntryCommand(
    Guid ProjectId,
    Guid? TaskId,
    Guid UserId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    decimal? Hours,
    string Description,
    bool IsBillable = true,
    decimal? HourlyRate = null,
    string Currency = "USD"
) : IRequest<Guid>;

/// <summary>
/// Validator for LogTimeEntryCommand
/// </summary>
public class LogTimeEntryCommandValidator : AbstractValidator<LogTimeEntryCommand>
{
    public LogTimeEntryCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.EndTime.HasValue)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.Hours)
            .GreaterThan(0).WithMessage("Hours must be greater than zero")
            .LessThanOrEqualTo(24).WithMessage("Hours cannot exceed 24 per day")
            .When(x => x.Hours.HasValue);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hourly rate cannot be negative")
            .When(x => x.HourlyRate.HasValue);

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");

        // Either EndTime or Hours must be provided
        RuleFor(x => x)
            .Must(x => x.EndTime.HasValue || x.Hours.HasValue)
            .WithMessage("Either end time or hours must be provided");
    }
}

/// <summary>
/// Handler for LogTimeEntryCommand
/// </summary>
public class LogTimeEntryCommandHandler : IRequestHandler<LogTimeEntryCommand, Guid>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogTimeEntryCommandHandler> _logger;

    public LogTimeEntryCommandHandler(
        ITimeEntryRepository timeEntryRepository,
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ILogger<LogTimeEntryCommandHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(LogTimeEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging time entry for project: {ProjectId}, task: {TaskId}, user: {UserId}", 
            request.ProjectId, request.TaskId, request.UserId);

        // Verify project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        // Verify task exists if specified
        if (request.TaskId.HasValue)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId.Value, cancellationToken);
            if (task == null)
            {
                throw new NotFoundException(nameof(ProjectTask), request.TaskId.Value);
            }

            if (task.ProjectId != request.ProjectId)
            {
                throw new InvalidOperationException("Task must belong to the specified project");
            }
        }

        // Calculate hours if not provided
        var hours = request.Hours;
        if (!hours.HasValue && request.EndTime.HasValue)
        {
            var timeSpan = request.EndTime.Value - request.StartTime;
            hours = (decimal)timeSpan.TotalHours;
        }

        // Create time entry
        var timeEntry = new TimeEntry(
            request.ProjectId,
            request.TaskId,
            request.UserId,
            request.Date,
            request.StartTime,
            request.EndTime,
            hours!.Value,
            request.Description,
            request.IsBillable,
            request.HourlyRate.HasValue ? new Money(request.HourlyRate.Value, request.Currency) : null);

        // Add time entry to repository
        await _timeEntryRepository.AddAsync(timeEntry, cancellationToken);

        // Publish domain event
        timeEntry.AddDomainEvent(new TimeEntryLoggedEvent(timeEntry));

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Time entry logged successfully with ID: {TimeEntryId}", timeEntry.Id);

        return timeEntry.Id;
    }
}
