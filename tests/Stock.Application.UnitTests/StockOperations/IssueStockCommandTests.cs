using TossErp.Stock.Application.Commands.IssueStock;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Stock.Application.UnitTests.StockOperations;

public class IssueStockCommandTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IStockLevelRepository> _mockStockLevelRepository;
    private readonly Mock<IStockMovementRepository> _mockStockMovementRepository;
    private readonly Mock<IWarehouseRepository> _mockWarehouseRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<IssueStockCommandHandler>> _mockLogger;
    private readonly IssueStockCommandHandler _handler;

    public IssueStockCommandTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockStockLevelRepository = new Mock<IStockLevelRepository>();
        _mockStockMovementRepository = new Mock<IStockMovementRepository>();
        _mockWarehouseRepository = new Mock<IWarehouseRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<IssueStockCommandHandler>>();
        _handler = new IssueStockCommandHandler(
            _mockItemRepository.Object, 
            _mockStockLevelRepository.Object, 
            _mockStockMovementRepository.Object,
            _mockWarehouseRepository.Object,
            _mockUnitOfWork.Object, 
            _mockCurrentUserService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldIssueStock()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var quantity = 10m;
        var currentStock = 50m;

        var item = new ItemAggregate(new ItemCode("TEST001"), "Test Item", "Test Group", new UnitOfMeasure("PCS", "Pieces"), ItemType.Stock, ValuationMethod.FIFO, "Test Company");
        var warehouse = new WarehouseAggregate(new WarehouseCode("WH001"), "Test Warehouse", "Test Location", "Test Company");
        var stockLevel = new StockLevel(itemId, warehouseId, Guid.NewGuid(), currentStock, 0m);

        var command = new IssueStockCommand
        {
            TenantId = Guid.NewGuid(),
            ItemId = itemId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Unit = "PCS",
            RefType = "Sale",
            RefId = "SALE001",
            Reason = "Test issue",
            CreatedBy = "testUser"
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(item));
        _mockWarehouseRepository.Setup(x => x.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<WarehouseAggregate?>(warehouse));
        _mockStockLevelRepository.Setup(x => x.GetByItemAndWarehouseAsync(itemId, warehouseId, null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<StockLevel?>(stockLevel));
        _mockStockLevelRepository.Setup(x => x.Update(It.IsAny<StockLevel>()));
        _mockStockMovementRepository.Setup(x => x.Add(It.IsAny<StockMovement>()));
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(currentStock - quantity, stockLevel.QuantityOnHand);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var quantity = 60m;
        var currentStock = 50m;

        var item = new ItemAggregate(new ItemCode("TEST001"), "Test Item", "Test Group", new UnitOfMeasure("PCS", "Pieces"), ItemType.Stock, ValuationMethod.FIFO, "Test Company");
        var warehouse = new WarehouseAggregate(new WarehouseCode("WH001"), "Test Warehouse", "Test Location", "Test Company");
        var stockLevel = new StockLevel(itemId, warehouseId, Guid.NewGuid(), currentStock, 0m);

        var command = new IssueStockCommand
        {
            TenantId = Guid.NewGuid(),
            ItemId = itemId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Unit = "PCS",
            RefType = "Sale",
            RefId = "SALE001",
            Reason = "Test issue",
            CreatedBy = "testUser"
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(item));
        _mockWarehouseRepository.Setup(x => x.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<WarehouseAggregate?>(warehouse));
        _mockStockLevelRepository.Setup(x => x.GetByItemAndWarehouseAsync(itemId, warehouseId, null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<StockLevel?>(stockLevel));

        // Act & Assert
        Exception? exception = null;
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        Assert.IsType<InsufficientStockException>(exception);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task Handle_StockLevelNotFound_ShouldThrowException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var quantity = 10m;

        var item = new ItemAggregate(new ItemCode("TEST001"), "Test Item", "Test Group", new UnitOfMeasure("PCS", "Pieces"), ItemType.Stock, ValuationMethod.FIFO, "Test Company");
        var warehouse = new WarehouseAggregate(new WarehouseCode("WH001"), "Test Warehouse", "Test Location", "Test Company");

        var command = new IssueStockCommand
        {
            TenantId = Guid.NewGuid(),
            ItemId = itemId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Unit = "PCS",
            RefType = "Sale",
            RefId = "SALE001",
            Reason = "Test issue",
            CreatedBy = "testUser"
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(item));
        _mockWarehouseRepository.Setup(x => x.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<WarehouseAggregate?>(warehouse));
        _mockStockLevelRepository.Setup(x => x.GetByItemAndWarehouseAsync(itemId, warehouseId, null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<StockLevel?>(null));

        // Act & Assert
        Exception? exception = null;
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        Assert.IsType<InsufficientStockException>(exception);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
    }
}
