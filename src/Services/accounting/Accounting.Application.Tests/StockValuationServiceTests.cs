using Microsoft.Extensions.Logging;
using Moq;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Services;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Infrastructure.Persistence;
using TossErp.Accounting.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TossErp.Accounting.Application.Tests;

/// <summary>
/// Unit tests for StockValuationService
/// </summary>
public class StockValuationServiceTests
{
    private readonly Mock<ILogger<StockValuationService>> _loggerMock;
    private readonly IStockValuationSnapshotRepository _snapshotRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StockValuationService _stockValuationService;

    public StockValuationServiceTests()
    {
        _loggerMock = new Mock<ILogger<StockValuationService>>();
        _snapshotRepository = new MockStockValuationSnapshotRepository();
        _accountRepository = new MockAccountRepository();
        _unitOfWork = new UnitOfWork(_accountRepository, new MockCashbookRepository(), new MockCashbookEntryRepository(), _snapshotRepository);
        
        _stockValuationService = new StockValuationService(
            _snapshotRepository,
            _accountRepository,
            _unitOfWork,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CalculateStockValuationAsync_ShouldReturnCorrectTotalValue()
    {
        // Arrange
        var valuationDate = DateTime.Today;
        var method = ValuationMethod.WeightedAverage;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CalculateStockValuationAsync(valuationDate, method, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ZAR", result.Currency);
        Assert.True(result.Amount > 0);
        
        // Expected total: (10 * 5000) + (50 * 200) + (25 * 300) + (15 * 1500) + (30 * 150) = 50,000 + 10,000 + 7,500 + 22,500 + 4,500 = 94,500
        Assert.Equal(94500m, result.Amount);
    }

    [Fact]
    public async Task CalculateWarehouseValuationAsync_ShouldReturnCorrectWarehouseValue()
    {
        // Arrange
        var warehouseCode = "WH001";
        var valuationDate = DateTime.Today;
        var method = ValuationMethod.WeightedAverage;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CalculateWarehouseValuationAsync(warehouseCode, valuationDate, method, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ZAR", result.Currency);
        Assert.True(result.Amount > 0);
        
        // Expected WH001 total: (10 * 5000) + (50 * 200) + (25 * 300) = 50,000 + 10,000 + 7,500 = 67,500
        Assert.Equal(67500m, result.Amount);
    }

    [Fact]
    public async Task CalculateItemValuationAsync_ShouldReturnCorrectItemValue()
    {
        // Arrange
        var itemCode = "ITEM001";
        var valuationDate = DateTime.Today;
        var method = ValuationMethod.WeightedAverage;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CalculateItemValuationAsync(itemCode, valuationDate, method, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ZAR", result.Currency);
        Assert.True(result.Amount > 0);
        
        // Expected ITEM001 total: 10 * 5000 = 50,000
        Assert.Equal(50000m, result.Amount);
    }

    [Fact]
    public async Task CreateStockValuationSnapshotAsync_ShouldCreateSnapshots()
    {
        // Arrange
        var snapshotDate = DateTime.Today.AddDays(1);
        var method = ValuationMethod.FIFO;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CreateStockValuationSnapshotAsync(snapshotDate, method, tenantId);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        
        // Verify snapshots were created by checking the repository
        var snapshots = await _snapshotRepository.GetByDateAsync(snapshotDate, tenantId);
        Assert.NotEmpty(snapshots);
        Assert.All(snapshots, s => Assert.Equal(method, s.Method));
    }

    [Fact]
    public async Task GetTotalStockValueForPLAsync_ShouldReturnCorrectTotal()
    {
        // Arrange
        var asOfDate = DateTime.Today;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.GetTotalStockValueForPLAsync(asOfDate, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ZAR", result.Currency);
        Assert.True(result.Amount > 0);
        
        // Expected total: 94,500 (same as CalculateStockValuationAsync)
        Assert.Equal(94500m, result.Amount);
    }

    [Fact]
    public async Task GetStockValuationSummaryAsync_ShouldReturnCompleteSummary()
    {
        // Arrange
        var asOfDate = DateTime.Today;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.GetStockValuationSummaryAsync(asOfDate, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(asOfDate.Date, result.AsOfDate.Date);
        Assert.Equal(94500m, result.TotalValue.Amount);
        Assert.Equal("ZAR", result.TotalValue.Currency);
        Assert.Equal(5, result.ItemCount); // 5 different items
        Assert.Equal(2, result.WarehouseCount); // 2 warehouses
        Assert.Equal(ValuationMethod.WeightedAverage, result.Method);
        
        // Check warehouse values
        Assert.Equal(2, result.WarehouseValues.Count);
        Assert.True(result.WarehouseValues.ContainsKey("WH001"));
        Assert.True(result.WarehouseValues.ContainsKey("WH002"));
        Assert.Equal(67500m, result.WarehouseValues["WH001"].Amount);
        Assert.Equal(27000m, result.WarehouseValues["WH002"].Amount);
        
        // Check category values
        Assert.True(result.CategoryValues.Count > 0);
    }

    [Fact]
    public async Task CalculateStockValuationAsync_WithNonExistentDate_ShouldReturnZero()
    {
        // Arrange
        var valuationDate = DateTime.Today.AddDays(-100); // Date with no snapshots
        var method = ValuationMethod.WeightedAverage;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CalculateStockValuationAsync(valuationDate, method, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Amount);
    }

    [Fact]
    public async Task CalculateItemValuationAsync_WithNonExistentItem_ShouldReturnZero()
    {
        // Arrange
        var itemCode = "NONEXISTENT";
        var valuationDate = DateTime.Today;
        var method = ValuationMethod.WeightedAverage;
        var tenantId = "tenant-001";

        // Act
        var result = await _stockValuationService.CalculateItemValuationAsync(itemCode, valuationDate, method, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Amount);
    }
}

