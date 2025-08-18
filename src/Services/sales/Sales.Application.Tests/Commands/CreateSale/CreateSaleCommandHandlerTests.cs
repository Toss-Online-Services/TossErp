using Xunit;
using Moq;
using MediatR;
using TossErp.Sales.Application.Commands.CreateSale;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Application.Tests.Commands.CreateSale;

public class CreateSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<ITillRepository> _tillRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDomainEventService> _domainEventServiceMock;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _tillRepositoryMock = new Mock<ITillRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _domainEventServiceMock = new Mock<IDomainEventService>();
        _handler = new CreateSaleCommandHandler(
            _saleRepositoryMock.Object,
            _tillRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _domainEventServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateSale()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            TillId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = "Test Item",
                    ItemSku = "SKU001",
                    Quantity = 2,
                    UnitPrice = 10.50m,
                    TaxRate = 0.15m
                }
            },
            DiscountAmount = 5.00m,
            DiscountReason = "Test discount",
            Notes = "Test sale"
        };

        var till = Till.Create(command.TillId, "Test Till", "Test till description", "tenant");
        _tillRepositoryMock.Setup(x => x.GetByIdAsync(command.TillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(till);

        Sale? savedSale = null;
        _saleRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Callback<Sale, CancellationToken>((sale, _) => savedSale = sale)
            .ReturnsAsync((Sale sale, CancellationToken _) => sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.TillId, result.TillId);
        Assert.Equal(command.CustomerId, result.CustomerId);
        Assert.Equal(command.CustomerName, result.CustomerName);
        Assert.Equal(SaleStatus.Pending, result.Status);
        Assert.Single(result.Items);
        Assert.Equal(command.DiscountAmount, result.DiscountAmount);
        Assert.Equal(command.DiscountReason, result.DiscountReason);
        Assert.Equal(command.Notes, result.Notes);

        _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _domainEventServiceMock.Verify(x => x.PublishAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidTillId_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            TillId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            Items = new List<CreateSaleItemRequest>()
        };

        _tillRepositoryMock.Setup(x => x.GetByIdAsync(command.TillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Till?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Till not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WithClosedTill_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            TillId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            Items = new List<CreateSaleItemRequest>()
        };

        var till = Till.Create(command.TillId, "Test Till", "Test till description", "tenant");
        till.Close("Test close");
        _tillRepositoryMock.Setup(x => x.GetByIdAsync(command.TillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(till);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Till is not open", exception.Message);
    }

    [Fact]
    public async Task Handle_WithEmptyItems_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            TillId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            Items = new List<CreateSaleItemRequest>()
        };

        var till = Till.Create(command.TillId, "Test Till", "Test till description", "tenant");
        _tillRepositoryMock.Setup(x => x.GetByIdAsync(command.TillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(till);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Sale must have at least one item", exception.Message);
    }

    [Fact]
    public async Task Handle_WithNegativeDiscount_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            TillId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = "Test Item",
                    ItemSku = "SKU001",
                    Quantity = 1,
                    UnitPrice = 10.00m
                }
            },
            DiscountAmount = -5.00m
        };

        var till = Till.Create(command.TillId, "Test Till", "Test till description", "tenant");
        _tillRepositoryMock.Setup(x => x.GetByIdAsync(command.TillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(till);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Discount amount cannot be negative", exception.Message);
    }
}
