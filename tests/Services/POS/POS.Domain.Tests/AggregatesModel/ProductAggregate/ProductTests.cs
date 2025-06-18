using FluentAssertions;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.ProductAggregate;

public class ProductTests
{
    private readonly string _name;
    private readonly string _description;
    private readonly string _sku;
    private readonly string _barcode;
    private readonly Money _price;
    private readonly decimal _cost;
    private readonly string _category;
    private readonly string _brand;
    private readonly string _unit;
    private readonly decimal _taxRate;
    private readonly bool _isActive;
    private readonly bool _trackInventory;
    private readonly decimal _reorderPoint;
    private readonly decimal _reorderQuantity;
    private readonly string _location;

    public ProductTests()
    {
        _name = "Test Product";
        _description = "Test Description";
        _sku = "SKU-001";
        _barcode = "123456789";
        _price = new Money(99.99m, "USD");
        _cost = 50.00m;
        _category = "Electronics";
        _brand = "Test Brand";
        _unit = "Each";
        _taxRate = 0.10m;
        _isActive = true;
        _trackInventory = true;
        _reorderPoint = 10;
        _reorderQuantity = 20;
        _location = "A-1-1";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesProductSuccessfully()
    {
        // Arrange & Act
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        product.Name.Should().Be(_name);
        product.Description.Should().Be(_description);
        product.SKU.Should().Be(_sku);
        product.Barcode.Should().Be(_barcode);
        product.Price.Should().Be(_price);
        product.Cost.Should().Be(_cost);
        product.Category.Should().Be(_category);
        product.Brand.Should().Be(_brand);
        product.Unit.Should().Be(_unit);
        product.TaxRate.Should().Be(_taxRate);
        product.IsActive.Should().Be(_isActive);
        product.TrackInventory.Should().Be(_trackInventory);
        product.ReorderPoint.Should().Be(_reorderPoint);
        product.ReorderQuantity.Should().Be(_reorderQuantity);
        product.Location.Should().Be(_location);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.StockQuantity.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsDomainException()
    {
        // Act
        var action = () => new Product(
            string.Empty,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Product name cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptySKU_ThrowsDomainException()
    {
        // Act
        var action = () => new Product(
            _name,
            _description,
            string.Empty,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("SKU cannot be empty");
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsDomainException()
    {
        // Arrange
        var negativePrice = new Money(-99.99m, "USD");

        // Act
        var action = () => new Product(
            _name,
            _description,
            _sku,
            _barcode,
            negativePrice,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Price cannot be negative");
    }

    [Fact]
    public void Constructor_WithNegativeCost_ThrowsDomainException()
    {
        // Arrange
        var negativeCost = -50.00m;

        // Act
        var action = () => new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            negativeCost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cost cannot be negative");
    }

    [Fact]
    public void Constructor_WithNegativeTaxRate_ThrowsDomainException()
    {
        // Arrange
        var negativeTaxRate = -0.10m;

        // Act
        var action = () => new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            negativeTaxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Tax rate cannot be negative");
    }

    [Fact]
    public void UpdateStock_WithValidQuantity_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        var quantity = 10;

        // Act
        product.UpdateStock(quantity);

        // Assert
        product.StockQuantity.Should().Be(quantity);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStock_WithNegativeQuantity_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        var action = () => product.UpdateStock(-10);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Stock quantity cannot be negative");
    }

    [Fact]
    public void UpdateStock_WhenNotTrackingInventory_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            false, // Not tracking inventory
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        var action = () => product.UpdateStock(10);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot update stock for a product that does not track inventory");
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        var newPrice = new Money(149.99m, "USD");

        // Act
        product.UpdatePrice(newPrice);

        // Assert
        product.Price.Should().Be(newPrice);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        var negativePrice = new Money(-149.99m, "USD");

        // Act
        var action = () => product.UpdatePrice(negativePrice);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Price cannot be negative");
    }

    [Fact]
    public void UpdateCost_WithValidCost_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        var newCost = 75.00m;

        // Act
        product.UpdateCost(newCost);

        // Assert
        product.Cost.Should().Be(newCost);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateCost_WithNegativeCost_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            _isActive,
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        var negativeCost = -75.00m;

        // Act
        var action = () => product.UpdateCost(negativeCost);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cost cannot be negative");
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            true, // Active
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            false, // Already inactive
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        var action = () => product.Deactivate();

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Product is already inactive");
    }

    [Fact]
    public void Reactivate_WhenInactive_ReactivatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            false, // Inactive
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        product.Reactivate();

        // Assert
        product.IsActive.Should().BeTrue();
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _cost,
            _category,
            _brand,
            _unit,
            _taxRate,
            true, // Already active
            _trackInventory,
            _reorderPoint,
            _reorderQuantity,
            _location);

        // Act
        var action = () => product.Reactivate();

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Product is already active");
    }
} 
