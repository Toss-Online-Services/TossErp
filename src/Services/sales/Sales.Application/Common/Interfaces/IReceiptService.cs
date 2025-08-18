using TossErp.Sales.Domain.Entities;

namespace TossErp.Sales.Application.Common.Interfaces;

/// <summary>
/// Service for generating receipts
/// </summary>
public interface IReceiptService
{
    /// <summary>
    /// Generate a receipt for a completed sale
    /// </summary>
    /// <param name="sale">The sale to generate receipt for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Receipt content</returns>
    Task<string> GenerateReceiptAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a receipt number for a new sale
    /// </summary>
    /// <param name="tillId">Till ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Receipt number</returns>
    Task<string> GenerateReceiptNumberAsync(Guid tillId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get receipt template
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Receipt template</returns>
    Task<string> GetReceiptTemplateAsync(CancellationToken cancellationToken = default);
}
