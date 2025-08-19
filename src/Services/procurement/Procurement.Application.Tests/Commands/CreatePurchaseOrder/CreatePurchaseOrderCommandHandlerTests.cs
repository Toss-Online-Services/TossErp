using Xunit;
using Moq;
using MediatR;
using TossErp.Procurement.Application.Commands.CreatePurchaseOrder;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Application.Tests.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreatePurchaseOrder()
    {
        // Arrange
        var mockSupplierRepository = new Mock<ISupplierRepository>();
        var mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDomainEventService = new Mock<IDomainEventService>();

        var supplierId = Guid.NewGuid();
        var supplier = Supplier.Create("SUP001", "Test Supplier", "test-tenant");
        mockSupplierRepository.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = supplierId,
            SupplierName = "Test Supplier",
            Items = new List<CreatePurchaseOrderItemRequest>
            {
                new()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = "Test Item",
                    ItemSku = "SKU001",
                    Quantity = 10,
                    UnitPrice = 25.50m,
                    TaxRate = 0.15m
                }
            },
            Notes = "Test purchase order"
        };

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(s => s.TenantId).Returns("test-tenant");
        mockCurrentUserService.Setup(s => s.UserName).Returns("test-user");

        var handler = new CreatePurchaseOrderCommandHandler(
            mockPurchaseOrderRepository.Object,
            mockSupplierRepository.Object,
            mockUnitOfWork.Object,
            mockDomainEventService.Object,
            mockCurrentUserService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplierId, result.SupplierId);
        Assert.Equal("Test Supplier", result.SupplierName);
        Assert.Single(result.Items);
        Assert.Equal("Test Item", result.Items.First().ItemName);
        Assert.Equal(PurchaseOrderStatus.Draft, result.Status);

        mockPurchaseOrderRepository.Verify(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDomainEventService.Verify(d => d.PublishAsync(It.IsAny<IEnumerable<TossErp.Procurement.Domain.Events.IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidSupplierId_ShouldThrowException()
    {
        // Arrange
        var mockSupplierRepository = new Mock<ISupplierRepository>();
        var mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDomainEventService = new Mock<IDomainEventService>();

        var supplierId = Guid.NewGuid();
        mockSupplierRepository.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = supplierId,
            SupplierName = "Test Supplier",
            Items = new List<CreatePurchaseOrderItemRequest>()
        };

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(s => s.TenantId).Returns("test-tenant");
        mockCurrentUserService.Setup(s => s.UserName).Returns("test-user");

        var handler = new CreatePurchaseOrderCommandHandler(
            mockPurchaseOrderRepository.Object,
            mockSupplierRepository.Object,
            mockUnitOfWork.Object,
            mockDomainEventService.Object,
            mockCurrentUserService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyItems_ShouldCreatePurchaseOrder()
    {
        // Arrange
        var mockSupplierRepository = new Mock<ISupplierRepository>();
        var mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDomainEventService = new Mock<IDomainEventService>();

        var supplierId = Guid.NewGuid();
        var supplier = Supplier.Create("SUP001", "Test Supplier", "test-tenant");
        mockSupplierRepository.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = supplierId,
            SupplierName = "Test Supplier",
            Items = new List<CreatePurchaseOrderItemRequest>() // Empty items
        };

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(s => s.TenantId).Returns("test-tenant");
        mockCurrentUserService.Setup(s => s.UserName).Returns("test-user");

        var handler = new CreatePurchaseOrderCommandHandler(
            mockPurchaseOrderRepository.Object,
            mockSupplierRepository.Object,
            mockUnitOfWork.Object,
            mockDomainEventService.Object,
            mockCurrentUserService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplierId, result.SupplierId);
        Assert.Equal("Test Supplier", result.SupplierName);
        Assert.Empty(result.Items);
        Assert.Equal(PurchaseOrderStatus.Draft, result.Status);

        mockPurchaseOrderRepository.Verify(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDomainEventService.Verify(d => d.PublishAsync(It.IsAny<IEnumerable<TossErp.Procurement.Domain.Events.IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidItemData_ShouldThrowException()
    {
        // Arrange
        var mockSupplierRepository = new Mock<ISupplierRepository>();
        var mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDomainEventService = new Mock<IDomainEventService>();

        var supplierId = Guid.NewGuid();
        var supplier = Supplier.Create("SUP001", "Test Supplier", "test-tenant");
        mockSupplierRepository.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = supplierId,
            SupplierName = "Test Supplier",
            Items = new List<CreatePurchaseOrderItemRequest>
            {
                new()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = "Test Item",
                    ItemSku = "SKU001",
                    Quantity = 0, // Invalid quantity
                    UnitPrice = 25.50m,
                    TaxRate = 0.15m
                }
            }
        };

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(s => s.TenantId).Returns("test-tenant");
        mockCurrentUserService.Setup(s => s.UserName).Returns("test-user");

        var handler = new CreatePurchaseOrderCommandHandler(
            mockPurchaseOrderRepository.Object,
            mockSupplierRepository.Object,
            mockUnitOfWork.Object,
            mockDomainEventService.Object,
            mockCurrentUserService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            handler.Handle(command, CancellationToken.None));
    }
}
