using System;
using System.Collections.Generic;
using System.Linq;
using Toss.Application.Sales.Commands.CreateDeliveryNote;
using Toss.Application.Sales.Commands.CreateSale;
using Toss.Application.Sales.Commands.CreateSalesDocument;
using Toss.Application.Sales.Queries.GetSalesDocuments;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Sales;

using static Testing;

public class NonPosWorkflowTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCompleteQuoteOrderDeliveryInvoiceFlow()
    {
        var userId = await RunAsDefaultUserAsync();
        var business = await CreateBusinessAsync();
        await AddBusinessMembershipAsync(userId, business);

        var shop = new Store
        {
            Name = "Workshop Shop",
            OwnerId = userId,
            Email = "shop@example.com",
            BusinessId = business.Id
        };
        await AddAsync(shop);

        var product = new Product
        {
            Name = "Bulk Sugar",
            SKU = $"SUGAR-{Guid.NewGuid():N}".Substring(0, 16),
            BasePrice = 25m
        };
        await AddAsync(product);

        var stockLevel = new StockLevel
        {
            ShopId = shop.Id,
            ProductId = product.Id,
            CurrentStock = 60,
            ReorderPoint = 5,
            ReorderQuantity = 20
        };
        await AddAsync(stockLevel);

        const int orderedQuantity = 4;
        const decimal unitPrice = 50m;

        var saleId = await SendAsync(new CreateSaleCommand
        {
            ShopId = shop.Id,
            SaleType = SaleType.PreOrder,
            Items = new List<SaleItemDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = orderedQuantity,
                    UnitPrice = unitPrice
                }
            }
        });

        var quoteId = await SendAsync(new CreateSalesDocumentCommand
        {
            SaleId = saleId,
            DocumentType = SalesDocumentType.Quote,
            Notes = "Initial quote for preorder"
        });

        var orderId = await SendAsync(new CreateSalesDocumentCommand
        {
            SaleId = saleId,
            DocumentType = SalesDocumentType.SalesOrder
        });

        var deliveryNoteId = await SendAsync(new CreateDeliveryNoteCommand
        {
            SaleId = saleId,
            ShopId = shop.Id,
            Lines = new List<DeliveryNoteLineDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = orderedQuantity
                }
            }
        });

        var stockAfterDelivery = await FindAsync<StockLevel>(stockLevel.Id);
        stockAfterDelivery.ShouldNotBeNull();
        stockAfterDelivery!.CurrentStock.ShouldBe(60 - orderedQuantity);

        var invoiceId = await SendAsync(new CreateSalesDocumentCommand
        {
            SaleId = saleId,
            DocumentType = SalesDocumentType.Invoice
        });

        var stockAfterInvoice = await FindAsync<StockLevel>(stockLevel.Id);
        stockAfterInvoice.ShouldNotBeNull();
        stockAfterInvoice!.CurrentStock.ShouldBe(stockAfterDelivery.CurrentStock);

        (await CountAsync<StockMovement>()).ShouldBe(1);

        var quotes = await SendAsync(new GetSalesDocumentsQuery { ShopId = shop.Id, Type = SalesDocumentType.Quote });
        quotes.TotalCount.ShouldBe(1);
        quotes.Items.Single().Id.ShouldBe(quoteId);

        var orders = await SendAsync(new GetSalesDocumentsQuery { ShopId = shop.Id, Type = SalesDocumentType.SalesOrder });
        orders.TotalCount.ShouldBe(1);
        orders.Items.Single().Id.ShouldBe(orderId);

        var deliveries = await SendAsync(new GetSalesDocumentsQuery { ShopId = shop.Id, Type = SalesDocumentType.DeliveryNote });
        deliveries.TotalCount.ShouldBe(1);
        deliveries.Items.Single().Id.ShouldBe(deliveryNoteId);

        var invoices = await SendAsync(new GetSalesDocumentsQuery { ShopId = shop.Id, Type = SalesDocumentType.Invoice });
        invoices.TotalCount.ShouldBe(1);
        invoices.Items.Single().Id.ShouldBe(invoiceId);
    }
}

