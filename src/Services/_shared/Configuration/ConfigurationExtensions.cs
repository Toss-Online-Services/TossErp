using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace TossErp.Configuration;

/// <summary>
/// Extension methods for configuration setup
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Add TOSS ERP configuration options
    /// </summary>
    public static IServiceCollection AddTossConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration options with validation
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EventBusOptions>()
            .Bind(configuration.GetSection(EventBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Add Azure Key Vault configuration
    /// </summary>
    public static IConfigurationBuilder AddTossKeyVault(this IConfigurationBuilder builder, string keyVaultUrl)
    {
        if (string.IsNullOrEmpty(keyVaultUrl))
            return builder;

        try
        {
            var credential = new DefaultAzureCredential();
            builder.AddAzureKeyVault(new Uri(keyVaultUrl), credential);
        }
        catch (Exception ex)
        {
            // Log warning but don't fail startup
            Console.WriteLine($"Warning: Could not connect to Key Vault at {keyVaultUrl}: {ex.Message}");
        }

        return builder;
    }

    /// <summary>
    /// Get a strongly-typed configuration section
    /// </summary>
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    /// <summary>
    /// Validate configuration on startup
    /// </summary>
    public static void ValidateConfiguration(this IServiceProvider serviceProvider)
    {
        // This will trigger validation for all registered options
        serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
        serviceProvider.GetRequiredService<IOptions<EventBusOptions>>();
        serviceProvider.GetRequiredService<IOptions<JwtOptions>>();
        serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
    }
}
