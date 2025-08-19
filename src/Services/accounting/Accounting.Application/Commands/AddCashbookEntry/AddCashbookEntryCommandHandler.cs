using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.AddCashbookEntry;

/// <summary>
/// Handler for AddCashbookEntryCommand
/// </summary>
public class AddCashbookEntryCommandHandler : IRequestHandler<AddCashbookEntryCommand, CashbookEntryDto>
{
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly ICashbookRepository _cashbookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCashbookEntryCommandHandler> _logger;

    public AddCashbookEntryCommandHandler(
        ICashbookEntryRepository entryRepository,
        ICashbookRepository cashbookRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCashbookEntryCommandHandler> logger)
    {
        _entryRepository = entryRepository;
        _cashbookRepository = cashbookRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CashbookEntryDto> Handle(AddCashbookEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding cashbook entry: Amount={Amount}, Type={Type}, Category={Category}", 
            request.Amount, request.Type, request.Category);

        // For MVP, we'll use a hardcoded tenant ID
        // In a real implementation, this would come from the current user context
        var tenantId = "tenant-001";

        // Validate cashbook exists
        var cashbook = await _cashbookRepository.GetByIdAsync(request.CashbookId, cancellationToken);
        if (cashbook == null)
        {
            throw new ArgumentException($"Cashbook with ID {request.CashbookId} not found");
        }

        // Parse enums
        if (!Enum.TryParse<EntryType>(request.Type, out var entryType))
        {
            throw new ArgumentException($"Invalid entry type: {request.Type}");
        }

        if (!Enum.TryParse<EntryCategory>(request.Category, out var entryCategory))
        {
            throw new ArgumentException($"Invalid entry category: {request.Category}");
        }

                       // Create the entry using the existing CreateCashbookEntry command logic
               // For MVP, we'll create a simple entry
               var entry = Domain.Entities.CashbookEntry.Create(
                   request.TransactionDate,
                   request.Reference ?? string.Empty,
                   request.Description ?? string.Empty,
                   new Domain.Common.Money(request.Amount, "ZAR"),
                   entryType,
                   entryCategory,
                   Guid.Empty, // No account for MVP
                   tenantId,
                   request.RelatedEntityId,
                   request.RelatedEntityType
               );

        // Add to repository
        await _entryRepository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

                       // Convert to DTO
               var dto = new CashbookEntryDto
               {
                   Id = entry.Id,
                   CashbookId = request.CashbookId, // Use the requested cashbook ID
                   Amount = entry.Amount.Amount,
                   Type = entry.Type.ToString(),
                   Category = entry.Category.ToString(),
                   Description = entry.Description,
                   Reference = entry.Reference,
                   TransactionDate = entry.TransactionDate,
                   CreatedAt = entry.CreatedAt,
                   CreatedBy = entry.CreatedBy,
                   UpdatedAt = entry.UpdatedAt,
                   UpdatedBy = entry.UpdatedBy
               };

        _logger.LogInformation("Added cashbook entry with ID {EntryId}", entry.Id);

        return dto;
    }
}
