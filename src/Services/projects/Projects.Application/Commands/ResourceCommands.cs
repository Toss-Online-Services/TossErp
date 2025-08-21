namespace TossErp.Projects.Application.Commands;

/// <summary>
/// Command to create a new resource
/// </summary>
public record CreateResourceCommand(
    string Name,
    string Description,
    ResourceType Type,
    decimal? CostPerHour = null,
    decimal? CostPerDay = null,
    string Currency = "USD",
    string? Location = null,
    string? ContactInfo = null
) : IRequest<Guid>;

/// <summary>
/// Validator for CreateResourceCommand
/// </summary>
public class CreateResourceCommandValidator : AbstractValidator<CreateResourceCommand>
{
    public CreateResourceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(200).WithMessage("Resource name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Resource description is required")
            .MaximumLength(1000).WithMessage("Resource description cannot exceed 1000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid resource type");

        RuleFor(x => x.CostPerHour)
            .GreaterThanOrEqualTo(0).WithMessage("Cost per hour cannot be negative")
            .When(x => x.CostPerHour.HasValue);

        RuleFor(x => x.CostPerDay)
            .GreaterThanOrEqualTo(0).WithMessage("Cost per day cannot be negative")
            .When(x => x.CostPerDay.HasValue);

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactInfo));
    }
}

/// <summary>
/// Handler for CreateResourceCommand
/// </summary>
public class CreateResourceCommandHandler : IRequestHandler<CreateResourceCommand, Guid>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateResourceCommandHandler> _logger;

    public CreateResourceCommandHandler(
        IResourceRepository resourceRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateResourceCommandHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new resource: {ResourceName}", request.Name);

        // Create resource entity
        var resource = new Resource(
            request.Name,
            request.Description,
            request.Type,
            request.CostPerHour.HasValue ? new Money(request.CostPerHour.Value, request.Currency) : null,
            request.CostPerDay.HasValue ? new Money(request.CostPerDay.Value, request.Currency) : null,
            request.Location,
            request.ContactInfo);

        // Add resource to repository
        await _resourceRepository.AddAsync(resource, cancellationToken);

        // Publish domain event
        resource.AddDomainEvent(new ResourceCreatedEvent(resource));

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource created successfully with ID: {ResourceId}", resource.Id);

        return resource.Id;
    }
}

/// <summary>
/// Command to update a resource
/// </summary>
public record UpdateResourceCommand(
    Guid Id,
    string Name,
    string Description,
    ResourceType Type,
    decimal? CostPerHour = null,
    decimal? CostPerDay = null,
    string Currency = "USD",
    string? Location = null,
    string? ContactInfo = null
) : IRequest;

/// <summary>
/// Validator for UpdateResourceCommand
/// </summary>
public class UpdateResourceCommandValidator : AbstractValidator<UpdateResourceCommand>
{
    public UpdateResourceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(200).WithMessage("Resource name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Resource description is required")
            .MaximumLength(1000).WithMessage("Resource description cannot exceed 1000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid resource type");

        RuleFor(x => x.CostPerHour)
            .GreaterThanOrEqualTo(0).WithMessage("Cost per hour cannot be negative")
            .When(x => x.CostPerHour.HasValue);

        RuleFor(x => x.CostPerDay)
            .GreaterThanOrEqualTo(0).WithMessage("Cost per day cannot be negative")
            .When(x => x.CostPerDay.HasValue);

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactInfo));
    }
}

/// <summary>
/// Handler for UpdateResourceCommand
/// </summary>
public class UpdateResourceCommandHandler : IRequestHandler<UpdateResourceCommand>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateResourceCommandHandler> _logger;

    public UpdateResourceCommandHandler(
        IResourceRepository resourceRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateResourceCommandHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating resource: {ResourceId}", request.Id);

        var resource = await _resourceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (resource == null)
        {
            throw new NotFoundException(nameof(Resource), request.Id);
        }

        // Update resource properties
        resource.UpdateDetails(
            request.Name,
            request.Description,
            request.Type,
            request.CostPerHour.HasValue ? new Money(request.CostPerHour.Value, request.Currency) : null,
            request.CostPerDay.HasValue ? new Money(request.CostPerDay.Value, request.Currency) : null,
            request.Location,
            request.ContactInfo);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource updated successfully: {ResourceId}", request.Id);
    }
}

