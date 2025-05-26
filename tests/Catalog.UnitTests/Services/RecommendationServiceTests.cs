using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Services;

public class RecommendationServiceTests
{
    private readonly Mock<ICatalogService> _catalogServiceMock;
    private readonly IRecommendationService _recommendationService;
    private readonly string _userId = "test-user";
    private readonly int _itemId = 1;

    public RecommendationServiceTests()
    {
        _catalogServiceMock = new Mock<ICatalogService>();
        _recommendationService = new RecommendationService(_catalogServiceMock.Object);
    }

    [Fact]
    public async Task GetPersonalizedRecommendations_ShouldReturnItems()
    {
        // Arrange
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetPersonalizedRecommendationsAsync(_userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetRelatedItems_ShouldReturnItems()
    {
        // Arrange
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1" },
            new() { Id = 3, Name = "Related Item 2" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetRelatedItemsAsync(_itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetFrequentlyBoughtTogether_ShouldReturnItems()
    {
        // Arrange
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Frequently Bought Item 1" },
            new() { Id = 3, Name = "Frequently Bought Item 2" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetFrequentlyBoughtTogetherAsync(_itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetTrendingItems_ShouldReturnItems()
    {
        // Arrange
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Trending Item 1" },
            new() { Id = 2, Name = "Trending Item 2" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetTrendingItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetNewArrivals_ShouldReturnItems()
    {
        // Arrange
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "New Item 1" },
            new() { Id = 2, Name = "New Item 2" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetNewArrivalsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetPersonalizedRecommendations_ShouldRespectCount()
    {
        // Arrange
        var count = 3;
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" },
            new() { Id = 3, Name = "Item 3" },
            new() { Id = 4, Name = "Item 4" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetPersonalizedRecommendationsAsync(_userId, count);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count());
    }

    [Fact]
    public async Task GetRelatedItems_ShouldRespectCount()
    {
        // Arrange
        var count = 2;
        var expectedItems = new List<CatalogItemDto>
        {
            new() { Id = 2, Name = "Related Item 1" },
            new() { Id = 3, Name = "Related Item 2" },
            new() { Id = 4, Name = "Related Item 3" }
        };

        _catalogServiceMock.Setup(x => x.GetCatalogItemsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _recommendationService.GetRelatedItemsAsync(_itemId, count);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count());
    }
} 
