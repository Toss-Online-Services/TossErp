using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TossErp.Accounting.Application.Services;

/// <summary>
/// Implementation of posting rules service for handling automatic accounting entries
/// </summary>
public class PostingRulesService : IPostingRulesService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICashbookRepository _cashbookRepository;
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PostingRulesService> _logger;

    public PostingRulesService(
        IAccountRepository accountRepository,
        ICashbookRepository cashbookRepository,
        ICashbookEntryRepository entryRepository,
        IUnitOfWork unitOfWork,
        ILogger<PostingRulesService> logger)
    {
        _accountRepository = accountRepository;
        _cashbookRepository = cashbookRepository;
        _entryRepository = entryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleSaleCompletedAsync(Guid saleId, decimal totalAmount, decimal taxAmount, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling sale completed event for sale {SaleId} with amount {Amount}", saleId, totalAmount);

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync(tenantId, cancellationToken);

            // Get required accounts
            var cashAccount = await GetAccountByCodeAsync("1000", tenantId, cancellationToken); // Cash account
            var salesAccount = await GetAccountByCodeAsync("4000", tenantId, cancellationToken); // Sales account
            var taxAccount = await GetAccountByCodeAsync("4002", tenantId, cancellationToken); // Sales tax account

            if (cashAccount == null || salesAccount == null || taxAccount == null)
            {
                _logger.LogError("Required accounts not found for sale posting");
                throw new InvalidOperationException("Required accounts not found for sale posting");
            }

            var netAmount = totalAmount - taxAmount;
            var transactionDate = DateTime.UtcNow;

            // Create entries
            var entries = new List<CashbookEntry>();

            // Debit Cash (increase)
            if (netAmount > 0)
            {
                var cashEntry = CashbookEntry.Create(
                    transactionDate,
                    $"SALE-{saleId}",
                    $"Cash receipt for sale {saleId}",
                    new Money(netAmount),
                    EntryType.Debit,
                    EntryCategory.Sale,
                    cashAccount.Id,
                    tenantId,
                    saleId.ToString(),
                    "Sale"
                );
                entries.Add(cashEntry);
            }

            // Credit Sales (increase)
            if (netAmount > 0)
            {
                var salesEntry = CashbookEntry.Create(
                    transactionDate,
                    $"SALE-{saleId}",
                    $"Sales revenue for sale {saleId}",
                    new Money(netAmount),
                    EntryType.Credit,
                    EntryCategory.Sale,
                    salesAccount.Id,
                    tenantId,
                    saleId.ToString(),
                    "Sale"
                );
                entries.Add(salesEntry);
            }

            // Credit Sales Tax (if applicable)
            if (taxAmount > 0)
            {
                var taxEntry = CashbookEntry.Create(
                    transactionDate,
                    $"SALE-{saleId}",
                    $"Sales tax for sale {saleId}",
                    new Money(taxAmount),
                    EntryType.Credit,
                    EntryCategory.SalesTax,
                    taxAccount.Id,
                    tenantId,
                    saleId.ToString(),
                    "Sale"
                );
                entries.Add(taxEntry);
            }

            // Add entries to cashbook and save
            foreach (var entry in entries)
            {
                cashbook.CreateAndAddEntry(
                    entry.TransactionDate,
                    entry.Reference,
                    entry.Description,
                    entry.Amount,
                    entry.Type,
                    entry.Category,
                    entry.AccountId,
                    entry.RelatedEntityId,
                    entry.RelatedEntityType
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} entries for sale {SaleId}", entries.Count, saleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling sale completed event for sale {SaleId}", saleId);
            throw;
        }
    }

    public async Task HandlePurchaseReceiptAsync(Guid purchaseOrderId, decimal totalAmount, decimal taxAmount, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling purchase receipt event for PO {PurchaseOrderId} with amount {Amount}", purchaseOrderId, totalAmount);

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync(tenantId, cancellationToken);

            // Get required accounts
            var cashAccount = await GetAccountByCodeAsync("1000", tenantId, cancellationToken); // Cash account
            var inventoryAccount = await GetAccountByCodeAsync("1003", tenantId, cancellationToken); // Inventory account
            var taxAccount = await GetAccountByCodeAsync("2002", tenantId, cancellationToken); // Purchase tax account

            if (cashAccount == null || inventoryAccount == null || taxAccount == null)
            {
                _logger.LogError("Required accounts not found for purchase posting");
                throw new InvalidOperationException("Required accounts not found for purchase posting");
            }

            var netAmount = totalAmount - taxAmount;
            var transactionDate = DateTime.UtcNow;

            // Create entries
            var entries = new List<CashbookEntry>();

            // Debit Inventory (increase)
            if (netAmount > 0)
            {
                var inventoryEntry = CashbookEntry.Create(
                    transactionDate,
                    $"PO-{purchaseOrderId}",
                    $"Inventory purchase for PO {purchaseOrderId}",
                    new Money(netAmount),
                    EntryType.Debit,
                    EntryCategory.Purchase,
                    inventoryAccount.Id,
                    tenantId,
                    purchaseOrderId.ToString(),
                    "PurchaseOrder"
                );
                entries.Add(inventoryEntry);
            }

            // Credit Cash (decrease)
            if (totalAmount > 0)
            {
                var cashEntry = CashbookEntry.Create(
                    transactionDate,
                    $"PO-{purchaseOrderId}",
                    $"Cash payment for PO {purchaseOrderId}",
                    new Money(totalAmount),
                    EntryType.Credit,
                    EntryCategory.Purchase,
                    cashAccount.Id,
                    tenantId,
                    purchaseOrderId.ToString(),
                    "PurchaseOrder"
                );
                entries.Add(cashEntry);
            }

            // Debit Purchase Tax (if applicable)
            if (taxAmount > 0)
            {
                var taxEntry = CashbookEntry.Create(
                    transactionDate,
                    $"PO-{purchaseOrderId}",
                    $"Purchase tax for PO {purchaseOrderId}",
                    new Money(taxAmount),
                    EntryType.Debit,
                    EntryCategory.PurchaseTax,
                    taxAccount.Id,
                    tenantId,
                    purchaseOrderId.ToString(),
                    "PurchaseOrder"
                );
                entries.Add(taxEntry);
            }

            // Add entries to cashbook and save
            foreach (var entry in entries)
            {
                cashbook.CreateAndAddEntry(
                    entry.TransactionDate,
                    entry.Reference,
                    entry.Description,
                    entry.Amount,
                    entry.Type,
                    entry.Category,
                    entry.AccountId,
                    entry.RelatedEntityId,
                    entry.RelatedEntityType
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} entries for purchase order {PurchaseOrderId}", entries.Count, purchaseOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling purchase receipt event for PO {PurchaseOrderId}", purchaseOrderId);
            throw;
        }
    }

    public async Task HandleInventoryAdjustmentAsync(Guid itemId, decimal quantity, decimal unitCost, string adjustmentType, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling inventory adjustment for item {ItemId} with quantity {Quantity}", itemId, quantity);

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync(tenantId, cancellationToken);

            // Get required accounts
            var inventoryAccount = await GetAccountByCodeAsync("1003", tenantId, cancellationToken); // Inventory account
            var adjustmentAccount = await GetAccountByCodeAsync("5008", tenantId, cancellationToken); // Other expenses account

            if (inventoryAccount == null || adjustmentAccount == null)
            {
                _logger.LogError("Required accounts not found for inventory adjustment posting");
                throw new InvalidOperationException("Required accounts not found for inventory adjustment posting");
            }

            var totalAmount = Math.Abs(quantity * unitCost);
            var transactionDate = DateTime.UtcNow;

            // Create entries based on adjustment type
            var entries = new List<CashbookEntry>();

            if (quantity > 0) // Positive adjustment (increase inventory)
            {
                // Debit Inventory (increase)
                var inventoryEntry = CashbookEntry.Create(
                    transactionDate,
                    $"ADJ-{itemId}",
                    $"Inventory adjustment for item {itemId} - {adjustmentType}",
                    new Money(totalAmount),
                    EntryType.Debit,
                    EntryCategory.Adjustment,
                    inventoryAccount.Id,
                    tenantId,
                    itemId.ToString(),
                    "InventoryItem"
                );
                entries.Add(inventoryEntry);
            }
            else // Negative adjustment (decrease inventory)
            {
                // Credit Inventory (decrease)
                var inventoryEntry = CashbookEntry.Create(
                    transactionDate,
                    $"ADJ-{itemId}",
                    $"Inventory adjustment for item {itemId} - {adjustmentType}",
                    new Money(totalAmount),
                    EntryType.Credit,
                    EntryCategory.Adjustment,
                    inventoryAccount.Id,
                    tenantId,
                    itemId.ToString(),
                    "InventoryItem"
                );
                entries.Add(inventoryEntry);

                // Debit Adjustment Account (expense)
                var adjustmentEntry = CashbookEntry.Create(
                    transactionDate,
                    $"ADJ-{itemId}",
                    $"Inventory adjustment expense for item {itemId} - {adjustmentType}",
                    new Money(totalAmount),
                    EntryType.Debit,
                    EntryCategory.Adjustment,
                    adjustmentAccount.Id,
                    tenantId,
                    itemId.ToString(),
                    "InventoryItem"
                );
                entries.Add(adjustmentEntry);
            }

            // Add entries to cashbook and save
            foreach (var entry in entries)
            {
                cashbook.CreateAndAddEntry(
                    entry.TransactionDate,
                    entry.Reference,
                    entry.Description,
                    entry.Amount,
                    entry.Type,
                    entry.Category,
                    entry.AccountId,
                    entry.RelatedEntityId,
                    entry.RelatedEntityType
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} entries for inventory adjustment {ItemId}", entries.Count, itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling inventory adjustment for item {ItemId}", itemId);
            throw;
        }
    }

    public async Task HandleCashReceiptAsync(decimal amount, string reference, string description, Guid accountId, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling cash receipt for reference {Reference} with amount {Amount}", reference, amount);

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync(tenantId, cancellationToken);

            // Get cash account
            var cashAccount = await GetAccountByCodeAsync("1000", tenantId, cancellationToken); // Cash account
            if (cashAccount == null)
            {
                _logger.LogError("Cash account not found for cash receipt posting");
                throw new InvalidOperationException("Cash account not found for cash receipt posting");
            }

            var transactionDate = DateTime.UtcNow;

            // Create entries
            var entries = new List<CashbookEntry>();

            // Debit Cash (increase)
            var cashEntry = CashbookEntry.Create(
                transactionDate,
                reference,
                description,
                new Money(amount),
                EntryType.Debit,
                EntryCategory.CashReceipt,
                cashAccount.Id,
                tenantId
            );
            entries.Add(cashEntry);

            // Credit the specified account
            var accountEntry = CashbookEntry.Create(
                transactionDate,
                reference,
                description,
                new Money(amount),
                EntryType.Credit,
                EntryCategory.CashReceipt,
                accountId,
                tenantId
            );
            entries.Add(accountEntry);

            // Add entries to cashbook and save
            foreach (var entry in entries)
            {
                cashbook.CreateAndAddEntry(
                    entry.TransactionDate,
                    entry.Reference,
                    entry.Description,
                    entry.Amount,
                    entry.Type,
                    entry.Category,
                    entry.AccountId,
                    entry.RelatedEntityId,
                    entry.RelatedEntityType
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} entries for cash receipt {Reference}", entries.Count, reference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling cash receipt for reference {Reference}", reference);
            throw;
        }
    }

    public async Task HandleCashPaymentAsync(decimal amount, string reference, string description, Guid accountId, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling cash payment for reference {Reference} with amount {Amount}", reference, amount);

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync(tenantId, cancellationToken);

            // Get cash account
            var cashAccount = await GetAccountByCodeAsync("1000", tenantId, cancellationToken); // Cash account
            if (cashAccount == null)
            {
                _logger.LogError("Cash account not found for cash payment posting");
                throw new InvalidOperationException("Cash account not found for cash payment posting");
            }

            var transactionDate = DateTime.UtcNow;

            // Create entries
            var entries = new List<CashbookEntry>();

            // Debit the specified account (expense/asset decrease)
            var accountEntry = CashbookEntry.Create(
                transactionDate,
                reference,
                description,
                new Money(amount),
                EntryType.Debit,
                EntryCategory.CashPayment,
                accountId,
                tenantId
            );
            entries.Add(accountEntry);

            // Credit Cash (decrease)
            var cashEntry = CashbookEntry.Create(
                transactionDate,
                reference,
                description,
                new Money(amount),
                EntryType.Credit,
                EntryCategory.CashPayment,
                cashAccount.Id,
                tenantId
            );
            entries.Add(cashEntry);

            // Add entries to cashbook and save
            foreach (var entry in entries)
            {
                cashbook.CreateAndAddEntry(
                    entry.TransactionDate,
                    entry.Reference,
                    entry.Description,
                    entry.Amount,
                    entry.Type,
                    entry.Category,
                    entry.AccountId,
                    entry.RelatedEntityId,
                    entry.RelatedEntityType
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created {Count} entries for cash payment {Reference}", entries.Count, reference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling cash payment for reference {Reference}", reference);
            throw;
        }
    }

    private async Task<Cashbook> GetOrCreateDefaultCashbookAsync(string tenantId, CancellationToken cancellationToken)
    {
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId, cancellationToken);
        
        if (cashbook == null)
        {
            cashbook = Cashbook.Create("Main Cashbook", tenantId, Money.Zero(), "Default cashbook for the tenant");
            cashbook = await _cashbookRepository.AddAsync(cashbook, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return cashbook;
    }

    private async Task<Account?> GetAccountByCodeAsync(string code, string tenantId, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetByCodeAsync(code, tenantId, cancellationToken);
    }
}


