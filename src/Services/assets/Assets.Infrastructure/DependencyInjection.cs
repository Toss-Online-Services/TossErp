namespace TossErp.Assets.Infrastructure;

/// <summary>
/// Dependency injection configuration for Assets Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAssetsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        // Database Context with EF Core 9 optimizations
        services.AddDbContext<AssetsDbContext>((serviceProvider, options) =>
        {
            var currentTenantService = serviceProvider.GetRequiredService<ICurrentTenantService>();
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(120);
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // EF Core 9 performance optimizations
            options.EnableServiceProviderCaching()
                   .EnableSensitiveDataLogging(false)
                   .LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);

            // Use scoped DbContext factory for multi-tenancy
            options.UseApplicationServiceProvider(serviceProvider);
        });

        // Configure DbContext factory for advanced scenarios
        services.AddDbContextFactory<AssetsDbContext>((serviceProvider, options) =>
        {
            var currentTenantService = serviceProvider.GetRequiredService<ICurrentTenantService>();
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.CommandTimeout(120);
            });
        }, ServiceLifetime.Scoped);

        // Repository Pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // File Storage Services
        AddFileStorageServices(services, configuration);

        // Background Services
        services.AddHostedService<AssetMaintenanceReminderService>();
        services.AddHostedService<AssetDepreciationCalculationService>();

        // Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<AssetsDbContext>("assets-database")
            .AddAzureBlobStorage(
                configuration.GetConnectionString("StorageAccount")!,
                "asset-documents",
                "asset-storage");

        return services;
    }

    private static void AddFileStorageServices(IServiceCollection services, IConfiguration configuration)
    {
        var storageType = configuration.GetValue<string>("Storage:Type")?.ToLowerInvariant() ?? "blob";

        switch (storageType)
        {
            case "blob":
            case "azure":
                var connectionString = configuration.GetConnectionString("StorageAccount");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    services.AddSingleton(serviceProvider =>
                        new BlobServiceClient(connectionString));
                    
                    services.AddScoped<IFileStorageService, BlobFileStorageService>();
                }
                else
                {
                    // Fallback to local file storage in development
                    services.AddScoped<IFileStorageService, LocalFileStorageService>();
                }
                break;

            case "local":
            case "file":
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported storage type: {storageType}");
        }
    }

    public static async Task<IServiceScope> MigrateAssetsDatabase(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AssetsDbContext>();
            await context.Database.MigrateAsync();
            
            // Seed initial data if needed
            await SeedInitialData(context);
            
            return scope;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    private static async Task SeedInitialData(AssetsDbContext context)
    {
        // Check if data already exists
        if (await context.AssetCategories.AnyAsync())
            return;

        // Seed default categories
        var categories = new[]
        {
            new AssetCategory 
            { 
                Id = Guid.NewGuid(),
                TenantId = Guid.Empty, // Global category
                Name = "Computer Equipment",
                Code = "COMP",
                Description = "Computers, laptops, servers, and related equipment",
                Color = "#007ACC",
                Icon = "computer",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new AssetCategory 
            { 
                Id = Guid.NewGuid(),
                TenantId = Guid.Empty,
                Name = "Office Furniture",
                Code = "FURN",
                Description = "Desks, chairs, cabinets, and office furniture",
                Color = "#8B4513",
                Icon = "chair",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new AssetCategory 
            { 
                Id = Guid.NewGuid(),
                TenantId = Guid.Empty,
                Name = "Vehicles",
                Code = "VHCL",
                Description = "Company vehicles and transportation equipment",
                Color = "#FF6B35",
                Icon = "car",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.AssetCategories.AddRange(categories);
        await context.SaveChangesAsync();
    }
}

/// <summary>
/// Local file storage implementation for development/testing
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly string _basePath;

    public LocalFileStorageService(
        ILogger<LocalFileStorageService> logger,
        ICurrentTenantService currentTenantService,
        IConfiguration configuration)
    {
        _logger = logger;
        _currentTenantService = currentTenantService;
        _basePath = configuration.GetValue<string>("Storage:Local:BasePath") ?? 
                   Path.Combine(Environment.CurrentDirectory, "uploads", "assets");
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId;
        var relativePath = GenerateFilePath(tenantId, fileName);
        var fullPath = Path.Combine(_basePath, relativePath);
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        
        using var fileWriteStream = File.Create(fullPath);
        await fileStream.CopyToAsync(fileWriteStream, cancellationToken);
        
        _logger.LogInformation("File uploaded locally: {FilePath}", relativePath);
        return relativePath;
    }

    public async Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");
        
        return File.OpenRead(fullPath);
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<long> GetFileSizeAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");
        
        var fileInfo = new FileInfo(fullPath);
        return Task.FromResult(fileInfo.Length);
    }

    public Task<string> GetFileUrlAsync(string filePath, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        // For local storage, return a relative path that can be served by the web server
        return Task.FromResult($"/api/assets/files/{filePath}");
    }

    private string GenerateFilePath(Guid tenantId, string fileName)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var sanitizedFileName = SanitizeFileName(fileName);
        
        return Path.Combine(tenantId.ToString(), timestamp, $"{uniqueId}_{sanitizedFileName}");
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}
