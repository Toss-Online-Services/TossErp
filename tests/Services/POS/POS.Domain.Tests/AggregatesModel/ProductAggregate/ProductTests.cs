using FluentAssertions;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.ValueObjects;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.ProductAggregate;

public class ProductTests
{
    private readonly string _name;
    private readonly string _description;
    private readonly string _sku;
    private readonly string _barcode;
    private readonly Money _price;
    private readonly decimal _costPrice;
    private readonly int _categoryId;
    private readonly Guid _storeId;
    private readonly int _stockQuantity;
    private readonly int _lowStockThreshold;

    public ProductTests()
    {
        _name = "Test Product";
        _description = "Test Description";
        _sku = "TEST-SKU-001";
        _barcode = "123456789012";
        _price = new Money(99.99m, "USD");
        _costPrice = 49.99m;
        _categoryId = 1;
        _storeId = Guid.NewGuid();
        _stockQuantity = 100;
        _lowStockThreshold = 10;
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesProductSuccessfully()
    {
        // Act
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Assert
        product.Name.Should().Be(_name);
        product.Description.Should().Be(_description);
        product.SKU.Should().Be(_sku);
        product.Barcode.Should().Be(_barcode);
        product.Price.Should().Be(_price);
        product.CostPrice.Should().Be(_costPrice);
        product.CategoryId.Should().Be(_categoryId);
        product.StoreId.Should().Be(_storeId);
        product.StockQuantity.Should().Be(_stockQuantity);
        product.LowStockThreshold.Should().Be(_lowStockThreshold);
        product.IsActive.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.LastModifiedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ThrowsDomainException(string name)
    {
        // Act
        var act = () => new Product(
            name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptySKU_ThrowsDomainException(string sku)
    {
        // Act
        var act = () => new Product(
            _name,
            _description,
            sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyBarcode_ThrowsDomainException(string barcode)
    {
        // Act
        var act = () => new Product(
            _name,
            _description,
            _sku,
            barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        var newName = "Updated Product";
        var newDescription = "Updated Description";
        var newPrice = new Money(149.99m, "USD");
        var newCostPrice = 74.99m;
        var newCategoryId = 2;

        // Act
        product.UpdateDetails(
            newName,
            newDescription,
            newPrice,
            newCostPrice,
            newCategoryId);

        // Assert
        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.Price.Should().Be(newPrice);
        product.CostPrice.Should().Be(newCostPrice);
        product.CategoryId.Should().Be(newCategoryId);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyName_ThrowsDomainException(string name)
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Act
        var act = () => product.UpdateDetails(
            name,
            _description,
            _price,
            _costPrice,
            _categoryId);

        // Assert
        act.Should().Throw<DomainException>();
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
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        var newQuantity = 50;

        // Act
        product.UpdateStock(newQuantity);

        // Assert
        product.StockQuantity.Should().Be(newQuantity);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AdjustStock_WithPositiveAdjustment_IncreasesStock()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        var adjustment = 50;

        // Act
        product.AdjustStock(adjustment);

        // Assert
        product.StockQuantity.Should().Be(_stockQuantity + adjustment);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AdjustStock_WithNegativeAdjustment_DecreasesStock()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        var adjustment = -30;

        // Act
        product.AdjustStock(adjustment);

        // Assert
        product.StockQuantity.Should().Be(_stockQuantity + adjustment);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetLowStockThreshold_WithValidThreshold_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        var newThreshold = 20;

        // Act
        product.SetLowStockThreshold(newThreshold);

        // Assert
        product.LowStockThreshold.Should().Be(newThreshold);
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsLowStock_WhenStockBelowThreshold_ReturnsTrue()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        product.UpdateStock(_lowStockThreshold - 1);

        // Act
        var result = product.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenStockAboveThreshold_ReturnsFalse()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        product.UpdateStock(_lowStockThreshold + 1);

        // Act
        var result = product.IsLowStock();

        // Assert
        result.Should().BeFalse();
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
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

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
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        product.Deactivate();

        // Act
        var act = () => product.Deactivate();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Activate_WhenInactive_ActivatesSuccessfully()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
        product.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ThrowsDomainException()
    {
        // Arrange
        var product = new Product(
            _name,
            _description,
            _sku,
            _barcode,
            _price,
            _costPrice,
            _categoryId,
            _storeId,
            _stockQuantity,
            _lowStockThreshold);

        // Act
        var act = () => product.Activate();

        // Assert
        act.Should().Throw<DomainException>();
    }
} 
