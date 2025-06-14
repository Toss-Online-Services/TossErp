#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<IEnumerable<Sale>> GetByStoreIdAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(int staffId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync();
    Task<IEnumerable<Sale>> GetByCustomerAsync(string customerId);
    Task DeleteAsync(int saleId);
} 
