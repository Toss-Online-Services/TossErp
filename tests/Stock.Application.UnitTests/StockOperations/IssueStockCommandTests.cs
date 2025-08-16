using TossErp.Stock.Application.Commands.IssueStock;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Application.Common.Interfaces;
using Moq;
using MediatR;

namespace Stock.Application.UnitTests.StockOperations;

public class IssueStockCommandTests
{
    private readonly Mock<IStockLevelRepository> _mockStockLevelRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IssueStockCommandHandler _handler;

    public IssueStockCommandTests()
    {
        _mockStockLevelRepository = new Mock<IStockLevelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new IssueStockCommandHandler(_mockStockLevelRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldIssueStock()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var quantity = 10m;
        var currentStock = 50m;

        var stockLevel = new StockLevel(itemId, warehouseId, binId, currentStock, 0m);
        var command = new IssueStockCommand
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            Quantity = quantity,
            Reference = "SALE001",
            ReferenceType = "Sale"
        };

        _mockStockLevelRepository.Setup(x => x.GetByItemAndLocationAsync(itemId, warehouseId, binId))
            .ReturnsAsync(stockLevel);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(currentStock - quantity, stockLevel.QuantityOnHand);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var quantity = 60m;
        var currentStock = 50m;

        var stockLevel = new StockLevel(itemId, warehouseId, binId, currentStock, 0m);
        var command = new IssueStockCommand
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            Quantity = quantity,
            Reference = "SALE001",
            ReferenceType = "Sale"
        };

        _mockStockLevelRepository.Setup(x => x.GetByItemAndLocationAsync(itemId, warehouseId, binId))
            .ReturnsAsync(stockLevel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Equal(currentStock, stockLevel.QuantityOnHand); // Stock should remain unchanged
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StockLevelNotFound_ShouldReturnFalse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var quantity = 10m;

        var command = new IssueStockCommand
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            Quantity = quantity,
            Reference = "SALE001",
            ReferenceType = "Sale"
        };

        _mockStockLevelRepository.Setup(x => x.GetByItemAndLocationAsync(itemId, warehouseId, binId))
            .ReturnsAsync((StockLevel)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ZeroQuantity_ShouldReturnFalse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var quantity = 0m;

        var command = new IssueStockCommand
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            Quantity = quantity,
            Reference = "SALE001",
            ReferenceType = "Sale"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockStockLevelRepository.Verify(x => x.GetByItemAndLocationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
