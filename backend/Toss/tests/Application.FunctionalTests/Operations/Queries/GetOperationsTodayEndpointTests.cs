using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Constants;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Operations.Queries;

using static Testing;

public class GetOperationsTodayEndpointTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAggregatedSnapshotForAuthorizedBusiness()
    {
        var email = "ops-user@local";
        var password = "Testing1234!";
        var userId = await RunAsUserAsync(email, password, Array.Empty<string>());
        var business = await CreateBusinessAsync();
        await AddBusinessMembershipAsync(userId, business, BusinessRoles.Owner, true);

        var store = new Store
        {
            BusinessId = business.Id,
            Name = "Main Shop",
            OwnerId = userId,
            Email = "shop@ops.local",
            IsActive = true
        };
        await AddAsync(store);

        var vendor = new Vendor
        {
            BusinessId = business.Id,
            Name = "Township Supplier"
        };
        await AddAsync(vendor);

        var product = new Product
        {
            BusinessId = business.Id,
            Name = "Maize Meal 10kg",
            SKU = "MM-10",
            MinimumStockLevel = 5,
            ReorderQuantity = 20
        };
        await AddAsync(product);

        var customer = new Customer
        {
            BusinessId = business.Id,
            StoreId = store.Id,
            Store = store,
            FirstName = "Thabo",
            LastName = "Ndlovu",
            Email = "thabo@example.com",
            IsActive = true
        };
        await AddAsync(customer);

        var now = DateTimeOffset.UtcNow;

        await AddAsync(new Sale
        {
            ShopId = store.Id,
            SaleDate = now.AddMinutes(-15),
            Status = SaleStatus.Completed,
            Total = 240m
        });

        await AddAsync(new Payment
        {
            ShopId = store.Id,
            Amount = 240m,
            PaymentDate = now.AddMinutes(-10),
            Status = PaymentStatus.Completed,
            SourceType = "Sale"
        });

        await AddAsync(new Payment
        {
            ShopId = store.Id,
            Amount = -90m,
            PaymentDate = now.AddMinutes(-5),
            Status = PaymentStatus.Completed,
            SourceType = "PurchaseOrder"
        });

        await AddAsync(new StockAlert
        {
            ShopId = store.Id,
            ProductId = product.Id,
            CurrentStock = 3,
            MinimumStock = 5,
            IsAcknowledged = false
        });

        await AddAsync(new PurchaseOrder
        {
            ShopId = store.Id,
            VendorId = vendor.Id,
            PONumber = "PO-OPS-1",
            Status = PurchaseOrderStatus.Pending,
            OrderDate = now.AddDays(-1),
            ExpectedDeliveryDate = now.AddHours(4),
            Total = 500m
        });

        await AddAsync(new Order
        {
            CustomerId = customer.Id,
            OrderStatus = OrderStatus.Pending,
            OrderTotal = 320m
        });

        var client = CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginPayload.GetProperty("token").GetString();
        Assert.That(token, Is.Not.Empty);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Business-Id", business.Id.ToString());

        var response = await client.GetAsync("/api/operations/today");
        response.EnsureSuccessStatusCode();
        var todayPayload = await response.Content.ReadFromJsonAsync<JsonElement>();

        var totals = todayPayload.GetProperty("totals");
        Assert.That(totals.GetProperty("salesTotal").GetDecimal(), Is.EqualTo(240m));
        Assert.That(totals.GetProperty("transactions").GetInt32(), Is.EqualTo(1));
        Assert.That(totals.GetProperty("averageTicket").GetDecimal(), Is.EqualTo(240m));

        var cash = todayPayload.GetProperty("cash");
        Assert.That(cash.GetProperty("cashIn").GetDecimal(), Is.EqualTo(240m));
        Assert.That(cash.GetProperty("cashOut").GetDecimal(), Is.EqualTo(90m));

        Assert.That(todayPayload.GetProperty("lowStock").GetArrayLength(), Is.GreaterThanOrEqualTo(1));
        Assert.That(todayPayload.GetProperty("pendingPurchaseOrders").GetArrayLength(), Is.EqualTo(1));
        Assert.That(todayPayload.GetProperty("pendingCustomerOrders").GetArrayLength(), Is.EqualTo(1));
        Assert.That(todayPayload.GetProperty("alerts").GetArrayLength(), Is.GreaterThanOrEqualTo(1));
    }
}

