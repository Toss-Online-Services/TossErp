using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Services;

public class ProductComparisonServiceTests
{
    private readonly Mock<ICatalogService> _catalogServiceMock;
    private readonly IProductComparisonService _comparisonService;
    private readonly string _userId = "test-user";
    private readonly int _itemId = 1;

    public ProductComparisonServiceTests()
    {
        _catalogServiceMock = new Mock<ICatalogService>();
        _comparisonService = new ProductComparisonService(_catalogServiceMock.Object);
    }

    [Fact]
    public async Task GetComparisonItems_ShouldReturnItems()
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
        var result = await _comparisonService.GetComparisonItemsAsync(_userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task AddToComparison_ShouldAddItem()
    {
        // Arrange
        var item = new CatalogItemDto { Id = _itemId, Name = "Test Item" };
        _catalogServiceMock.Setup(x => x.GetCatalogItemAsync(_itemId))
            .ReturnsAsync(item);

        // Act
        await _comparisonService.AddToComparisonAsync(_userId, _itemId);

        // Assert
        var result = await _comparisonService.GetComparisonItemsAsync(_userId);
        Assert.Contains(result, x => x.Id == _itemId);
    }

    [Fact]
    public async Task RemoveFromComparison_ShouldRemoveItem()
    {
        // Arrange
        var item = new CatalogItemDto { Id = _itemId, Name = "Test Item" };
        _catalogServiceMock.Setup(x => x.GetCatalogItemAsync(_itemId))
            .ReturnsAsync(item);

        await _comparisonService.AddToComparisonAsync(_userId, _itemId);

        // Act
        await _comparisonService.RemoveFromComparisonAsync(_userId, _itemId);

        // Assert
        var result = await _comparisonService.GetComparisonItemsAsync(_userId);
        Assert.DoesNotContain(result, x => x.Id == _itemId);
    }

    [Fact]
    public async Task ClearComparison_ShouldRemoveAllItems()
    {
        // Arrange
        var items = Enumerable.Range(1, 3)
            .Select(i => new CatalogItemDto { Id = i, Name = $"Item {i}" })
            .ToList();

        foreach (var item in items)
        {
            _catalogServiceMock.Setup(x => x.GetCatalogItemAsync(item.Id))
                .ReturnsAsync(item);
            await _comparisonService.AddToComparisonAsync(_userId, item.Id);
        }

        // Act
        await _comparisonService.ClearComparisonAsync(_userId);

        // Assert
        var result = await _comparisonService.GetComparisonItemsAsync(_userId);
        Assert.Empty(result);
    }

    [Fact]
    public async Task IsInComparison_ShouldReturnCorrectStatus()
    {
        // Arrange
        var item = new CatalogItemDto { Id = _itemId, Name = "Test Item" };
        _catalogServiceMock.Setup(x => x.GetCatalogItemAsync(_itemId))
            .ReturnsAsync(item);

        // Act & Assert
        Assert.False(await _comparisonService.IsInComparisonAsync(_userId, _itemId));

        await _comparisonService.AddToComparisonAsync(_userId, _itemId);
        Assert.True(await _comparisonService.IsInComparisonAsync(_userId, _itemId));

        await _comparisonService.RemoveFromComparisonAsync(_userId, _itemId);
        Assert.False(await _comparisonService.IsInComparisonAsync(_userId, _itemId));
    }

    [Fact]
    public async Task GetComparisonCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var items = Enumerable.Range(1, 3)
            .Select(i => new CatalogItemDto { Id = i, Name = $"Item {i}" })
            .ToList();

        foreach (var item in items)
        {
            _catalogServiceMock.Setup(x => x.GetCatalogItemAsync(item.Id))
                .ReturnsAsync(item);
            await _comparisonService.AddToComparisonAsync(_userId, item.Id);
        }

        // Act
        var count = await _comparisonService.GetComparisonCountAsync(_userId);

        // Assert
        Assert.Equal(items.Count, count);
    }
} 
