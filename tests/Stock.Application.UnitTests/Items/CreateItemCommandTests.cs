using TossErp.Stock.Application.Items.Commands.CreateItem;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Application.Common.Interfaces;
using Moq;
using MediatR;

namespace Stock.Application.UnitTests.Items;

public class CreateItemCommandTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateItemCommandHandler _handler;

    public CreateItemCommandTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateItemCommandHandler(_mockItemRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateItem()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            Name = "Test Item",
            Description = "Test Description",
            ItemCode = "TEST001",
            ItemType = ItemType.Stock,
            UnitOfMeasure = "PCS",
            DefaultPrice = 10.99m,
            ReorderLevel = 10,
            ReorderQuantity = 50
        };

        _mockItemRepository.Setup(x => x.AddAsync(It.IsAny<ItemAggregate>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockItemRepository.Verify(x => x.AddAsync(It.IsAny<ItemAggregate>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetCorrectProperties()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            Name = "Test Item",
            Description = "Test Description",
            ItemCode = "TEST001",
            ItemType = ItemType.Stock,
            UnitOfMeasure = "PCS",
            DefaultPrice = 10.99m,
            ReorderLevel = 10,
            ReorderQuantity = 50
        };

        _mockItemRepository.Setup(x => x.AddAsync(It.IsAny<ItemAggregate>()))
            .Callback<ItemAggregate>(item =>
            {
                Assert.Equal(command.Name, item.Name);
                Assert.Equal(command.Description, item.Description);
                Assert.Equal(command.ItemCode, item.ItemCode.Value);
                Assert.Equal(command.ItemType, item.ItemType);
                Assert.Equal(command.UnitOfMeasure, item.DefaultUnitOfMeasure);
                Assert.Equal(command.DefaultPrice, item.DefaultPrice);
                Assert.Equal(command.ReorderLevel, item.ReorderLevel);
                Assert.Equal(command.ReorderQuantity, item.ReorderQuantity);
            })
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockItemRepository.Verify(x => x.AddAsync(It.IsAny<ItemAggregate>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldGenerateUniqueId()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            Name = "Test Item",
            Description = "Test Description",
            ItemCode = "TEST001",
            ItemType = ItemType.Stock,
            UnitOfMeasure = "PCS"
        };

        _mockItemRepository.Setup(x => x.AddAsync(It.IsAny<ItemAggregate>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result1);
        Assert.NotEqual(Guid.Empty, result2);
        Assert.NotEqual(result1, result2);
    }
}
