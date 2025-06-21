using TossErp.Domain.SeedWork;
using TossErp.Domain.Events;

namespace TossErp.Domain.AggregatesModel.ProductAggregate;

public class Product : Entity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
    public string SKU { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal CostPrice { get; private set; }
    public int StockQuantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty; // e.g., kg, pieces, liters
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid? VendorId { get; private set; }

    private readonly List<ProductImage> _images;
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private readonly List<StockMovement> _stockMovements;
    public IReadOnlyCollection<StockMovement> StockMovements => _stockMovements.AsReadOnly();

    protected Product() 
    {
        _images = new List<ProductImage>();
        _stockMovements = new List<StockMovement>();
        Name = string.Empty;
        Description = string.Empty;
        Barcode = string.Empty;
        SKU = string.Empty;
        Category = string.Empty;
        Unit = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public Product(string name, string description, string barcode, string sku, 
                   decimal price, decimal costPrice, int stockQuantity, int minimumStockLevel,
                   string category, string unit, Guid businessId, Guid? vendorId = null) : this()
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Barcode = barcode;
        SKU = sku;
        Price = price;
        CostPrice = costPrice;
        StockQuantity = stockQuantity;
        MinimumStockLevel = minimumStockLevel;
        Category = category;
        Unit = unit;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        BusinessId = businessId;
        VendorId = vendorId;

        // TODO: Add domain event when ProductCreatedDomainEvent is created
        // AddDomainEvent(new ProductCreatedDomainEvent(this));
    }

    public void UpdateDetails(string name, string description, decimal price, decimal costPrice, 
                             string category, string unit)
    {
        Name = name;
        Description = description;
        Price = price;
        CostPrice = costPrice;
        Category = category;
        Unit = unit;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event when ProductUpdatedDomainEvent is created
        // AddDomainEvent(new ProductUpdatedDomainEvent(this));
    }

    public void UpdateStock(int quantity, string movementType, string reason = "")
    {
        var previousStock = StockQuantity;
        
        switch (movementType.ToLower())
        {
            case "in":
                StockQuantity += quantity;
                break;
            case "out":
                if (StockQuantity < quantity)
                    throw new InvalidOperationException("Insufficient stock for this operation");
                StockQuantity -= quantity;
                break;
            case "adjustment":
                StockQuantity = quantity;
                break;
            default:
                throw new ArgumentException("Invalid movement type");
        }

        var movement = new StockMovement(Id, quantity, movementType, reason, previousStock, StockQuantity);
        _stockMovements.Add(movement);

        // TODO: Add domain event when ProductStockUpdatedDomainEvent is created
        // AddDomainEvent(new ProductStockUpdatedDomainEvent(this, movement));

        if (StockQuantity <= MinimumStockLevel)
        {
            // TODO: Add domain event when ProductLowStockDomainEvent is created
            // AddDomainEvent(new ProductLowStockDomainEvent(this));
        }
    }

    public void SetMinimumStockLevel(int minimumLevel)
    {
        MinimumStockLevel = minimumLevel;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        // TODO: Add domain event when ProductDeactivatedDomainEvent is created
        // AddDomainEvent(new ProductDeactivatedDomainEvent(this));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        // TODO: Add domain event when ProductActivatedDomainEvent is created
        // AddDomainEvent(new ProductActivatedDomainEvent(this));
    }

    public bool IsLowStock() => StockQuantity <= MinimumStockLevel;

    public void AddImage(string imageUrl, string altText = "")
    {
        var image = new ProductImage(Id, imageUrl, altText);
        _images.Add(image);
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image != null)
        {
            _images.Remove(image);
        }
    }
} 