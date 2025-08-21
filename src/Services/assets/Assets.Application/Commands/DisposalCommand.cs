using FluentValidation;

namespace TossErp.Assets.Application.Commands;

/// <summary>
/// Command to dispose of an asset
/// </summary>
public record DisposeAssetCommand(
    Guid AssetId,
    DisposalMethod Method,
    DateOnly DisposalDate,
    string Reason,
    decimal? DisposalValue,
    string? Currency,
    string? DisposedTo,
    string? AuthorizedBy,
    string? Notes,
    List<string>? DocumentPaths
) : IRequest<AssetDisposalDto>;

/// <summary>
/// Handler for disposing assets
/// </summary>
public class DisposeAssetCommandHandler : IRequestHandler<DisposeAssetCommand, AssetDisposalDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IFileService _fileService;
    private readonly ILogger<DisposeAssetCommandHandler> _logger;

    public DisposeAssetCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IFileService fileService,
        ILogger<DisposeAssetCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<AssetDisposalDto> Handle(DisposeAssetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disposing asset {AssetId} using method {Method}", request.AssetId, request.Method);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        if (asset.Status == AssetStatus.Disposed)
        {
            throw new InvalidOperationException($"Asset {request.AssetId} is already disposed");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Create disposal value if provided
        var disposalValue = request.DisposalValue.HasValue && !string.IsNullOrEmpty(request.Currency)
            ? Money.Create(request.DisposalValue.Value, request.Currency)
            : null;

        // Process disposal documents
        var processedDocuments = new List<AssetDocument>();
        if (request.DocumentPaths?.Any() == true)
        {
            foreach (var documentPath in request.DocumentPaths)
            {
                var document = await _fileService.ProcessAssetDocumentAsync(
                    assetId: asset.Id,
                    filePath: documentPath,
                    documentType: "Disposal",
                    uploadedBy: currentUserId,
                    cancellationToken: cancellationToken);

                processedDocuments.Add(document);
            }
        }

        // Create disposal record
        var disposal = new AssetDisposal(
            assetId: asset.Id,
            method: request.Method,
            disposalDate: request.DisposalDate,
            reason: request.Reason,
            disposalValue: disposalValue,
            disposedTo: request.DisposedTo,
            authorizedBy: request.AuthorizedBy,
            notes: request.Notes,
            disposedBy: currentUserId);

        // Add disposal documents
        foreach (var document in processedDocuments)
        {
            disposal.AddDocument(document);
        }

        // Dispose the asset
        asset.Dispose(disposal, currentUserId);

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send disposal notification
        await _notificationService.SendAssetDisposalNotificationAsync(
            asset.Id, asset.Name, request.Method, request.DisposalDate, cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully disposed asset {AssetId}", request.AssetId);

        return MapToDto(disposal, asset);
    }

    private static AssetDisposalDto MapToDto(AssetDisposal disposal, Asset asset)
    {
        return new AssetDisposalDto
        {
            Id = disposal.Id,
            AssetId = disposal.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            Method = disposal.Method,
            DisposalDate = disposal.DisposalDate,
            Reason = disposal.Reason,
            DisposalValue = disposal.DisposalValue?.Amount,
            Currency = disposal.DisposalValue?.Currency.ToString(),
            DisposedTo = disposal.DisposedTo,
            AuthorizedBy = disposal.AuthorizedBy,
            Notes = disposal.Notes,
            DisposedBy = disposal.DisposedBy,
            DisposedAt = disposal.DisposedAt,
            Documents = disposal.Documents.Select(d => new AssetDocumentDto
            {
                Id = d.Id,
                AssetId = d.AssetId,
                FileName = d.FileName,
                FilePath = d.FilePath,
                FileSize = d.FileSize,
                ContentType = d.ContentType,
                DocumentType = d.DocumentType,
                Description = d.Description,
                UploadedBy = d.UploadedBy,
                UploadedAt = d.UploadedAt
            }).ToList()
        };
    }
}

/// <summary>
/// Validator for DisposeAssetCommand
/// </summary>
public class DisposeAssetCommandValidator : AbstractValidator<DisposeAssetCommand>
{
    public DisposeAssetCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Valid disposal method is required");

        RuleFor(x => x.DisposalDate)
            .NotEmpty()
            .WithMessage("Disposal date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Disposal date cannot be in the future");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Disposal reason is required")
            .MaximumLength(1000)
            .WithMessage("Disposal reason cannot exceed 1000 characters");

        RuleFor(x => x.DisposalValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DisposalValue.HasValue)
            .WithMessage("Disposal value cannot be negative");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .When(x => x.DisposalValue.HasValue)
            .WithMessage("Currency is required when disposal value is provided");

        RuleFor(x => x.AuthorizedBy)
            .NotEmpty()
            .WithMessage("Authorization is required for asset disposal")
            .MaximumLength(200)
            .WithMessage("Authorized by cannot exceed 200 characters");
    }
}

