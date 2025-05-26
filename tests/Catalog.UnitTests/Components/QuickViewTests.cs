using Bunit;
using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Components;

public class QuickViewTests : TestContext
{
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly Mock<IProductComparisonService> _comparisonServiceMock;
    private readonly string _userId = "test-user";
    private readonly CatalogItemDto _item;

    public QuickViewTests()
    {
        _cartServiceMock = new Mock<ICartService>();
        _comparisonServiceMock = new Mock<IProductComparisonService>();
        Services.AddSingleton(_cartServiceMock.Object);
        Services.AddSingleton(_comparisonServiceMock.Object);

        _item = new CatalogItemDto
        {
            Id = 1,
            Name = "Test Item",
            Description = "Test Description",
            Price = 100,
            PictureUri = "test.jpg",
            AvailableStock = 10,
            Rating = 4
        };
    }

    [Fact]
    public void ShouldDisplayItemDetails()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        // Act
        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Assert
        Assert.Contains(_item.Name, cut.Markup);
        Assert.Contains(_item.Description, cut.Markup);
        Assert.Contains(_item.Price.ToString("C"), cut.Markup);
        Assert.Contains("In Stock", cut.Markup);
    }

    [Fact]
    public void ShouldAddToCart()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".add-to-cart") != null);
        cut.Find(".add-to-cart").Click();

        // Assert
        _cartServiceMock.Verify(x => x.AddToCartAsync(_userId, _item.Id, 1), Times.Once);
    }

    [Fact]
    public void ShouldToggleComparison()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".compare-button") != null);
        cut.Find(".compare-button").Click();

        // Assert
        _comparisonServiceMock.Verify(x => x.AddToComparisonAsync(_userId, _item.Id), Times.Once);
    }

    [Fact]
    public void ShouldUpdateQuantity()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".quantity-controls") != null);
        var quantityInput = cut.Find("input[type='number']");
        quantityInput.Change(5);

        // Assert
        cut.WaitForState(() => cut.Find(".add-to-cart") != null);
        cut.Find(".add-to-cart").Click();
        _cartServiceMock.Verify(x => x.AddToCartAsync(_userId, _item.Id, 5), Times.Once);
    }

    [Fact]
    public void ShouldNotAllowQuantityBelowOne()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".quantity-controls") != null);
        var quantityInput = cut.Find("input[type='number']");
        quantityInput.Change(0);

        // Assert
        cut.WaitForState(() => cut.Find(".add-to-cart") != null);
        cut.Find(".add-to-cart").Click();
        _cartServiceMock.Verify(x => x.AddToCartAsync(_userId, _item.Id, 1), Times.Once);
    }

    [Fact]
    public void ShouldNotAllowQuantityAboveStock()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".quantity-controls") != null);
        var quantityInput = cut.Find("input[type='number']");
        quantityInput.Change(20);

        // Assert
        cut.WaitForState(() => cut.Find(".add-to-cart") != null);
        cut.Find(".add-to-cart").Click();
        _cartServiceMock.Verify(x => x.AddToCartAsync(_userId, _item.Id, 10), Times.Once);
    }

    [Fact]
    public void ShouldCloseOnAddToCart()
    {
        // Arrange
        var onCloseCalled = false;
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => onCloseCalled = true)));

        // Act
        cut.WaitForState(() => cut.Find(".add-to-cart") != null);
        cut.Find(".add-to-cart").Click();

        // Assert
        Assert.True(onCloseCalled);
    }

    [Fact]
    public void ShouldCloseOnViewDetails()
    {
        // Arrange
        var onCloseCalled = false;
        var onViewDetailsCalled = false;
        _comparisonServiceMock.Setup(x => x.IsInComparisonAsync(_userId, _item.Id))
            .ReturnsAsync(false);

        var cut = RenderComponent<QuickView>(parameters => parameters
            .Add(p => p.Item, _item)
            .Add(p => p.UserId, _userId)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => onCloseCalled = true))
            .Add(p => p.OnViewDetails, EventCallback.Factory.Create<int>(this, _ => onViewDetailsCalled = true)));

        // Act
        cut.WaitForState(() => cut.Find(".view-details") != null);
        cut.Find(".view-details").Click();

        // Assert
        Assert.True(onCloseCalled);
        Assert.True(onViewDetailsCalled);
    }
} 
