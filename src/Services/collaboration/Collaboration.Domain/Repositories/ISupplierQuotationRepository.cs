using Collaboration.Domain.Entities;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Repositories;

/// <summary>
/// Repository interface for SupplierQuotation entities
/// </summary>
public interface ISupplierQuotationRepository
{
    Task<SupplierQuotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetByStatusAsync(QuotationStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetActiveQuotationsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetExpiredQuotationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetAcceptedQuotationsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierQuotation>> GetBestPriceQuotationsAsync(Guid campaignId, int quantity, CancellationToken cancellationToken = default);
    Task<SupplierQuotation> AddAsync(SupplierQuotation quotation, CancellationToken cancellationToken = default);
    Task<SupplierQuotation> UpdateAsync(SupplierQuotation quotation, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasSupplierQuotationAsync(Guid campaignId, Guid supplierId, CancellationToken cancellationToken = default);
    Task<int> GetQuotationCountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetLowestUnitPriceAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetHighestDiscountAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