/// <summary>
/// Command to assign resource to project
/// </summary>
public record AssignResourceToProjectCommand(
    Guid ResourceId,
    Guid ProjectId,
    DateOnly StartDate,
    DateOnly? EndDate = null,
    decimal? AllocationPercentage = null
) : IRequest;

/// <summary>
/// Validator for AssignResourceToProjectCommand
/// </summary>
public class AssignResourceToProjectCommandValidator : AbstractValidator<AssignResourceToProjectCommand>
{
    public AssignResourceToProjectCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.AllocationPercentage)
            .InclusiveBetween(0, 100).WithMessage("Allocation percentage must be between 0 and 100")
            .When(x => x.AllocationPercentage.HasValue);
    }
}

/// <summary>
/// Handler for AssignResourceToProjectCommand
/// </summary>
public class AssignResourceToProjectCommandHandler : IRequestHandler<AssignResourceToProjectCommand>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignResourceToProjectCommandHandler> _logger;

    public AssignResourceToProjectCommandHandler(
        IResourceRepository resourceRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignResourceToProjectCommandHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AssignResourceToProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning resource: {ResourceId} to project: {ProjectId}", 
            request.ResourceId, request.ProjectId);

        // Verify resource exists
        var resource = await _resourceRepository.GetByIdAsync(request.ResourceId, cancellationToken);
        if (resource == null)
        {
            throw new NotFoundException(nameof(Resource), request.ResourceId);
        }

        // Verify project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        // Check if resource is available
        if (!resource.IsAvailable)
        {
            throw new InvalidOperationException("Resource is not available for assignment");
        }

        // Assign resource to project
        resource.AssignToProject(request.ProjectId, request.StartDate, request.EndDate, request.AllocationPercentage);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource assigned to project successfully: {ResourceId} -> {ProjectId}", 
            request.ResourceId, request.ProjectId);
    }
}

/// <summary>
/// Command to change resource availability
/// </summary>
public record ChangeResourceAvailabilityCommand(
    Guid Id,
    bool IsAvailable
) : IRequest;

/// <summary>
/// Validator for ChangeResourceAvailabilityCommand
/// </summary>
public class ChangeResourceAvailabilityCommandValidator : AbstractValidator<ChangeResourceAvailabilityCommand>
{
    public ChangeResourceAvailabilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Resource ID is required");
    }
}

/// <summary>
/// Handler for ChangeResourceAvailabilityCommand
/// </summary>
public class ChangeResourceAvailabilityCommandHandler : IRequestHandler<ChangeResourceAvailabilityCommand>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeResourceAvailabilityCommandHandler> _logger;

    public ChangeResourceAvailabilityCommandHandler(
        IResourceRepository resourceRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeResourceAvailabilityCommandHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangeResourceAvailabilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing resource availability: {ResourceId} to {IsAvailable}", 
            request.Id, request.IsAvailable);

        var resource = await _resourceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (resource == null)
        {
            throw new NotFoundException(nameof(Resource), request.Id);
        }

        if (request.IsAvailable)
        {
            resource.MakeAvailable();
        }
        else
        {
            resource.MakeUnavailable();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource availability changed successfully: {ResourceId}", request.Id);
    }
}

/// <summary>
/// Command to create a new milestone
/// </summary>
public record CreateMilestoneCommand(
    string Name,
    string Description,
    Guid ProjectId,
    DateOnly DueDate,
    List<Guid>? DependentTaskIds = null
) : IRequest<Guid>;

/// <summary>
/// Validator for CreateMilestoneCommand
/// </summary>
public class CreateMilestoneCommandValidator : AbstractValidator<CreateMilestoneCommand>
{
    public CreateMilestoneCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Milestone name is required")
            .MaximumLength(200).WithMessage("Milestone name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Milestone description is required")
            .MaximumLength(1000).WithMessage("Milestone description cannot exceed 1000 characters");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Due date cannot be in the past");
    }
}

