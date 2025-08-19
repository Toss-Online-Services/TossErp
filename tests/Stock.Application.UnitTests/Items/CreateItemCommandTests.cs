using TossErp.Stock.Application.Items.Commands.CreateItem;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Stock.Application.UnitTests.Items;

public class CreateItemCommandTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<CreateItemCommandHandler>> _mockLogger;
    private readonly CreateItemCommandHandler _handler;

    public CreateItemCommandTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<CreateItemCommandHandler>>();
        _handler = new CreateItemCommandHandler(_mockItemRepository.Object, _mockUnitOfWork.Object, _mockCurrentUserService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateItem()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            ItemCode = "TEST001",
            ItemName = "Test Item",
            ItemGroup = "Test Group",
            StockUOM = "PCS",
            ItemType = "Stock",
            ValuationMethod = "FIFO",
            Company = "Test Company",
            Description = "Test Description",
            StandardRate = 10.99m,
            ReOrderLevel = 10,
            ReOrderQty = 50
        };

        _mockCurrentUserService.Setup(x => x.CompanyId).Returns("Test Company");
        _mockItemRepository.Setup(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(null));
        _mockItemRepository.Setup(x => x.Add(It.IsAny<ItemAggregate>()));
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.ItemCode, result.ItemCode);
        _mockItemRepository.Verify(x => x.Add(It.IsAny<ItemAggregate>()), Times.Once());
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetCorrectProperties()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            ItemCode = "TEST001",
            ItemName = "Test Item",
            ItemGroup = "Test Group",
            StockUOM = "PCS",
            ItemType = "Stock",
            ValuationMethod = "FIFO",
            Company = "Test Company",
            Description = "Test Description",
            StandardRate = 10.99m,
            ReOrderLevel = 10,
            ReOrderQty = 50
        };

        _mockCurrentUserService.Setup(x => x.CompanyId).Returns("Test Company");
        _mockItemRepository.Setup(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(null));
        _mockItemRepository.Setup(x => x.Add(It.IsAny<ItemAggregate>()))
            .Callback<ItemAggregate>(item =>
            {
                Assert.Equal(command.ItemName, item.ItemName);
                Assert.Equal(command.ItemCode, item.ItemCode.Value);
                Assert.Equal(command.ItemGroup, item.ItemGroup);
                Assert.Equal(command.StockUOM, item.StockUOM.Name);
            });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockItemRepository.Verify(x => x.Add(It.IsAny<ItemAggregate>()), Times.Once());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldGenerateUniqueId()
    {
        // Arrange
        var command = new CreateItemCommand
        {
            ItemCode = "TEST001",
            ItemName = "Test Item",
            ItemGroup = "Test Group",
            StockUOM = "PCS",
            ItemType = "Stock",
            ValuationMethod = "FIFO",
            Company = "Test Company"
        };

        _mockCurrentUserService.Setup(x => x.CompanyId).Returns("Test Company");
        _mockItemRepository.Setup(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ItemAggregate?>(null));
        _mockItemRepository.Setup(x => x.Add(It.IsAny<ItemAggregate>()));
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.Id, result2.Id);
    }
}
