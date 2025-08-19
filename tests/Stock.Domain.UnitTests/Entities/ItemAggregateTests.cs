using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.ValueObjects;

namespace Stock.Domain.UnitTests.Entities;

public class ItemAggregateTests
{
	[Fact]
	public void Constructor_ValidParameters_ShouldCreateItem()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");

		// Act
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);
		item.UpdatePricing(10.99m, null);
		item.UpdateInventorySettings(10, 50, null, null);

		// Assert
		Assert.NotEqual(Guid.Empty, item.Id);
		Assert.Equal("Test Item", item.ItemName);
		Assert.Equal("TEST001", item.ItemCode.Value);
		Assert.Equal(ItemType.Stock, item.ItemType);
		Assert.Equal("PCS", item.StockUOM.Code);
		Assert.Equal(10.99m, item.DefaultPrice);
		Assert.Equal(10, item.ReorderLevel);
		Assert.Equal(50, item.ReorderQuantity);
		Assert.True(item.IsActive);
	}

	[Fact]
	public void Constructor_InvalidItemCode_ShouldThrowException()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => new ItemAggregate(
			new ItemCode(""),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		));
		Assert.Contains("Item code", ex.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void UpdatePricing_NegativePrice_ShouldThrowException()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => item.UpdatePricing(-10.99m, null));
		Assert.Contains("Standard rate", ex.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void UpdateInventorySettings_NegativeReorderLevel_ShouldThrowException()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => item.UpdateInventorySettings(-10, 50, null, null));
		Assert.Contains("Reorder level", ex.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void Update_ValidParameters_ShouldUpdateItem()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);

		// Act
		item.Update("Updated Item", "Updated Description", ItemType.Service, "KG", 20.99m, 20, 100);

		// Assert
		Assert.Equal("Updated Item", item.ItemName);
		Assert.Equal("Updated Description", item.Description);
		Assert.Equal(ItemType.Service, item.ItemType);
		Assert.Equal(20.99m, item.DefaultPrice);
		Assert.Equal(20, item.ReorderLevel);
		Assert.Equal(100, item.ReorderQuantity);
	}

	[Fact]
	public void Disable_ActiveItem_ShouldDisableItem()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);

		// Act
		item.Disable();

		// Assert
		Assert.False(item.IsActive);
		Assert.True(item.DomainEvents.Any(e => e is ItemDisabledEvent));
	}

	[Fact]
	public void Enable_DisabledItem_ShouldEnableItem()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);
		item.Disable();

		// Act
		item.Enable();

		// Assert
		Assert.True(item.IsActive);
		Assert.True(item.DomainEvents.Any(e => e is ItemEnabledEvent));
	}

	[Fact]
	public void AddVariant_ValidVariant_ShouldAddVariant()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);
		var variant = new ItemVariant("Size", "Large", "L", 15.99m);

		// Act
		item.AddVariant(variant);

		// Assert
		Assert.Contains(variant, item.Variants);
		Assert.True(item.DomainEvents.Any(e => e is ItemVariantAddedEvent));
	}

	[Fact]
	public void AddVariant_DuplicateVariant_ShouldThrowException()
	{
		// Arrange
		var uom = new UnitOfMeasure("PCS", "Pieces");
		var item = new ItemAggregate(
			new ItemCode("TEST001"),
			"Test Item",
			"Default",
			uom,
			ItemType.Stock,
			ValuationMethod.FIFO,
			"ACME"
		);
		var variant = new ItemVariant("Size", "Large", "L", 15.99m);
		item.AddVariant(variant);

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => item.AddVariant(variant));
		Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
	}
}
