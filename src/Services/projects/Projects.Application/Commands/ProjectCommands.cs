namespace TossErp.Projects.Application.Commands;

/// <summary>
/// Command to create a new project
/// </summary>
public record CreateProjectCommand(
    string Name,
    string Description,
    Guid CustomerId,
    Guid ProjectManagerId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal Budget,
    string Currency,
    ProjectPriority Priority = ProjectPriority.Medium,
    List<string>? Tags = null
) : IRequest<Guid>;

/// <summary>
/// Validator for CreateProjectCommand
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Project description is required")
            .MaximumLength(2000).WithMessage("Project description cannot exceed 2000 characters");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.ProjectManagerId)
            .NotEmpty().WithMessage("Project Manager ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)))
            .WithMessage("Start date cannot be more than 30 days in the past");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters")
            .When(x => x.Tags != null);
    }
}

/// <summary>
/// Handler for CreateProjectCommand
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IProjectNotificationService notificationService,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new project: {ProjectName}", request.Name);

        // Create project entity
        var project = new Project(
            request.Name,
            request.Description,
            request.CustomerId,
            request.ProjectManagerId,
            request.StartDate,
            request.EndDate,
            new Money(request.Budget, request.Currency),
            request.Priority);

        // Add tags if provided
        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                project.AddTag(tag);
            }
        }

        // Add project to repository
        await _projectRepository.AddAsync(project, cancellationToken);

        // Publish domain event
        project.AddDomainEvent(new ProjectCreatedEvent(project));

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification
        await _notificationService.SendProjectCreatedNotificationAsync(project, cancellationToken);

        _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);

        return project.Id;
    }
}

/// <summary>
/// Command to update a project
/// </summary>
public record UpdateProjectCommand(
    Guid Id,
    string Name,
    string Description,
    DateOnly? EndDate,
    decimal Budget,
    string Currency,
    ProjectPriority Priority,
    List<string>? Tags = null
) : IRequest;

/// <summary>
/// Validator for UpdateProjectCommand
/// </summary>
public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Project description is required")
            .MaximumLength(2000).WithMessage("Project description cannot exceed 2000 characters");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters")
            .When(x => x.Tags != null);
    }
}

/// <summary>
/// Handler for UpdateProjectCommand
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating project: {ProjectId}", request.Id);

        var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        // Update project properties
        project.UpdateDetails(
            request.Name,
            request.Description,
            request.EndDate,
            new Money(request.Budget, request.Currency),
            request.Priority);

        // Update tags
        project.ClearTags();
        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                project.AddTag(tag);
            }
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project updated successfully: {ProjectId}", request.Id);
    }
}

/// <summary>
/// Command to change project status
/// </summary>
public record ChangeProjectStatusCommand(
    Guid Id,
    ProjectStatus Status,
    string? Reason = null
) : IRequest;

/// <summary>
/// Validator for ChangeProjectStatusCommand
/// </summary>
public class ChangeProjectStatusCommandValidator : AbstractValidator<ChangeProjectStatusCommand>
{
    public ChangeProjectStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

/// <summary>
/// Handler for ChangeProjectStatusCommand
/// </summary>
public class ChangeProjectStatusCommandHandler : IRequestHandler<ChangeProjectStatusCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<ChangeProjectStatusCommandHandler> _logger;

    public ChangeProjectStatusCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<ChangeProjectStatusCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing project status: {ProjectId} to {Status}", request.Id, request.Status);

        var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        var oldStatus = project.Status;

        // Update status based on new status
        switch (request.Status)
        {
            case ProjectStatus.Active:
                project.Start();
                break;
            case ProjectStatus.OnHold:
                project.Pause(request.Reason);
                break;
            case ProjectStatus.Completed:
                project.Complete();
                break;
            case ProjectStatus.Cancelled:
                project.Cancel(request.Reason);
                break;
            default:
                throw new InvalidOperationException($"Invalid status transition to {request.Status}");
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification if status changed
        if (oldStatus != project.Status)
        {
            await _notificationService.SendProjectStatusChangedNotificationAsync(project, oldStatus, cancellationToken);
        }

        _logger.LogInformation("Project status changed successfully: {ProjectId} from {OldStatus} to {NewStatus}", 
            request.Id, oldStatus, project.Status);
    }
}

/// <summary>
/// Command to assign project manager
/// </summary>
public record AssignProjectManagerCommand(
    Guid ProjectId,
    Guid ProjectManagerId
) : IRequest;

/// <summary>
/// Validator for AssignProjectManagerCommand
/// </summary>
public class AssignProjectManagerCommandValidator : AbstractValidator<AssignProjectManagerCommand>
{
    public AssignProjectManagerCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.ProjectManagerId)
            .NotEmpty().WithMessage("Project Manager ID is required");
    }
}

/// <summary>
/// Handler for AssignProjectManagerCommand
/// </summary>
public class AssignProjectManagerCommandHandler : IRequestHandler<AssignProjectManagerCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignProjectManagerCommandHandler> _logger;

    public AssignProjectManagerCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignProjectManagerCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AssignProjectManagerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning project manager: {ProjectManagerId} to project: {ProjectId}", 
            request.ProjectManagerId, request.ProjectId);

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        project.AssignProjectManager(request.ProjectManagerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project manager assigned successfully: {ProjectId}", request.ProjectId);
    }
}

/// <summary>
/// Command to create project from template
/// </summary>
public record CreateProjectFromTemplateCommand(
    Guid TemplateId,
    string ProjectName,
    string Description,
    Guid CustomerId,
    Guid ProjectManagerId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal Budget,
    string Currency
) : IRequest<Guid>;

/// <summary>
/// Validator for CreateProjectFromTemplateCommand
/// </summary>
public class CreateProjectFromTemplateCommandValidator : AbstractValidator<CreateProjectFromTemplateCommand>
{
    public CreateProjectFromTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId)
            .NotEmpty().WithMessage("Template ID is required");

        RuleFor(x => x.ProjectName)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Project description is required")
            .MaximumLength(2000).WithMessage("Project description cannot exceed 2000 characters");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.ProjectManagerId)
            .NotEmpty().WithMessage("Project Manager ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");
    }
}

/// <summary>
/// Handler for CreateProjectFromTemplateCommand
/// </summary>
public class CreateProjectFromTemplateCommandHandler : IRequestHandler<CreateProjectFromTemplateCommand, Guid>
{
    private readonly IProjectTemplateService _templateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<CreateProjectFromTemplateCommandHandler> _logger;

    public CreateProjectFromTemplateCommandHandler(
        IProjectTemplateService templateService,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<CreateProjectFromTemplateCommandHandler> logger)
    {
        _templateService = templateService;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProjectFromTemplateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating project from template: {TemplateId}", request.TemplateId);

        var templateRequest = new CreateProjectFromTemplateRequest
        {
            ProjectName = request.ProjectName,
            Description = request.Description,
            CustomerId = request.CustomerId,
            ProjectManagerId = request.ProjectManagerId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Budget = request.Budget,
            Currency = request.Currency
        };

        var project = await _templateService.CreateProjectFromTemplateAsync(request.TemplateId, templateRequest, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.SendProjectCreatedNotificationAsync(project, cancellationToken);

        _logger.LogInformation("Project created from template successfully with ID: {ProjectId}", project.Id);

        return project.Id;
    }
}
