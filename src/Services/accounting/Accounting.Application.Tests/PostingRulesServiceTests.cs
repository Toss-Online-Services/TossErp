using Microsoft.Extensions.Logging;
using Moq;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Services;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Infrastructure.Persistence;
using TossErp.Accounting.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TossErp.Accounting.Application.Tests;

/// <summary>
/// Unit tests for PostingRulesService
/// </summary>
public class PostingRulesServiceTests
{
    private readonly Mock<ILogger<PostingRulesService>> _loggerMock;
    private readonly IAccountRepository _accountRepository;
    private readonly ICashbookRepository _cashbookRepository;
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PostingRulesService _postingRulesService;

    public PostingRulesServiceTests()
    {
        _loggerMock = new Mock<ILogger<PostingRulesService>>();
        _accountRepository = new MockAccountRepository();
        _cashbookRepository = new MockCashbookRepository();
        _entryRepository = new MockCashbookEntryRepository();
        _unitOfWork = new UnitOfWork(_accountRepository, _cashbookRepository, _entryRepository, new MockStockValuationSnapshotRepository());
        
        _postingRulesService = new PostingRulesService(
            _accountRepository,
            _cashbookRepository,
            _entryRepository,
            _unitOfWork,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleSaleCompletedAsync_ShouldCreateCorrectEntries()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var totalAmount = 1000m;
        var taxAmount = 150m;
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandleSaleCompletedAsync(saleId, totalAmount, taxAmount, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Equal(3, entries.Count); // Cash, Sales, and Tax entries

        // Verify cash entry (debit)
        var cashEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Sale && e.Type == EntryType.Debit);
        Assert.NotNull(cashEntry);
        Assert.Equal(850m, cashEntry.Amount.Amount); // Net amount (1000 - 150)

        // Verify sales entry (credit)
        var salesEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Sale && e.Type == EntryType.Credit);
        Assert.NotNull(salesEntry);
        Assert.Equal(850m, salesEntry.Amount.Amount); // Net amount

        // Verify tax entry (credit)
        var taxEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.SalesTax && e.Type == EntryType.Credit);
        Assert.NotNull(taxEntry);
        Assert.Equal(150m, taxEntry.Amount.Amount); // Tax amount
    }

    [Fact]
    public async Task HandlePurchaseReceiptAsync_ShouldCreateCorrectEntries()
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();
        var totalAmount = 500m;
        var taxAmount = 75m;
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandlePurchaseReceiptAsync(purchaseOrderId, totalAmount, taxAmount, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Equal(3, entries.Count); // Inventory, Cash, and Tax entries

        // Verify inventory entry (debit)
        var inventoryEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Purchase && e.Type == EntryType.Debit);
        Assert.NotNull(inventoryEntry);
        Assert.Equal(425m, inventoryEntry.Amount.Amount); // Net amount (500 - 75)

        // Verify cash entry (credit)
        var cashEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Purchase && e.Type == EntryType.Credit);
        Assert.NotNull(cashEntry);
        Assert.Equal(500m, cashEntry.Amount.Amount); // Total amount

        // Verify tax entry (debit)
        var taxEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.PurchaseTax && e.Type == EntryType.Debit);
        Assert.NotNull(taxEntry);
        Assert.Equal(75m, taxEntry.Amount.Amount); // Tax amount
    }

    [Fact]
    public async Task HandleInventoryAdjustmentAsync_PositiveAdjustment_ShouldCreateCorrectEntries()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var quantity = 10m;
        var unitCost = 25m;
        var adjustmentType = "Stock Take";
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandleInventoryAdjustmentAsync(itemId, quantity, unitCost, adjustmentType, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Single(entries); // Only inventory entry for positive adjustment

        // Verify inventory entry (debit)
        var inventoryEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Adjustment && e.Type == EntryType.Debit);
        Assert.NotNull(inventoryEntry);
        Assert.Equal(250m, inventoryEntry.Amount.Amount); // 10 * 25
    }

    [Fact]
    public async Task HandleInventoryAdjustmentAsync_NegativeAdjustment_ShouldCreateCorrectEntries()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var quantity = -5m; // Negative adjustment
        var unitCost = 25m;
        var adjustmentType = "Damage";
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandleInventoryAdjustmentAsync(itemId, quantity, unitCost, adjustmentType, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Equal(2, entries.Count); // Inventory credit and expense debit

        // Verify inventory entry (credit)
        var inventoryEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Adjustment && e.Type == EntryType.Credit);
        Assert.NotNull(inventoryEntry);
        Assert.Equal(125m, inventoryEntry.Amount.Amount); // 5 * 25

        // Verify expense entry (debit)
        var expenseEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.Adjustment && e.Type == EntryType.Debit);
        Assert.NotNull(expenseEntry);
        Assert.Equal(125m, expenseEntry.Amount.Amount); // 5 * 25
    }

    [Fact]
    public async Task HandleCashReceiptAsync_ShouldCreateCorrectEntries()
    {
        // Arrange
        var amount = 1000m;
        var reference = "CASH-001";
        var description = "Cash receipt from customer";
        var accountId = Guid.NewGuid();
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandleCashReceiptAsync(amount, reference, description, accountId, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Equal(2, entries.Count); // Cash debit and account credit

        // Verify cash entry (debit)
        var cashEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.CashReceipt && e.Type == EntryType.Debit);
        Assert.NotNull(cashEntry);
        Assert.Equal(1000m, cashEntry.Amount.Amount);

        // Verify account entry (credit)
        var accountEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.CashReceipt && e.Type == EntryType.Credit);
        Assert.NotNull(accountEntry);
        Assert.Equal(1000m, accountEntry.Amount.Amount);
    }

    [Fact]
    public async Task HandleCashPaymentAsync_ShouldCreateCorrectEntries()
    {
        // Arrange
        var amount = 500m;
        var reference = "PAY-001";
        var description = "Cash payment for supplies";
        var accountId = Guid.NewGuid();
        var tenantId = "tenant-id";

        // Act
        await _postingRulesService.HandleCashPaymentAsync(amount, reference, description, accountId, tenantId);

        // Assert
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId);
        Assert.NotNull(cashbook);
        
        var entries = cashbook.Entries.ToList();
        Assert.Equal(2, entries.Count); // Account debit and cash credit

        // Verify account entry (debit)
        var accountEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.CashPayment && e.Type == EntryType.Debit);
        Assert.NotNull(accountEntry);
        Assert.Equal(500m, accountEntry.Amount.Amount);

        // Verify cash entry (credit)
        var cashEntry = entries.FirstOrDefault(e => e.Category == EntryCategory.CashPayment && e.Type == EntryType.Credit);
        Assert.NotNull(cashEntry);
        Assert.Equal(500m, cashEntry.Amount.Amount);
    }
}
