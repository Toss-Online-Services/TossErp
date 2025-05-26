using Bunit;
using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Components;

public class RecentlyViewedTests : TestContext
{
    private readonly Mock<IRecentlyViewedService> _recentlyViewedServiceMock;
    private readonly string _userId = "test-user";

    public RecentlyViewedTests()
    {
        _recentlyViewedServiceMock = new Mock<IRecentlyViewedService>();
        Services.AddSingleton(_recentlyViewedServiceMock.Object);
    }

    [Fact]
    public void ShouldDisplayRecentlyViewedItems()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 },
            new() { Id = 2, Name = "Item 2", PictureUri = "image2.jpg", Price = 200 }
        };

        _recentlyViewedServiceMock.Setup(x => x.GetRecentlyViewedAsync(_userId))
            .ReturnsAsync(items);

        // Act
        var cut = RenderComponent<RecentlyViewed>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".item-card").Count == 2);
        Assert.Equal(2, cut.FindAll(".item-card").Count);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
    }

    [Fact]
    public void ShouldDisplayEmptyMessageWhenNoItems()
    {
        // Arrange
        _recentlyViewedServiceMock.Setup(x => x.GetRecentlyViewedAsync(_userId))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<RecentlyViewed>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.Find(".empty-message") != null);
        Assert.Contains("No recently viewed items", cut.Markup);
    }

    [Fact]
    public void ShouldNavigateToItemDetailsOnClick()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _recentlyViewedServiceMock.Setup(x => x.GetRecentlyViewedAsync(_userId))
            .ReturnsAsync(items);

        var cut = RenderComponent<RecentlyViewed>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".item-card") != null);
        cut.Find(".item-card").Click();

        // Assert
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Equal($"/catalog/item/1", navigationManager.Uri);
    }

    [Fact]
    public void ShouldNotLoadItemsWhenUserIdIsEmpty()
    {
        // Arrange
        _recentlyViewedServiceMock.Setup(x => x.GetRecentlyViewedAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<RecentlyViewed>();

        // Assert
        _recentlyViewedServiceMock.Verify(x => x.GetRecentlyViewedAsync(It.IsAny<string>()), Times.Never);
    }
} 
