using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Queries.GetStockEntriesByType;

/// <summary>
/// Handler for retrieving stock entries by type
/// </summary>
public class GetStockEntriesByTypeQueryHandler : IRequestHandler<GetStockEntriesByTypeQuery, List<StockEntryDto>>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly ILogger<GetStockEntriesByTypeQueryHandler> _logger;

    public GetStockEntriesByTypeQueryHandler(
        IStockEntryRepository stockEntryRepository,
        ILogger<GetStockEntriesByTypeQueryHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _logger = logger;
    }

    public async Task<List<StockEntryDto>> Handle(GetStockEntriesByTypeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting stock entries by type: {EntryType}", request.EntryType);

        // Try to parse the entry type as a StockEntryPurpose enum
        if (!Enum.TryParse<TossErp.Stock.Domain.Enums.StockEntryPurpose>(request.EntryType, true, out var entryPurpose))
        {
            _logger.LogWarning("Invalid entry type {EntryType}", request.EntryType);
            return new List<StockEntryDto>();
        }

        // Get stock entries by purpose
        var stockEntries = await _stockEntryRepository.GetByPurposeAsync(entryPurpose, cancellationToken);

        // Apply date filtering if specified
        if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            stockEntries = stockEntries.Where(se => 
                (!request.FromDate.HasValue || se.EntryDate >= request.FromDate.Value) &&
                (!request.ToDate.HasValue || se.EntryDate <= request.ToDate.Value)
            );
        }

        // Apply search filter if specified
        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            var searchLower = request.SearchString.ToLower();
            stockEntries = stockEntries.Where(se => 
                se.EntryNumber.Value.ToLower().Contains(searchLower) ||
                (se.Reference != null && se.Reference.ToLower().Contains(searchLower)) ||
                (se.Notes != null && se.Notes.ToLower().Contains(searchLower))
            );
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "entrydate":
                    stockEntries = request.IsDescending 
                        ? stockEntries.OrderByDescending(se => se.EntryDate)
                        : stockEntries.OrderBy(se => se.EntryDate);
                    break;
                case "entrynumber":
                    stockEntries = request.IsDescending 
                        ? stockEntries.OrderByDescending(se => se.EntryNumber.Value)
                        : stockEntries.OrderBy(se => se.EntryNumber.Value);
                    break;
                case "company":
                    stockEntries = request.IsDescending 
                        ? stockEntries.OrderByDescending(se => se.Company)
                        : stockEntries.OrderBy(se => se.Company);
                    break;
                default:
                    stockEntries = stockEntries.OrderByDescending(se => se.EntryDate);
                    break;
            }
        }
        else
        {
            stockEntries = stockEntries.OrderByDescending(se => se.EntryDate);
        }

        // Apply pagination
        var pagedEntries = stockEntries
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        // Map to DTOs
        var result = pagedEntries.Select(se => new StockEntryDto
        {
            Id = se.Id,
            StockEntryNo = se.EntryNumber.Value,
            StockEntryType = entryPurpose.ToString(),
            Purpose = entryPurpose.ToString(),
            PostingDate = se.EntryDate,
            PostingTime = se.EntryDate, // Use same date for time
            Company = se.Company,
            ReferenceNo = se.Reference ?? string.Empty,
            Status = se.IsPosted ? "Posted" : "Draft",
            Remarks = se.Notes ?? string.Empty,
            TotalAmount = se.GetTotalValue(),
            TotalQty = se.GetTotalQuantity(),
            Created = DateTime.UtcNow, // These would ideally come from audit properties
            CreatedBy = "System"
        }).ToList();

        _logger.LogInformation("Retrieved {Count} stock entries for type {EntryType}", result.Count, request.EntryType);
        
        return result;
    }
}
