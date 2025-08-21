using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SendGrid.Extensions.DependencyInjection;
using Setup.Application.Common.Interfaces;
using Setup.Infrastructure.BackgroundJobs;
using Setup.Infrastructure.Data;
using Setup.Infrastructure.Repositories;
using Setup.Infrastructure.Services;

namespace Setup.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSetupInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<SetupDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SetupDbContext).Assembly.FullName)));

        // Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<SetupDbContext>()
        .AddDefaultTokenProviders();

        // SendGrid
        var sendGridApiKey = configuration.GetValue<string>("SendGrid:ApiKey");
        if (!string.IsNullOrEmpty(sendGridApiKey))
        {
            services.AddSendGrid(options =>
            {
                options.ApiKey = sendGridApiKey;
            });
        }

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<ISetupUnitOfWork, SetupUnitOfWork>();

        // Services
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISystemConfigService, SystemConfigService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEmailService, EmailService>();

        // Background Jobs
        services.AddHostedService<SubscriptionManagementJob>();
        services.AddHostedService<UsageQuotaMonitoringJob>();
        services.AddHostedService<AuditProcessingJob>();
        services.AddHostedService<DataCleanupJob>();
        services.AddHostedService<SystemMaintenanceJob>();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        return services;
    }

    public static async Task InitializeSetupDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SetupDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SetupDbContext>>();

        try
        {
            logger.LogInformation("Initializing Setup database...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("Applying pending migrations...");
                await context.Database.MigrateAsync();
            }

            // Seed initial data
            await SeedInitialDataAsync(context, logger);

            logger.LogInformation("Setup database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the Setup database");
            throw;
        }
    }

    private static async Task SeedInitialDataAsync(SetupDbContext context, ILogger logger)
    {
        try
        {
            // Seed default application configurations
            if (!await context.ApplicationConfigurations.AnyAsync())
            {
                logger.LogInformation("Seeding default application configurations...");

                var defaultConfigs = new[]
                {
                    new Setup.Domain.Aggregates.ApplicationConfigurationAggregate.ApplicationConfiguration
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "System Settings",
                        Description = "Core system configuration",
                        IsEnabled = true,
                        Environment = Setup.Domain.Enums.Environment.Production,
                        CreatedBy = "system",
                        CreatedAt = DateTime.UtcNow,
                        ConfigurationValues = new List<Setup.Domain.ValueObjects.ConfigurationValue>
                        {
                            new()
                            {
                                Key = "System.Name",
                                Value = "TOSS ERP",
                                DataType = Setup.Domain.Enums.ConfigDataType.String,
                                IsEncrypted = false,
                                IsReadOnly = true
                            },
                            new()
                            {
                                Key = "System.Version",
                                Value = "1.0.0",
                                DataType = Setup.Domain.Enums.ConfigDataType.String,
                                IsEncrypted = false,
                                IsReadOnly = true
                            },
                            new()
                            {
                                Key = "System.MaxTenants",
                                Value = "1000",
                                DataType = Setup.Domain.Enums.ConfigDataType.Integer,
                                IsEncrypted = false,
                                IsReadOnly = false
                            }
                        }
                    }
                };

                context.ApplicationConfigurations.AddRange(defaultConfigs);
            }

            // Seed default feature flags
            if (!await context.FeatureFlags.AnyAsync())
            {
                logger.LogInformation("Seeding default feature flags...");

                var defaultFeatures = new[]
                {
                    new Setup.Domain.Aggregates.ApplicationConfigurationAggregate.FeatureFlag
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = "MultiTenancy",
                        Name = "Multi-Tenancy Support",
                        Description = "Enable multi-tenant functionality",
                        IsEnabled = true,
                        EnabledForPercentage = 100,
                        CreatedBy = "system",
                        CreatedAt = DateTime.UtcNow,
                        IsArchived = false,
                        EnabledUsers = new List<string>(),
                        EnabledRoles = new List<string>(),
                        EnabledTenants = new List<string>(),
                        Conditions = new Dictionary<string, object>(),
                        Variants = new Dictionary<string, object>()
                    },
                    new Setup.Domain.Aggregates.ApplicationConfigurationAggregate.FeatureFlag
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = "AuditLogging",
                        Name = "Audit Logging",
                        Description = "Enable comprehensive audit logging",
                        IsEnabled = true,
                        EnabledForPercentage = 100,
                        CreatedBy = "system",
                        CreatedAt = DateTime.UtcNow,
                        IsArchived = false,
                        EnabledUsers = new List<string>(),
                        EnabledRoles = new List<string>(),
                        EnabledTenants = new List<string>(),
                        Conditions = new Dictionary<string, object>(),
                        Variants = new Dictionary<string, object>()
                    }
                };

                context.FeatureFlags.AddRange(defaultFeatures);
            }

            // Seed default notification templates
            if (!await context.NotificationTemplates.AnyAsync())
            {
                logger.LogInformation("Seeding default notification templates...");

                var defaultTemplates = new[]
                {
                    new Setup.Domain.Aggregates.ApplicationConfigurationAggregate.NotificationTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Welcome Email",
                        Type = Setup.Domain.Enums.NotificationType.Email,
                        Subject = "Welcome to TOSS ERP!",
                        Body = "Welcome to TOSS ERP! Please activate your account by clicking the link below.",
                        IsHtml = true,
                        Language = "en-US",
                        IsEnabled = true,
                        Priority = Setup.Domain.Enums.NotificationPriority.Normal,
                        CreatedBy = "system",
                        CreatedAt = DateTime.UtcNow,
                        UsageCount = 0,
                        Version = 1,
                        Variables = new List<string> { "UserName", "ActivationLink" },
                        Triggers = new List<string> { "UserRegistration" },
                        Conditions = new Dictionary<string, object>(),
                        Metadata = new Dictionary<string, object>(),
                        NotificationSettings = new Setup.Domain.ValueObjects.NotificationSettings
                        {
                            EnableEmail = true,
                            EnableSms = false,
                            EnablePush = false,
                            EnableInApp = false,
                            DeliveryDelay = 0,
                            RetryAttempts = 3,
                            RetryInterval = 300,
                            ExpiryHours = 24
                        }
                    }
                };

                context.NotificationTemplates.AddRange(defaultTemplates);
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Initial data seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding initial data");
            throw;
        }
    }
}
