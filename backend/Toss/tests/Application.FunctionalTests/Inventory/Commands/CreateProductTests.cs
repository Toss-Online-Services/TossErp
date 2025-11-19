using Toss.Application.Common.Exceptions;
using Toss.Application.Inventory.Commands.CreateProduct;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.FunctionalTests.Inventory.Commands;

using static Testing;

public class CreateProductTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateProductCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateProduct()
    {
        await RunAsDefaultUserAsync();

        var command = new CreateProductCommand
        {
            Name = "Test Product",
            SKU = "TEST-SKU-001",
            BasePrice = 99.99m,
            Description = "A test product"
        };

        var productId = await SendAsync(command);

        var product = await FindAsync<Product>(productId);

        product.ShouldNotBeNull();
        product!.Name.ShouldBe(command.Name);
        product.SKU.ShouldBe(command.SKU);
        product.BasePrice.ShouldBe(command.BasePrice);
        product.Description.ShouldBe(command.Description);
    }

    [Test]
    public async Task ShouldRequireUniqueSKU()
    {
        await RunAsDefaultUserAsync();

        var command1 = new CreateProductCommand
        {
            Name = "Product 1",
            SKU = "DUPLICATE-SKU",
            BasePrice = 10
        };

        await SendAsync(command1);

        var command2 = new CreateProductCommand
        {
            Name = "Product 2",
            SKU = "DUPLICATE-SKU", // Same SKU
            BasePrice = 20
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command2));
    }

    [Test]
    public async Task ShouldCreateProductWithBarcode()
    {
        await RunAsDefaultUserAsync();

        var command = new CreateProductCommand
        {
            Name = "Barcode Product",
            SKU = "BAR-001",
            Barcode = "1234567890123",
            BasePrice = 15.50m
        };

        var productId = await SendAsync(command);

        var product = await FindAsync<Product>(productId);

        product.ShouldNotBeNull();
        product!.Barcode.ShouldBe(command.Barcode);
    }
}

