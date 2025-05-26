using System.Text.Json;
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgvector;
using Npgsql;
using Polly;

namespace Catalog.Infrastructure;

public class CatalogContextSeed : IDbSeeder<CatalogContext>
{
    private readonly IHostEnvironment _env;
    private readonly CatalogOptions _settings;
    private readonly ICatalogAI _catalogAI;
    private readonly ILogger<CatalogContextSeed> _logger;

    public CatalogContextSeed(
        IHostEnvironment env,
        IOptions<CatalogOptions> settings,
        ICatalogAI catalogAI,
        ILogger<CatalogContextSeed> logger)
    {
        _env = env;
        _settings = settings.Value;
        _catalogAI = catalogAI;
        _logger = logger;
    }

    public async Task SeedAsync(CatalogContext context)
    {
        var useCustomizationData = _settings.UseCustomizationData;
        var contentRootPath = _env.ContentRootPath;
        var webRootPath = Path.Combine(contentRootPath, "wwwroot");

        // Workaround from https://github.com/npgsql/efcore.pg/issues/292#issuecomment-388608426
        context.Database.OpenConnection();
        ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();

        if (!context.Products.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "catalog.json");
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<CatalogSourceEntry[]>(sourceJson) ?? Array.Empty<CatalogSourceEntry>();

            context.CatalogBrands.RemoveRange(context.CatalogBrands);
            await context.CatalogBrands.AddRangeAsync(sourceItems.Select(x => x.Brand).Distinct()
                .Select(brandName => new CatalogBrand { Brand = brandName }));
            _logger.LogInformation("Seeded catalog with {NumBrands} brands", context.CatalogBrands.Count());

            context.CatalogTypes.RemoveRange(context.CatalogTypes);
            await context.CatalogTypes.AddRangeAsync(sourceItems.Select(x => x.Type).Distinct()
                .Select(typeName => new CatalogType { Type = typeName }));
            _logger.LogInformation("Seeded catalog with {NumTypes} types", context.CatalogTypes.Count());

            await context.SaveChangesAsync();

            var brandIdsByName = await context.CatalogBrands.ToDictionaryAsync(x => x.Brand, x => x.Id);
            var typeIdsByName = await context.CatalogTypes.ToDictionaryAsync(x => x.Type, x => x.Id);

            var products = sourceItems.Select(source => new Product(source.Name, source.Description, source.Description, source.Price)
            {
                Id = source.Id,
                CatalogBrandId = brandIdsByName[source.Brand],
                CatalogTypeId = typeIdsByName[source.Type],
                AvailableStock = 100,
                MaxStockThreshold = 200,
                RestockThreshold = 10,
                PictureFileName = $"{source.Id}.webp",
            }).ToArray();

            if (_catalogAI.IsEnabled)
            {
                _logger.LogInformation("Generating {NumItems} embeddings", products.Length);
                IReadOnlyList<Vector> embeddings = await _catalogAI.GetEmbeddingsAsync(products);
                for (int i = 0; i < products.Length; i++)
                {
                    products[i].Embedding = embeddings[i];
                }
            }

            await context.Products.AddRangeAsync(products);
            _logger.LogInformation("Seeded catalog with {NumItems} items", context.Products.Count());
            await context.SaveChangesAsync();
        }
    }

    private class CatalogSourceEntry
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
    }

    public static async Task SeedAsync(CatalogContext context, IHostEnvironment env, ILogger<CatalogContext> logger)
    {
        var policy = CreatePolicy(logger, nameof(CatalogContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var contentRootPath = env.ContentRootPath;
            using (var connection = new NpgsqlConnection(context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
            }
        });
    }

    private static IAsyncPolicy CreatePolicy(ILogger logger, string prefix, int retries = 3)
    {
        return Policy.Handle<NpgsqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}
