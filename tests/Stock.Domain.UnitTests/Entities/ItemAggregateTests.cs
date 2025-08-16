using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;

namespace Stock.Domain.UnitTests.Entities;

public class ItemAggregateTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateItem()
    {
        // Arrange & Act
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Test Item", item.Name);
        Assert.Equal("Test Description", item.Description);
        Assert.Equal("TEST001", item.ItemCode.Value);
        Assert.Equal(ItemType.Stock, item.ItemType);
        Assert.Equal("PCS", item.DefaultUnitOfMeasure);
        Assert.Equal(10.99m, item.DefaultPrice);
        Assert.Equal(10, item.ReorderLevel);
        Assert.Equal(50, item.ReorderQuantity);
        Assert.True(item.IsActive);
        Assert.Empty(item.DomainEvents);
    }

    [Fact]
    public void Constructor_InvalidItemCode_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode(""), // Invalid empty code
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        ));

        Assert.Contains("Item code cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_NegativePrice_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            -10.99m, // Negative price
            10,
            50
        ));

        Assert.Contains("Price cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_NegativeReorderLevel_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            -10, // Negative reorder level
            50
        ));

        Assert.Contains("Reorder level cannot be negative", exception.Message);
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateItem()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );

        // Act
        item.Update(
            "Updated Item",
            "Updated Description",
            ItemType.Service,
            "KG",
            20.99m,
            20,
            100
        );

        // Assert
        Assert.Equal("Updated Item", item.Name);
        Assert.Equal("Updated Description", item.Description);
        Assert.Equal(ItemType.Service, item.ItemType);
        Assert.Equal("KG", item.DefaultUnitOfMeasure);
        Assert.Equal(20.99m, item.DefaultPrice);
        Assert.Equal(20, item.ReorderLevel);
        Assert.Equal(100, item.ReorderQuantity);
        Assert.NotEmpty(item.DomainEvents);
    }

    [Fact]
    public void Update_DisabledItem_ShouldThrowException()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );
        item.Disable();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.Update(
            "Updated Item",
            "Updated Description",
            ItemType.Service,
            "KG",
            20.99m,
            20,
            100
        ));

        Assert.Contains("Cannot update disabled item", exception.Message);
    }

    [Fact]
    public void Disable_ActiveItem_ShouldDisableItem()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );

        // Act
        item.Disable();

        // Assert
        Assert.False(item.IsActive);
        Assert.NotEmpty(item.DomainEvents);
        var domainEvent = item.DomainEvents.First();
        Assert.IsType<ItemDisabledEvent>(domainEvent);
    }

    [Fact]
    public void Enable_DisabledItem_ShouldEnableItem()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );
        item.Disable();

        // Act
        item.Enable();

        // Assert
        Assert.True(item.IsActive);
        Assert.NotEmpty(item.DomainEvents);
        var domainEvent = item.DomainEvents.Last();
        Assert.IsType<ItemEnabledEvent>(domainEvent);
    }

    [Fact]
    public void AddVariant_ValidVariant_ShouldAddVariant()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );

        var variant = new ItemVariant(
            "Size",
            "Large",
            "L",
            15.99m
        );

        // Act
        item.AddVariant(variant);

        // Assert
        Assert.Contains(variant, item.Variants);
        Assert.NotEmpty(item.DomainEvents);
    }

    [Fact]
    public void AddVariant_DuplicateVariant_ShouldThrowException()
    {
        // Arrange
        var item = new ItemAggregate(
            "Test Item",
            "Test Description",
            new ItemCode("TEST001"),
            ItemType.Stock,
            "PCS",
            10.99m,
            10,
            50
        );

        var variant = new ItemVariant(
            "Size",
            "Large",
            "L",
            15.99m
        );

        item.AddVariant(variant);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.AddVariant(variant));
        Assert.Contains("Variant already exists", exception.Message);
    }
}
