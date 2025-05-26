using Catalog.Domain.SeedWork;
using Catalog.Domain.ValueObjects;
using System.Numerics;
using Pgvector;

namespace Catalog.Domain.AggregatesModel.CatalogAggregate;

public class CatalogItem : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public string PictureUri { get; private set; }
    public int CatalogTypeId { get; private set; }
    public CatalogType CatalogType { get; private set; }
    public int CatalogBrandId { get; private set; }
    public CatalogBrand CatalogBrand { get; private set; }
    public int AvailableStock { get; private set; }
    public int RestockThreshold { get; private set; }
    public int MaxStockThreshold { get; private set; }
    public bool OnReorder { get; private set; }
    public Vector? Embedding { get; private set; }

    protected CatalogItem() { }

    public CatalogItem(
        string name,
        string description,
        Money price,
        string pictureUri,
        int catalogTypeId,
        int catalogBrandId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold)
    {
        Name = name;
        Description = description;
        Price = price;
        PictureUri = pictureUri;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        AvailableStock = availableStock;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        OnReorder = false;
    }

    public void UpdateDetails(
        string name,
        string description,
        Money price,
        string pictureUri,
        int catalogTypeId,
        int catalogBrandId)
    {
        Name = name;
        Description = description;
        Price = price;
        PictureUri = pictureUri;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
    }

    public void UpdateStock(int availableStock)
    {
        AvailableStock = availableStock;
        OnReorder = availableStock < RestockThreshold;
    }

    public void UpdateEmbedding(Vector embedding)
    {
        Embedding = embedding;
    }
} 
