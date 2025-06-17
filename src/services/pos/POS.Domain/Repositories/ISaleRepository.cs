using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Enums;

namespace POS.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStatusAsync(POS.Domain.AggregatesModel.SaleAggregate.SaleStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByPaymentMethodAsync(PaymentType paymentMethod, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetDraftSalesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync(CancellationToken cancellationToken = default);
    
    Task<decimal> GetTotalSalesByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByStaffAsync(Guid staffId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByStatusAsync(POS.Domain.AggregatesModel.SaleAggregate.SaleStatus status, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByPaymentMethodAsync(PaymentType paymentMethod, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<bool> HasRefundAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<bool> HasPartialRefundAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<bool> HasDiscountAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<bool> HasTaxAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<bool> HasShippingAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<bool> HasLoyaltyPointsAsync(Guid saleId, CancellationToken cancellationToken = default);
} 
