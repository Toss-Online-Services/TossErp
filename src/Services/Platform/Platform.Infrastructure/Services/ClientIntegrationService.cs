using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TossErp.Platform.Domain.Entities;
using TossErp.Platform.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace TossErp.Platform.Infrastructure.Services;

/// <summary>
/// Enterprise client integration service for Service-as-Software platform
/// </summary>
public class ClientIntegrationService : IClientIntegrationService
{
    private readonly ILogger<ClientIntegrationService> _logger;
    private readonly IClientRepository _clientRepository;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IAPIKeyService _apiKeyService;
    private readonly IWebhookService _webhookService;
    private readonly IRateLimitingService _rateLimitingService;
    private readonly ISecurityService _securityService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IMemoryCache _cache;
    private readonly Timer _healthCheckTimer;
    private readonly Timer _metricsCollectionTimer;

    public ClientIntegrationService(
        ILogger<ClientIntegrationService> logger,
        IClientRepository clientRepository,
        IIntegrationRepository integrationRepository,
        IAPIKeyService apiKeyService,
        IWebhookService webhookService,
        IRateLimitingService rateLimitingService,
        ISecurityService securityService,
        IAnalyticsService analyticsService,
        IMemoryCache cache)
    {
        _logger = logger;
        _clientRepository = clientRepository;
        _integrationRepository = integrationRepository;
        _apiKeyService = apiKeyService;
        _webhookService = webhookService;
        _rateLimitingService = rateLimitingService;
        _securityService = securityService;
        _analyticsService = analyticsService;
        _cache = cache;

        // Initialize background tasks
        _healthCheckTimer = new Timer(PerformHealthChecks, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        _metricsCollectionTimer = new Timer(CollectMetrics, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task<ClientRegistrationResult> RegisterClientAsync(
        string tenantId,
        ClientRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var clientId = Guid.NewGuid().ToString();
        _logger.LogInformation("Registering client {ClientId} for tenant {TenantId} with type {ClientType}",
            clientId, tenantId, request.ClientType);

        try
        {
            // Validate tenant limits
            await ValidateTenantLimitsAsync(tenantId, cancellationToken);

            // Create client configuration
            var client = new ClientConfiguration
            {
                Id = clientId,
                TenantId = tenantId,
                Name = request.Name,
                Description = request.Description,
                ClientType = request.ClientType,
                Version = request.Version ?? "1.0.0",
                Environment = request.Environment,
                Status = ClientStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                Configuration = new ClientConfigurationSettings
                {
                    AllowedOrigins = request.AllowedOrigins ?? new List<string>(),
                    RedirectUris = request.RedirectUris ?? new List<string>(),
                    Scopes = request.Scopes ?? new List<string> { "read", "write" },
                    TokenLifetime = request.TokenLifetime ?? TimeSpan.FromHours(1),
                    RefreshTokenLifetime = request.RefreshTokenLifetime ?? TimeSpan.FromDays(30),
                    RequireProofKey = request.RequireProofKey ?? true,
                    RequireConsent = request.RequireConsent ?? false,
                    AllowOfflineAccess = request.AllowOfflineAccess ?? false,
                    EnableLogging = request.EnableLogging ?? true,
                    RateLimits = request.RateLimits ?? GetDefaultRateLimits(request.ClientType),
                    WebhookSettings = request.WebhookSettings,
                    CustomSettings = request.CustomSettings ?? new Dictionary<string, object>()
                },
                Endpoints = GenerateClientEndpoints(clientId, request.ClientType),
                SecuritySettings = new ClientSecuritySettings
                {
                    RequireClientSecret = ShouldRequireClientSecret(request.ClientType),
                    AllowedGrantTypes = GetAllowedGrantTypes(request.ClientType),
                    RequireHttps = request.Environment != ClientEnvironment.Development,
                    EnableCors = request.ClientType == ClientType.SPA,
                    IPWhitelist = request.IPWhitelist ?? new List<string>(),
                    AllowedCertificateThumbprints = request.AllowedCertificateThumbprints ?? new List<string>()
                }
            };

            // Generate API credentials
            var apiKey = await _apiKeyService.GenerateAPIKeyAsync(clientId, client.Configuration.Scopes, cancellationToken);
            client.Credentials = new ClientCredentials
            {
                ClientId = clientId,
                ApiKey = apiKey.Key,
                SecretHash = await _securityService.HashSecretAsync(apiKey.Secret),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = apiKey.ExpiresAt
            };

            // Save client configuration
            await _clientRepository.SaveClientAsync(client, cancellationToken);

            // Create integration record
            var integration = new ClientIntegration
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                TenantId = tenantId,
                IntegrationType = DetermineIntegrationType(request.ClientType),
                Status = IntegrationStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Configuration = client.Configuration,
                Metrics = new IntegrationMetrics
                {
                    TotalRequests = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 0,
                    AverageResponseTime = 0,
                    LastRequestAt = null,
                    DataVolume = 0
                }
            };

            await _integrationRepository.SaveIntegrationAsync(integration, cancellationToken);

            // Setup webhooks if configured
            if (client.Configuration.WebhookSettings != null)
            {
                await _webhookService.ConfigureWebhooksAsync(clientId, client.Configuration.WebhookSettings, cancellationToken);
            }

            _logger.LogInformation("Client {ClientId} registered successfully for tenant {TenantId}", clientId, tenantId);

            return new ClientRegistrationResult
            {
                ClientId = clientId,
                ApiKey = apiKey.Key,
                Secret = apiKey.Secret,
                Endpoints = client.Endpoints,
                Configuration = client.Configuration,
                Documentation = GenerateClientDocumentation(client),
                SDKInfo = GetSDKInformation(request.ClientType),
                ExampleCode = GenerateExampleCode(client, request.ClientType),
                Success = true,
                Message = "Client registered successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register client for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<APIResponse<T>> ProcessAPIRequestAsync<T>(
        string clientId,
        APIRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        _logger.LogDebug("Processing API request {RequestId} from client {ClientId} to endpoint {Endpoint}",
            requestId, clientId, request.Endpoint);

        try
        {
            // Get client configuration
            var client = await GetClientConfigurationAsync(clientId, cancellationToken);
            if (client == null)
            {
                return new APIResponse<T>
                {
                    Success = false,
                    ErrorCode = "CLIENT_NOT_FOUND",
                    Message = "Client configuration not found",
                    RequestId = requestId
                };
            }

            // Validate client status
            if (client.Status != ClientStatus.Active)
            {
                return new APIResponse<T>
                {
                    Success = false,
                    ErrorCode = "CLIENT_INACTIVE",
                    Message = "Client is not active",
                    RequestId = requestId
                };
            }

            // Rate limiting
            var rateLimitResult = await _rateLimitingService.CheckRateLimitAsync(
                clientId, 
                request.Endpoint, 
                client.Configuration.RateLimits,
                cancellationToken);

            if (!rateLimitResult.Allowed)
            {
                await LogAPIRequestAsync(clientId, request, false, "Rate limit exceeded", startTime, cancellationToken);
                return new APIResponse<T>
                {
                    Success = false,
                    ErrorCode = "RATE_LIMIT_EXCEEDED",
                    Message = $"Rate limit exceeded. Try again in {rateLimitResult.RetryAfter} seconds",
                    RequestId = requestId,
                    RateLimitInfo = rateLimitResult
                };
            }

            // Security validation
            var securityResult = await _securityService.ValidateRequestAsync(client, request, cancellationToken);
            if (!securityResult.IsValid)
            {
                await LogAPIRequestAsync(clientId, request, false, securityResult.Reason, startTime, cancellationToken);
                return new APIResponse<T>
                {
                    Success = false,
                    ErrorCode = "SECURITY_VALIDATION_FAILED",
                    Message = securityResult.Reason,
                    RequestId = requestId
                };
            }

            // Process request based on endpoint
            var response = await ProcessRequestByEndpointAsync<T>(client, request, requestId, cancellationToken);

            // Log successful request
            await LogAPIRequestAsync(clientId, request, response.Success, response.Message, startTime, cancellationToken);

            // Update metrics
            await UpdateClientMetricsAsync(clientId, request, response, startTime, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process API request {RequestId} from client {ClientId}", requestId, clientId);
            
            await LogAPIRequestAsync(clientId, request, false, ex.Message, startTime, cancellationToken);
            
            return new APIResponse<T>
            {
                Success = false,
                ErrorCode = "INTERNAL_ERROR",
                Message = "An internal error occurred",
                RequestId = requestId
            };
        }
    }

    public async Task<List<IntegrationHealth>> GetIntegrationHealthAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting integration health for tenant {TenantId}", tenantId);

        try
        {
            var integrations = await _integrationRepository.GetTenantIntegrationsAsync(tenantId, cancellationToken);
            var healthResults = new List<IntegrationHealth>();

            foreach (var integration in integrations)
            {
                var health = await AssessIntegrationHealthAsync(integration, cancellationToken);
                healthResults.Add(health);
            }

            return healthResults.OrderBy(h => h.HealthScore).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get integration health for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<IntegrationMetrics> GetIntegrationMetricsAsync(
        string clientId,
        TimeSpan period,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting integration metrics for client {ClientId}, period {Period}", clientId, period);

        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.Subtract(period);

            var metrics = await _analyticsService.GetClientMetricsAsync(clientId, startDate, endDate, cancellationToken);
            
            return new IntegrationMetrics
            {
                Period = period,
                StartDate = startDate,
                EndDate = endDate,
                TotalRequests = metrics.TotalRequests,
                SuccessfulRequests = metrics.SuccessfulRequests,
                FailedRequests = metrics.FailedRequests,
                AverageResponseTime = metrics.AverageResponseTime,
                MedianResponseTime = metrics.MedianResponseTime,
                P95ResponseTime = metrics.P95ResponseTime,
                P99ResponseTime = metrics.P99ResponseTime,
                ErrorRate = metrics.ErrorRate,
                ThroughputPerSecond = metrics.ThroughputPerSecond,
                DataVolumeIn = metrics.DataVolumeIn,
                DataVolumeOut = metrics.DataVolumeOut,
                TopEndpoints = metrics.TopEndpoints,
                ErrorBreakdown = metrics.ErrorBreakdown,
                ResponseTimeBreakdown = metrics.ResponseTimeBreakdown,
                GeographicDistribution = metrics.GeographicDistribution
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get integration metrics for client {ClientId}", clientId);
            throw;
        }
    }

    public async Task<bool> UpdateClientConfigurationAsync(
        string clientId,
        ClientConfigurationUpdate update,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating client configuration for {ClientId}", clientId);

        try
        {
            var client = await _clientRepository.GetClientAsync(clientId, cancellationToken);
            if (client == null)
            {
                _logger.LogWarning("Client {ClientId} not found", clientId);
                return false;
            }

            // Update configuration
            if (update.AllowedOrigins != null)
            {
                client.Configuration.AllowedOrigins = update.AllowedOrigins;
            }

            if (update.RedirectUris != null)
            {
                client.Configuration.RedirectUris = update.RedirectUris;
            }

            if (update.Scopes != null)
            {
                client.Configuration.Scopes = update.Scopes;
            }

            if (update.RateLimits != null)
            {
                client.Configuration.RateLimits = update.RateLimits;
            }

            if (update.WebhookSettings != null)
            {
                client.Configuration.WebhookSettings = update.WebhookSettings;
                await _webhookService.ConfigureWebhooksAsync(clientId, update.WebhookSettings, cancellationToken);
            }

            if (update.CustomSettings != null)
            {
                foreach (var setting in update.CustomSettings)
                {
                    client.Configuration.CustomSettings[setting.Key] = setting.Value;
                }
            }

            client.UpdatedAt = DateTime.UtcNow;
            client.UpdatedBy = update.UpdatedBy;

            // Save updated configuration
            await _clientRepository.SaveClientAsync(client, cancellationToken);

            // Clear cache
            _cache.Remove($"client_config_{clientId}");

            _logger.LogInformation("Client configuration for {ClientId} updated successfully", clientId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update client configuration for {ClientId}", clientId);
            throw;
        }
    }

    public async Task<ClientSDKBundle> GenerateSDKAsync(
        string clientId,
        SDKGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating SDK for client {ClientId}, language {Language}", clientId, request.Language);

        try
        {
            var client = await _clientRepository.GetClientAsync(clientId, cancellationToken);
            if (client == null)
            {
                throw new ArgumentException($"Client {clientId} not found");
            }

            var sdkGenerator = GetSDKGenerator(request.Language);
            var sdkBundle = await sdkGenerator.GenerateSDKAsync(client, request, cancellationToken);

            // Save SDK metadata
            var sdkMetadata = new ClientSDKMetadata
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Language = request.Language,
                Version = sdkBundle.Version,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = request.GeneratedBy,
                DownloadUrl = sdkBundle.DownloadUrl,
                ExpiresAt = DateTime.UtcNow.AddDays(30) // SDK download expires in 30 days
            };

            await _clientRepository.SaveSDKMetadataAsync(sdkMetadata, cancellationToken);

            _logger.LogInformation("SDK generated successfully for client {ClientId}, language {Language}", clientId, request.Language);
            return sdkBundle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate SDK for client {ClientId}, language {Language}", clientId, request.Language);
            throw;
        }
    }

    private async Task<ClientConfiguration?> GetClientConfigurationAsync(string clientId, CancellationToken cancellationToken)
    {
        var cacheKey = $"client_config_{clientId}";
        
        if (_cache.TryGetValue(cacheKey, out ClientConfiguration? cachedConfig))
        {
            return cachedConfig;
        }

        var config = await _clientRepository.GetClientAsync(clientId, cancellationToken);
        if (config != null)
        {
            _cache.Set(cacheKey, config, TimeSpan.FromMinutes(15));
        }

        return config;
    }

    private async Task<APIResponse<T>> ProcessRequestByEndpointAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        return request.Endpoint.ToLowerInvariant() switch
        {
            "/api/data" => await ProcessDataRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/workflow" => await ProcessWorkflowRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/analytics" => await ProcessAnalyticsRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/users" => await ProcessUserRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/reports" => await ProcessReportRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/compliance" => await ProcessComplianceRequestAsync<T>(client, request, requestId, cancellationToken),
            "/api/ai" => await ProcessAIRequestAsync<T>(client, request, requestId, cancellationToken),
            _ => new APIResponse<T>
            {
                Success = false,
                ErrorCode = "ENDPOINT_NOT_FOUND",
                Message = $"Endpoint {request.Endpoint} not found",
                RequestId = requestId
            }
        };
    }

    private async Task<APIResponse<T>> ProcessDataRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement data API logic
        await Task.Delay(50, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "Data request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessWorkflowRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement workflow API logic
        await Task.Delay(100, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "Workflow request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessAnalyticsRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement analytics API logic
        await Task.Delay(200, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "Analytics request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessUserRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement user API logic
        await Task.Delay(75, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "User request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessReportRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement report API logic
        await Task.Delay(300, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "Report request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessComplianceRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement compliance API logic
        await Task.Delay(150, cancellationToken); // Simulate processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "Compliance request processed successfully",
            RequestId = requestId
        };
    }

    private async Task<APIResponse<T>> ProcessAIRequestAsync<T>(
        ClientConfiguration client,
        APIRequest request,
        string requestId,
        CancellationToken cancellationToken)
    {
        // Implement AI API logic
        await Task.Delay(500, cancellationToken); // Simulate AI processing
        
        return new APIResponse<T>
        {
            Success = true,
            Data = default(T),
            Message = "AI request processed successfully",
            RequestId = requestId
        };
    }

    private Dictionary<string, ClientEndpoint> GenerateClientEndpoints(string clientId, ClientType clientType)
    {
        var baseUrl = $"https://api.toss-platform.com/clients/{clientId}";
        
        var endpoints = new Dictionary<string, ClientEndpoint>
        {
            ["data"] = new ClientEndpoint { Url = $"{baseUrl}/api/data", Method = "GET,POST,PUT,DELETE", Description = "Data management operations" },
            ["workflow"] = new ClientEndpoint { Url = $"{baseUrl}/api/workflow", Method = "GET,POST", Description = "Workflow management" },
            ["analytics"] = new ClientEndpoint { Url = $"{baseUrl}/api/analytics", Method = "GET", Description = "Analytics and reporting" },
            ["users"] = new ClientEndpoint { Url = $"{baseUrl}/api/users", Method = "GET,POST,PUT,DELETE", Description = "User management" },
            ["reports"] = new ClientEndpoint { Url = $"{baseUrl}/api/reports", Method = "GET,POST", Description = "Report generation" },
            ["compliance"] = new ClientEndpoint { Url = $"{baseUrl}/api/compliance", Method = "GET", Description = "Compliance monitoring" }
        };

        // Add AI endpoints for enterprise clients
        if (clientType == ClientType.Enterprise || clientType == ClientType.AIEnabled)
        {
            endpoints["ai"] = new ClientEndpoint 
            { 
                Url = $"{baseUrl}/api/ai", 
                Method = "POST", 
                Description = "AI processing and inference" 
            };
        }

        return endpoints;
    }

    private RateLimitConfiguration GetDefaultRateLimits(ClientType clientType)
    {
        return clientType switch
        {
            ClientType.Enterprise => new RateLimitConfiguration
            {
                RequestsPerMinute = 1000,
                RequestsPerHour = 50000,
                RequestsPerDay = 1000000,
                BurstLimit = 100
            },
            ClientType.Professional => new RateLimitConfiguration
            {
                RequestsPerMinute = 500,
                RequestsPerHour = 25000,
                RequestsPerDay = 500000,
                BurstLimit = 50
            },
            _ => new RateLimitConfiguration
            {
                RequestsPerMinute = 100,
                RequestsPerHour = 5000,
                RequestsPerDay = 100000,
                BurstLimit = 20
            }
        };
    }

    private void PerformHealthChecks(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _integrationRepository.PerformHealthChecksAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during integration health checks");
            }
        });
    }

    private void CollectMetrics(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _analyticsService.CollectIntegrationMetricsAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during metrics collection");
            }
        });
    }

    public void Dispose()
    {
        _healthCheckTimer?.Dispose();
        _metricsCollectionTimer?.Dispose();
    }
}

// Supporting enums and classes
public enum ClientType
{
    WebApp,
    MobileApp,
    SPA,
    API,
    Service,
    Enterprise,
    Professional,
    AIEnabled
}

public enum ClientStatus
{
    Pending,
    Active,
    Suspended,
    Disabled,
    Archived
}

public enum ClientEnvironment
{
    Development,
    Staging,
    Production
}

public enum IntegrationType
{
    REST,
    GraphQL,
    WebSocket,
    Webhook,
    MessageQueue,
    FileTransfer
}

public enum IntegrationStatus
{
    Active,
    Inactive,
    Error,
    Degraded
}