/// <summary>
/// Command to record asset valuation
/// </summary>
public record RecordAssetValuationCommand(
    Guid AssetId,
    decimal CurrentValue,
    string Currency,
    DateOnly ValuationDate,
    string ValuationMethod,
    string? ValuedBy,
    string? Notes,
    List<string>? DocumentPaths
) : IRequest<AssetValuationDto>;

/// <summary>
/// Handler for recording asset valuations
/// </summary>
public class RecordAssetValuationCommandHandler : IRequestHandler<RecordAssetValuationCommand, AssetValuationDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly ILogger<RecordAssetValuationCommandHandler> _logger;

    public RecordAssetValuationCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        IFileService fileService,
        ILogger<RecordAssetValuationCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<AssetValuationDto> Handle(RecordAssetValuationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording valuation for asset {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Create valuation amount
        var valuationAmount = Money.Create(request.CurrentValue, request.Currency);

        // Process valuation documents
        var processedDocuments = new List<AssetDocument>();
        if (request.DocumentPaths?.Any() == true)
        {
            foreach (var documentPath in request.DocumentPaths)
            {
                var document = await _fileService.ProcessAssetDocumentAsync(
                    assetId: asset.Id,
                    filePath: documentPath,
                    documentType: "Valuation",
                    uploadedBy: currentUserId,
                    cancellationToken: cancellationToken);

                processedDocuments.Add(document);
            }
        }

        // Create valuation record
        var valuation = new AssetValuation(
            assetId: asset.Id,
            currentValue: valuationAmount,
            valuationDate: request.ValuationDate,
            valuationMethod: request.ValuationMethod,
            valuedBy: request.ValuedBy,
            notes: request.Notes,
            recordedBy: currentUserId);

        // Add valuation documents
        foreach (var document in processedDocuments)
        {
            valuation.AddDocument(document);
        }

        // Record the valuation
        asset.RecordValuation(valuation, currentUserId);

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully recorded valuation for asset {AssetId}", request.AssetId);

        return MapToDto(valuation, asset);
    }

    private static AssetValuationDto MapToDto(AssetValuation valuation, Asset asset)
    {
        return new AssetValuationDto
        {
            Id = valuation.Id,
            AssetId = valuation.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            CurrentValue = valuation.CurrentValue.Amount,
            Currency = valuation.CurrentValue.Currency.ToString(),
            ValuationDate = valuation.ValuationDate,
            ValuationMethod = valuation.ValuationMethod,
            ValuedBy = valuation.ValuedBy,
            Notes = valuation.Notes,
            RecordedBy = valuation.RecordedBy,
            RecordedAt = valuation.RecordedAt,
            Documents = valuation.Documents.Select(d => new AssetDocumentDto
            {
                Id = d.Id,
                AssetId = d.AssetId,
                FileName = d.FileName,
                FilePath = d.FilePath,
                FileSize = d.FileSize,
                ContentType = d.ContentType,
                DocumentType = d.DocumentType,
                Description = d.Description,
                UploadedBy = d.UploadedBy,
                UploadedAt = d.UploadedAt
            }).ToList()
        };
    }
}

/// <summary>
/// Validator for RecordAssetValuationCommand
/// </summary>
public class RecordAssetValuationCommandValidator : AbstractValidator<RecordAssetValuationCommand>
{
    public RecordAssetValuationCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.CurrentValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current value cannot be negative");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.ValuationDate)
            .NotEmpty()
            .WithMessage("Valuation date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Valuation date cannot be in the future");

        RuleFor(x => x.ValuationMethod)
            .NotEmpty()
            .WithMessage("Valuation method is required")
            .MaximumLength(200)
            .WithMessage("Valuation method cannot exceed 200 characters");
    }
}
