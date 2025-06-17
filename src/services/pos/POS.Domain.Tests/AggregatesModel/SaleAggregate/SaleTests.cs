using FluentAssertions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Tests.Common;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.SaleAggregate;

public class SaleTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateSale()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();

        // Assert
        sale.Should().NotBeNull();
        sale.Status.Should().Be(SaleStatus.Pending);
        sale.Items.Should().BeEmpty();
        sale.Payments.Should().BeEmpty();
        sale.Discounts.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddItem()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();

        // Act
        sale.AddItem(item);

        // Assert
        sale.Items.Should().ContainSingle();
        sale.Items.Should().Contain(item);
    }

    [Fact]
    public void AddItem_WithInvalidItem_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();
        sale.AddItem(item);

        // Act & Assert
        var action = () => sale.AddItem(item);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Item already exists in the sale");
    }

    [Fact]
    public void AddPayment_WithValidPayment_ShouldAddPayment()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();

        // Act
        sale.AddPayment(payment);

        // Assert
        sale.Payments.Should().ContainSingle();
        sale.Payments.Should().Contain(payment);
    }

    [Fact]
    public void AddPayment_WithInvalidPayment_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();
        sale.AddPayment(payment);

        // Act & Assert
        var action = () => sale.AddPayment(payment);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Payment already exists in the sale");
    }

    [Fact]
    public void AddDiscount_WithValidDiscount_ShouldAddDiscount()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var discount = TestDataFactory.Sale.CreateValidSaleDiscount();

        // Act
        sale.AddDiscount(discount);

        // Assert
        sale.Discounts.Should().ContainSingle();
        sale.Discounts.Should().Contain(discount);
    }

    [Fact]
    public void AddDiscount_WithInvalidDiscount_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var discount = TestDataFactory.Sale.CreateValidSaleDiscount();
        sale.AddDiscount(discount);

        // Act & Assert
        var action = () => sale.AddDiscount(discount);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Discount already exists in the sale");
    }

    [Fact]
    public void Process_WithValidData_ShouldProcessSale()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();
        sale.AddItem(item);
        sale.AddPayment(payment);

        // Act
        sale.Process();

        // Assert
        sale.Status.Should().Be(SaleStatus.Processing);
    }

    [Fact]
    public void Process_WithNoItems_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();

        // Act & Assert
        var action = () => sale.Process();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot process sale with no items");
    }

    [Fact]
    public void Complete_WithValidData_ShouldCompleteSale()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();
        sale.AddItem(item);
        sale.AddPayment(payment);
        sale.Process();

        // Act
        sale.Complete();

        // Assert
        sale.Status.Should().Be(SaleStatus.Completed);
    }

    [Fact]
    public void Complete_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();

        // Act & Assert
        var action = () => sale.Complete();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot complete sale that is not in processing status");
    }

    [Fact]
    public void Void_WithValidData_ShouldVoidSale()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();
        sale.AddItem(item);
        sale.AddPayment(payment);
        sale.Process();

        // Act
        sale.Void("Test reason");

        // Assert
        sale.Status.Should().Be(SaleStatus.Voided);
    }

    [Fact]
    public void Void_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();

        // Act & Assert
        var action = () => sale.Void("Test reason");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot void sale that is not in processing status");
    }

    [Fact]
    public void Cancel_WithValidData_ShouldCancelSale()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();

        // Act
        sale.Cancel("Test reason");

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var sale = TestDataFactory.Sale.CreateValidSale();
        var item = TestDataFactory.Sale.CreateValidSaleItem();
        var payment = TestDataFactory.Sale.CreateValidSalePayment();
        sale.AddItem(item);
        sale.AddPayment(payment);
        sale.Process();
        sale.Complete();

        // Act & Assert
        var action = () => sale.Cancel("Test reason");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel sale that is not in pending status");
    }
} 