/// <summary>
/// Handler for CreateMilestoneCommand
/// </summary>
public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, Guid>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<CreateMilestoneCommandHandler> _logger;

    public CreateMilestoneCommandHandler(
        IMilestoneRepository milestoneRepository,
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<CreateMilestoneCommandHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new milestone: {MilestoneName} for project: {ProjectId}", 
            request.Name, request.ProjectId);

        // Verify project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        // Verify dependent tasks exist if specified
        if (request.DependentTaskIds?.Any() == true)
        {
            foreach (var taskId in request.DependentTaskIds)
            {
                var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
                if (task == null)
                {
                    throw new NotFoundException(nameof(ProjectTask), taskId);
                }

                if (task.ProjectId != request.ProjectId)
                {
                    throw new InvalidOperationException("All dependent tasks must belong to the same project");
                }
            }
        }

        // Create milestone entity
        var milestone = new Milestone(
            request.Name,
            request.Description,
            request.ProjectId,
            request.DueDate,
            request.DependentTaskIds);

        // Add milestone to repository
        await _milestoneRepository.AddAsync(milestone, cancellationToken);

        // Publish domain event
        milestone.AddDomainEvent(new MilestoneCreatedEvent(milestone));

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Milestone created successfully with ID: {MilestoneId}", milestone.Id);

        return milestone.Id;
    }
}

/// <summary>
/// Command to update a milestone
/// </summary>
public record UpdateMilestoneCommand(
    Guid Id,
    string Name,
    string Description,
    DateOnly DueDate,
    List<Guid>? DependentTaskIds = null
) : IRequest;

/// <summary>
/// Validator for UpdateMilestoneCommand
/// </summary>
public class UpdateMilestoneCommandValidator : AbstractValidator<UpdateMilestoneCommand>
{
    public UpdateMilestoneCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Milestone name is required")
            .MaximumLength(200).WithMessage("Milestone name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Milestone description is required")
            .MaximumLength(1000).WithMessage("Milestone description cannot exceed 1000 characters");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required");
    }
}

/// <summary>
/// Handler for UpdateMilestoneCommand
/// </summary>
public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMilestoneCommandHandler> _logger;

    public UpdateMilestoneCommandHandler(
        IMilestoneRepository milestoneRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMilestoneCommandHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating milestone: {MilestoneId}", request.Id);

        var milestone = await _milestoneRepository.GetByIdAsync(request.Id, cancellationToken);
        if (milestone == null)
        {
            throw new NotFoundException(nameof(Milestone), request.Id);
        }

        // Verify dependent tasks exist if specified
        if (request.DependentTaskIds?.Any() == true)
        {
            foreach (var taskId in request.DependentTaskIds)
            {
                var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
                if (task == null)
                {
                    throw new NotFoundException(nameof(ProjectTask), taskId);
                }

                if (task.ProjectId != milestone.ProjectId)
                {
                    throw new InvalidOperationException("All dependent tasks must belong to the same project");
                }
            }
        }

        // Update milestone properties
        milestone.UpdateDetails(
            request.Name,
            request.Description,
            request.DueDate,
            request.DependentTaskIds);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Milestone updated successfully: {MilestoneId}", request.Id);
    }
}

/// <summary>
/// Command to complete a milestone
/// </summary>
public record CompleteMilestoneCommand(
    Guid Id,
    DateOnly? CompletedDate = null
) : IRequest;

/// <summary>
/// Validator for CompleteMilestoneCommand
/// </summary>
public class CompleteMilestoneCommandValidator : AbstractValidator<CompleteMilestoneCommand>
{
    public CompleteMilestoneCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone ID is required");

        RuleFor(x => x.CompletedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.CompletedDate.HasValue)
            .WithMessage("Completed date cannot be in the future");
    }
}

/// <summary>
/// Handler for CompleteMilestoneCommand
/// </summary>
public class CompleteMilestoneCommandHandler : IRequestHandler<CompleteMilestoneCommand>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectNotificationService _notificationService;
    private readonly ILogger<CompleteMilestoneCommandHandler> _logger;

    public CompleteMilestoneCommandHandler(
        IMilestoneRepository milestoneRepository,
        IUnitOfWork unitOfWork,
        IProjectNotificationService notificationService,
        ILogger<CompleteMilestoneCommandHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(CompleteMilestoneCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing milestone: {MilestoneId}", request.Id);

        var milestone = await _milestoneRepository.GetByIdAsync(request.Id, cancellationToken);
        if (milestone == null)
        {
            throw new NotFoundException(nameof(Milestone), request.Id);
        }

        var completedDate = request.CompletedDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        milestone.Complete(completedDate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification
        await _notificationService.SendMilestoneReachedNotificationAsync(milestone, cancellationToken);

        _logger.LogInformation("Milestone completed successfully: {MilestoneId}", request.Id);
    }
}
