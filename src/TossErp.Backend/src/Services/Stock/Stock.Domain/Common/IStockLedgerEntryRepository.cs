using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Common;

public interface IStockLedgerEntryRepository : IRepository<StockLedgerEntry>
{
    Task<IEnumerable<StockLedgerEntry>> GetByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByBinAsync(string binName, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByItemAndWarehouseAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByItemAndWarehouseAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByVoucherAsync(string voucherType, string voucherNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByVoucherDetailAsync(string voucherType, string voucherNo, string voucherDetailNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByDateRangeAsync(string itemCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetIncomingEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetOutgoingEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetIncomingEntriesAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetIncomingEntriesAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetOutgoingEntriesAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetOutgoingEntriesAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetBySerialNoAsync(string serialNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByBatchNoAsync(string batchNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByProjectAsync(string project, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByCostCenterAsync(string costCenter, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByFiscalYearAsync(string fiscalYear, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetByReferenceDocumentAsync(string referenceDocumentType, string referenceDocumentNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetCancelledEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLedgerEntry>> GetNonCancelledEntriesAsync(CancellationToken cancellationToken = default);
    Task<StockLedgerEntry?> GetLatestEntryAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<StockLedgerEntry?> GetLatestEntryAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(ItemCode itemCode, WarehouseCode warehouseCode, DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(string itemCode, string warehouseCode, DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetValueAsync(ItemCode itemCode, WarehouseCode warehouseCode, DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetValueAsync(string itemCode, string warehouseCode, DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetValueAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetValueAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByVoucherAsync(string voucherType, string voucherNo, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
} 
