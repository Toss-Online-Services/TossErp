using Bunit;
using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Components;

public class RelatedItemsTests : TestContext
{
    private readonly Mock<IRecommendationService> _recommendationServiceMock;
    private readonly int _itemId = 1;

    public RelatedItemsTests()
    {
        _recommendationServiceMock = new Mock<IRecommendationService>();
        Services.AddSingleton(_recommendationServiceMock.Object);
    }

    [Fact]
    public void ShouldDisplayRelatedItems()
    {
        // Arrange
        var relatedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1", PictureUri = "image1.jpg", Price = 100, Rating = 4 },
            new() { Id = 3, Name = "Related Item 2", PictureUri = "image2.jpg", Price = 200, Rating = 5 }
        };

        _recommendationServiceMock.Setup(x => x.GetRelatedItemsAsync(_itemId))
            .ReturnsAsync(relatedItems);

        // Act
        var cut = RenderComponent<RelatedItems>(parameters => parameters
            .Add(p => p.CatalogItemId, _itemId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".item-card").Count == 2);
        Assert.Equal(2, cut.FindAll(".item-card").Count);
        Assert.Contains("Related Item 1", cut.Markup);
        Assert.Contains("Related Item 2", cut.Markup);
    }

    [Fact]
    public void ShouldDisplayFrequentlyBoughtItems()
    {
        // Arrange
        var frequentlyBoughtItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Frequently Bought Item 1", PictureUri = "image1.jpg", Price = 100 },
            new() { Id = 3, Name = "Frequently Bought Item 2", PictureUri = "image2.jpg", Price = 200 }
        };

        _recommendationServiceMock.Setup(x => x.GetFrequentlyBoughtTogetherAsync(_itemId))
            .ReturnsAsync(frequentlyBoughtItems);

        // Act
        var cut = RenderComponent<RelatedItems>(parameters => parameters
            .Add(p => p.CatalogItemId, _itemId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".item-card").Count == 2);
        Assert.Equal(2, cut.FindAll(".item-card").Count);
        Assert.Contains("Frequently Bought Item 1", cut.Markup);
        Assert.Contains("Frequently Bought Item 2", cut.Markup);
    }

    [Fact]
    public void ShouldNavigateToItemDetailsOnClick()
    {
        // Arrange
        var relatedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _recommendationServiceMock.Setup(x => x.GetRelatedItemsAsync(_itemId))
            .ReturnsAsync(relatedItems);

        var cut = RenderComponent<RelatedItems>(parameters => parameters
            .Add(p => p.CatalogItemId, _itemId));

        // Act
        cut.WaitForState(() => cut.Find(".item-card") != null);
        cut.Find(".item-card").Click();

        // Assert
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Equal($"/catalog/item/2", navigationManager.Uri);
    }

    [Fact]
    public void ShouldDisplayOutOfStockBadge()
    {
        // Arrange
        var relatedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1", PictureUri = "image1.jpg", Price = 100, AvailableStock = 0 }
        };

        _recommendationServiceMock.Setup(x => x.GetRelatedItemsAsync(_itemId))
            .ReturnsAsync(relatedItems);

        // Act
        var cut = RenderComponent<RelatedItems>(parameters => parameters
            .Add(p => p.CatalogItemId, _itemId));

        // Assert
        cut.WaitForState(() => cut.Find(".out-of-stock-badge") != null);
        Assert.Contains("Out of Stock", cut.Markup);
    }

    [Fact]
    public void ShouldLoadBothRelatedAndFrequentlyBoughtItems()
    {
        // Arrange
        var relatedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        var frequentlyBoughtItems = new List<CatalogItemDto>
        {
            new() { Id = 3, Name = "Frequently Bought Item 1", PictureUri = "image2.jpg", Price = 200 }
        };

        _recommendationServiceMock.Setup(x => x.GetRelatedItemsAsync(_itemId))
            .ReturnsAsync(relatedItems);

        _recommendationServiceMock.Setup(x => x.GetFrequentlyBoughtTogetherAsync(_itemId))
            .ReturnsAsync(frequentlyBoughtItems);

        // Act
        var cut = RenderComponent<RelatedItems>(parameters => parameters
            .Add(p => p.CatalogItemId, _itemId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".item-card").Count == 2);
        Assert.Equal(2, cut.FindAll(".item-card").Count);
        Assert.Contains("Related Item 1", cut.Markup);
        Assert.Contains("Frequently Bought Item 1", cut.Markup);
    }
} 
