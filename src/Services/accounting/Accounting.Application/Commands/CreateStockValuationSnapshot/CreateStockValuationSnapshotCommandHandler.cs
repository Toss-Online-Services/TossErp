using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.Commands.CreateStockValuationSnapshot;

/// <summary>
/// Handler for CreateStockValuationSnapshotCommand
/// </summary>
public class CreateStockValuationSnapshotCommandHandler : IRequestHandler<CreateStockValuationSnapshotCommand, Guid>
{
    private readonly IStockValuationService _stockValuationService;
    private readonly ILogger<CreateStockValuationSnapshotCommandHandler> _logger;

    public CreateStockValuationSnapshotCommandHandler(
        IStockValuationService stockValuationService,
        ILogger<CreateStockValuationSnapshotCommandHandler> logger)
    {
        _stockValuationService = stockValuationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateStockValuationSnapshotCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating stock valuation snapshot for {Date} using {Method}", 
            request.SnapshotDate, request.Method);

        // For MVP, we'll use a hardcoded tenant ID
        // In a real implementation, this would come from the current user context
        var tenantId = "tenant-001";

        var snapshotId = await _stockValuationService.CreateStockValuationSnapshotAsync(
            request.SnapshotDate, 
            request.Method, 
            tenantId, 
            cancellationToken);

        _logger.LogInformation("Created stock valuation snapshot with ID {SnapshotId}", snapshotId);

        return snapshotId;
    }
}

