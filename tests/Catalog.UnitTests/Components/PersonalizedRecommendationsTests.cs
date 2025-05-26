using Bunit;
using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Components;

public class PersonalizedRecommendationsTests : TestContext
{
    private readonly Mock<IRecommendationService> _recommendationServiceMock;
    private readonly string _userId = "test-user";

    public PersonalizedRecommendationsTests()
    {
        _recommendationServiceMock = new Mock<IRecommendationService>();
        Services.AddSingleton(_recommendationServiceMock.Object);
    }

    [Fact]
    public void ShouldDisplayRecommendations()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100, Rating = 4 },
            new() { Id = 2, Name = "Item 2", PictureUri = "image2.jpg", Price = 200, Rating = 5 }
        };

        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(_userId))
            .ReturnsAsync(items);

        // Act
        var cut = RenderComponent<PersonalizedRecommendations>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.FindAll(".recommendation-card").Count == 2);
        Assert.Equal(2, cut.FindAll(".recommendation-card").Count);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
    }

    [Fact]
    public void ShouldDisplayEmptyStateWhenNoRecommendations()
    {
        // Arrange
        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(_userId))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<PersonalizedRecommendations>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.Find(".empty-state") != null);
        Assert.Contains("No recommendations available yet", cut.Markup);
    }

    [Fact]
    public void ShouldNavigateToItemDetailsOnClick()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100 }
        };

        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(_userId))
            .ReturnsAsync(items);

        var cut = RenderComponent<PersonalizedRecommendations>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".recommendation-card") != null);
        cut.Find(".recommendation-card").Click();

        // Assert
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Equal($"/catalog/item/1", navigationManager.Uri);
    }

    [Fact]
    public void ShouldNavigateToCatalogOnBrowseClick()
    {
        // Arrange
        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(_userId))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        var cut = RenderComponent<PersonalizedRecommendations>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Act
        cut.WaitForState(() => cut.Find(".browse-button") != null);
        cut.Find(".browse-button").Click();

        // Assert
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Equal("/catalog", navigationManager.Uri);
    }

    [Fact]
    public void ShouldNotLoadRecommendationsWhenUserIdIsEmpty()
    {
        // Arrange
        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<CatalogItemDto>());

        // Act
        var cut = RenderComponent<PersonalizedRecommendations>();

        // Assert
        _recommendationServiceMock.Verify(x => x.GetPersonalizedRecommendationsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShouldDisplayOutOfStockBadge()
    {
        // Arrange
        var items = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1", PictureUri = "image1.jpg", Price = 100, AvailableStock = 0 }
        };

        _recommendationServiceMock.Setup(x => x.GetPersonalizedRecommendationsAsync(_userId))
            .ReturnsAsync(items);

        // Act
        var cut = RenderComponent<PersonalizedRecommendations>(parameters => parameters
            .Add(p => p.UserId, _userId));

        // Assert
        cut.WaitForState(() => cut.Find(".out-of-stock-badge") != null);
        Assert.Contains("Out of Stock", cut.Markup);
    }
} 
