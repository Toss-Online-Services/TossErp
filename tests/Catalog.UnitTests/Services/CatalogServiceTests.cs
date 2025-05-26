using AutoMapper;
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.DTOs;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Services;

public class CatalogServiceTests
{
    private readonly Mock<ICatalogRepository> _mockCatalogRepository;
    private readonly Mock<ICatalogAI> _mockCatalogAI;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ICatalogService _catalogService;

    public CatalogServiceTests()
    {
        _mockCatalogRepository = new Mock<ICatalogRepository>();
        _mockCatalogAI = new Mock<ICatalogAI>();
        _mockMapper = new Mock<IMapper>();
        _catalogService = new CatalogService(_mockCatalogRepository.Object, _mockCatalogAI.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetCatalogItemAsync_WhenItemExists_ReturnsMappedItem()
    {
        // Arrange
        var id = 1;
        var catalogItem = new CatalogItem(
            "Test Item",
            "Test Description",
            new Money(10.99m, "USD"),
            "test.jpg",
            1,
            1,
            10,
            5,
            20);

        _mockCatalogRepository.Setup(repo => repo.GetProductByIdAsync(id))
            .ReturnsAsync(catalogItem);

        var expectedDto = new CatalogItemDto { Id = id, Name = "Test Item" };
        _mockMapper.Setup(m => m.Map<CatalogItemDto>(catalogItem))
            .Returns(expectedDto);

        // Act
        var result = await _catalogService.GetCatalogItemAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Name, result.Name);
    }

    [Fact]
    public async Task GetCatalogItemsAsync_WithFilters_ReturnsFilteredItems()
    {
        // Arrange
        var items = new List<CatalogItem>
        {
            new CatalogItem("Item1", "Desc1", new Money(10m, "USD"), "img1.jpg", 1, 1, 10, 5, 20),
            new CatalogItem("Item2", "Desc2", new Money(20m, "USD"), "img2.jpg", 1, 2, 10, 5, 20),
            new CatalogItem("Item3", "Desc3", new Money(30m, "USD"), "img3.jpg", 2, 1, 10, 5, 20)
        };

        _mockCatalogRepository.Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(items);

        var expectedDtos = items.Select(i => new CatalogItemDto { Id = i.Id, Name = i.Name }).ToList();
        _mockMapper.Setup(m => m.Map<IEnumerable<CatalogItemDto>>(It.IsAny<IEnumerable<CatalogItem>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _catalogService.GetCatalogItemsAsync(0, 10, 1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count());
    }

    [Fact]
    public async Task GetCatalogItemsWithSemanticRelevanceAsync_WhenAIEnabled_ReturnsRelevantItems()
    {
        // Arrange
        _mockCatalogAI.Setup(ai => ai.IsEnabled).Returns(true);
        var items = new List<CatalogItem>
        {
            new CatalogItem("Item1", "Desc1", new Money(10m, "USD"), "img1.jpg", 1, 1, 10, 5, 20),
            new CatalogItem("Item2", "Desc2", new Money(20m, "USD"), "img2.jpg", 1, 2, 10, 5, 20)
        };

        _mockCatalogAI.Setup(ai => ai.SearchProductsAsync("test"))
            .ReturnsAsync(items);

        var expectedDtos = items.Select(i => new CatalogItemDto { Id = i.Id, Name = i.Name }).ToList();
        _mockMapper.Setup(m => m.Map<IEnumerable<CatalogItemDto>>(It.IsAny<IEnumerable<CatalogItem>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _catalogService.GetCatalogItemsWithSemanticRelevanceAsync(0, 10, "test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBrandsAsync_ReturnsBrandNames()
    {
        // Arrange
        var brands = new List<CatalogBrand>
        {
            new CatalogBrand("Brand1"),
            new CatalogBrand("Brand2")
        };

        _mockCatalogRepository.Setup(repo => repo.GetBrandsAsync())
            .ReturnsAsync(brands);

        // Act
        var result = await _catalogService.GetBrandsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains("Brand1", result);
        Assert.Contains("Brand2", result);
    }

    [Fact]
    public async Task GetCatalogItemAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = 999;
        _mockCatalogRepository.Setup(repo => repo.GetProductByIdAsync(id))
            .ReturnsAsync((CatalogItem)null);

        // Act
        var result = await _catalogService.GetCatalogItemAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCatalogItemsAsync_WithEmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        _mockCatalogRepository.Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(new List<CatalogItem>());

        var filter = new CatalogFilterDto();

        // Act
        var result = await _catalogService.GetCatalogItemsAsync(0, 10, filter);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCatalogItemsAsync_WithInvalidPagination_ReturnsEmptyList()
    {
        // Arrange
        var items = new List<CatalogItem>
        {
            new CatalogItem("Item1", "Desc1", new Money(10m, "USD"), "img1.jpg", 1, 1, 10, 5, 20)
        };

        _mockCatalogRepository.Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(items);

        var filter = new CatalogFilterDto();

        // Act
        var result = await _catalogService.GetCatalogItemsAsync(10, 10, filter);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCatalogItemsWithSemanticRelevanceAsync_WhenAIDisabled_ReturnsEmptyList()
    {
        // Arrange
        _mockCatalogAI.Setup(ai => ai.IsEnabled).Returns(false);

        // Act
        var result = await _catalogService.GetCatalogItemsWithSemanticRelevanceAsync(0, 10, "test");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateCatalogItemAsync_WithValidData_CreatesAndReturnsItem()
    {
        // Arrange
        var itemDto = new CatalogItemDto
        {
            Name = "Test Item",
            Description = "Test Description",
            Price = 10.99m,
            Currency = "USD",
            PictureUri = "test.jpg",
            CatalogTypeId = 1,
            CatalogBrandId = 1,
            AvailableStock = 10,
            RestockThreshold = 5,
            MaxStockThreshold = 20
        };

        var catalogItem = new CatalogItem(
            itemDto.Name,
            itemDto.Description,
            new Money(itemDto.Price, itemDto.Currency),
            itemDto.PictureUri,
            itemDto.CatalogTypeId,
            itemDto.CatalogBrandId,
            itemDto.AvailableStock,
            itemDto.RestockThreshold,
            itemDto.MaxStockThreshold);

        _mockCatalogRepository.Setup(repo => repo.AddProductAsync(It.IsAny<CatalogItem>()))
            .ReturnsAsync(catalogItem);

        _mockMapper.Setup(m => m.Map<CatalogItemDto>(catalogItem))
            .Returns(itemDto);

        // Act
        var result = await _catalogService.CreateCatalogItemAsync(itemDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemDto.Name, result.Name);
        Assert.Equal(itemDto.Price, result.Price);
    }

    [Fact]
    public async Task UpdateCatalogItemAsync_WhenItemDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = 999;
        var itemDto = new CatalogItemDto { Id = id, Name = "Test Item" };

        _mockCatalogRepository.Setup(repo => repo.GetProductByIdAsync(id))
            .ReturnsAsync((CatalogItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _catalogService.UpdateCatalogItemAsync(id, itemDto));
    }

    [Fact]
    public async Task DeleteCatalogItemAsync_WhenItemDoesNotExist_DoesNotThrow()
    {
        // Arrange
        var id = 999;
        _mockCatalogRepository.Setup(repo => repo.DeleteProductAsync(id))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await _catalogService.DeleteCatalogItemAsync(id);
        _mockCatalogRepository.Verify(repo => repo.DeleteProductAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetSimilarItemsAsync_WhenAIDisabled_ReturnsEmptyList()
    {
        // Arrange
        _mockCatalogAI.Setup(ai => ai.IsEnabled).Returns(false);

        // Act
        var result = await _catalogService.GetSimilarItemsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
} 
