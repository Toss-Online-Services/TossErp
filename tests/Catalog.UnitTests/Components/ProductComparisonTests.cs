using Bunit;
using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Components;

public class ProductComparisonTests : TestContext
{
    private readonly Mock<IProductComparisonService> _comparisonServiceMock;
    private readonly string _userId = "test-user";

    public ProductComparisonTests()
    {
        _comparisonServiceMock = new Mock<IProductComparisonService>();
        Services.AddSingleton(_comparisonServiceMock.Object);
    }

    [Fact]
    public void ShouldDisplayComparisonItems()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() 
            { 
                Id = 1, 
                Name = "Item 1", 
                PictureUri = "image1.jpg", 
                Price = 100,
                CatalogBrand = new CatalogBrandDto { Name = "Brand 1" },
                CatalogType = new CatalogTypeDto { Name = "Type 1" },
                AvailableStock = 10,
                Rating = 4
            },
            new() 
            { 
                Id = 2, 
                Name = "Item 2", 
                PictureUri = "image2.jpg", 
                Price = 200,
                CatalogBrand = new CatalogBrandDto { Name = "Brand 2" },
                CatalogType = new CatalogTypeDto { Name = "Type 2" },
                AvailableStock = 0,
                Rating = 3
            }
        };

        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(_userId))
            .ReturnsAsync(items);

        // Act
        var cut = RenderComponent<ProductComparison>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".comparison-row").Count > 1);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("Brand 1", cut.Markup);
        Assert.Contains("Type 1", cut.Markup);
    }

    [Fact]
    public void ShouldDisplayEmptyMessageWhenNoItems()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(_userId))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<ProductComparison>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.Find(".empty-comparison") != null);
        Assert.Contains("No products to compare", cut.Markup);
    }

    [Fact]
    public void ShouldRemoveItemFromComparison()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(_userId))
            .ReturnsAsync(items);

        var cut = RenderComponent<ProductComparison>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".remove-button") != null);
        cut.Find(".remove-button").Click();

        // Assert
        _comparisonServiceMock.Verify(x => x.RemoveFromComparisonAsync(_userId, 1), Times.Once);
    }

    [Fact]
    public void ShouldClearComparison()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(_userId))
            .ReturnsAsync(items);

        var cut = RenderComponent<ProductComparison>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".clear-comparison") != null);
        cut.Find(".clear-comparison").Click();

        // Assert
        _comparisonServiceMock.Verify(x => x.ClearComparisonAsync(_userId), Times.Once);
    }

    [Fact]
    public void ShouldNavigateToItemDetails()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(_userId))
            .ReturnsAsync(items);

        var cut = RenderComponent<ProductComparison>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".view-button") != null);
        cut.Find(".view-button").Click();

        // Assert
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Equal($"/catalog/item/1", navigationManager.Uri);
    }

    [Fact]
    public void ShouldNotLoadItemsWhenUserIdIsEmpty()
    {
        // Arrange
        _comparisonServiceMock.Setup(x => x.GetComparisonItemsAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<ProductComparison>();

        // Assert
        _comparisonServiceMock.Verify(x => x.GetComparisonItemsAsync(It.IsAny<string>()), Times.Never);
    }
} 
