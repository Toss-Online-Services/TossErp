namespace TossErp.Assets.Infrastructure.Services;

/// <summary>
/// Azure Blob Storage implementation for file storage
/// </summary>
public class BlobFileStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobFileStorageService> _logger;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly string _containerName;

    public BlobFileStorageService(
        BlobServiceClient blobServiceClient,
        ILogger<BlobFileStorageService> logger,
        ICurrentTenantService currentTenantService,
        IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _currentTenantService = currentTenantService;
        _containerName = configuration.GetValue<string>("Storage:AssetDocuments:ContainerName") ?? "asset-documents";
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = _currentTenantService.TenantId;
            var blobName = GenerateBlobName(tenantId, fileName);
            
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            var metadata = new Dictionary<string, string>
            {
                ["TenantId"] = tenantId.ToString(),
                ["OriginalFileName"] = fileName,
                ["UploadDate"] = DateTime.UtcNow.ToString("O"),
                ["UploadedBy"] = _currentTenantService.UserId ?? "System"
            };

            var response = await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders,
                    Metadata = metadata,
                    Conditions = new BlobRequestConditions()
                },
                cancellationToken);

            _logger.LogInformation("File uploaded successfully: {BlobName}", blobName);
            return blobName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw new FileStorageException($"Failed to upload file '{fileName}'", ex);
        }
    }

    public async Task<Stream> DownloadFileAsync(
        string filePath, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(filePath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Verify tenant access
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.Metadata.TryGetValue("TenantId", out var tenantId) &&
                Guid.Parse(tenantId) != _currentTenantService.TenantId)
            {
                throw new UnauthorizedAccessException("Access denied to file from different tenant");
            }

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("File downloaded successfully: {FilePath}", filePath);
            
            return response.Value.Content;
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Error downloading file {FilePath}", filePath);
            throw new FileStorageException($"Failed to download file '{filePath}'", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(
        string filePath, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(filePath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return false;
            }

            // Verify tenant access
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.Metadata.TryGetValue("TenantId", out var tenantId) &&
                Guid.Parse(tenantId) != _currentTenantService.TenantId)
            {
                throw new UnauthorizedAccessException("Access denied to file from different tenant");
            }

            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            
            if (response.Value)
            {
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            }
            
            return response.Value;
        }
        catch (Exception ex) when (!(ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            throw new FileStorageException($"Failed to delete file '{filePath}'", ex);
        }
    }

    public async Task<bool> FileExistsAsync(
        string filePath, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(filePath);

            var exists = await blobClient.ExistsAsync(cancellationToken);
            
            if (exists.Value)
            {
                // Verify tenant access
                var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                if (properties.Value.Metadata.TryGetValue("TenantId", out var tenantId) &&
                    Guid.Parse(tenantId) != _currentTenantService.TenantId)
                {
                    return false; // File exists but not accessible to current tenant
                }
            }

            return exists.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence {FilePath}", filePath);
            return false;
        }
    }

    public async Task<long> GetFileSizeAsync(
        string filePath, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(filePath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Verify tenant access
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.Metadata.TryGetValue("TenantId", out var tenantId) &&
                Guid.Parse(tenantId) != _currentTenantService.TenantId)
            {
                throw new UnauthorizedAccessException("Access denied to file from different tenant");
            }

            return properties.Value.ContentLength;
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Error getting file size {FilePath}", filePath);
            throw new FileStorageException($"Failed to get file size for '{filePath}'", ex);
        }
    }

    public async Task<string> GetFileUrlAsync(
        string filePath, 
        TimeSpan? expiry = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(cancellationToken);
            var blobClient = containerClient.GetBlobClient(filePath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Verify tenant access
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.Metadata.TryGetValue("TenantId", out var tenantId) &&
                Guid.Parse(tenantId) != _currentTenantService.TenantId)
            {
                throw new UnauthorizedAccessException("Access denied to file from different tenant");
            }

            var expiryTime = expiry ?? TimeSpan.FromHours(24);
            
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = filePath,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUrl = blobClient.GenerateSasUri(sasBuilder);
                return sasUrl.ToString();
            }

            // Fallback to regular URL (not recommended for production)
            return blobClient.Uri.ToString();
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Error generating file URL {FilePath}", filePath);
            throw new FileStorageException($"Failed to generate URL for file '{filePath}'", ex);
        }
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        return containerClient;
    }

    private string GenerateBlobName(Guid tenantId, string fileName)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var sanitizedFileName = SanitizeFileName(fileName);
        
        return $"{tenantId}/{timestamp}/{uniqueId}_{sanitizedFileName}";
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}

/// <summary>
/// Custom exception for file storage operations
/// </summary>
public class FileStorageException : Exception
{
    public FileStorageException(string message) : base(message) { }
    public FileStorageException(string message, Exception innerException) : base(message, innerException) { }
}
