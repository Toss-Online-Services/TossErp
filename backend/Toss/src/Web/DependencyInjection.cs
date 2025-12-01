using System;
using Azure.Identity;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NetEscapades.AspNetCore.SecurityHeaders;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text.Json.Serialization;
using Toss.Application.Common.Interfaces;
using Toss.Infrastructure.Data;
using Toss.Web.Infrastructure;
using Toss.Web.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();


    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        // Configure JSON options for enum string conversion
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.PropertyNamingPolicy = null; // Keep original property names
        });

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        // Add response caching for GET endpoints
        builder.Services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1 MB
            options.UseCaseSensitivePaths = false;
        });

        // Add memory cache for application-level caching
        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // Limit number of cache entries
        });

        // Add API versioning
        builder.Services.AddApiVersioningServices();

        // Harden cookies and antiforgery tokens
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => false;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = HttpOnlyPolicy.Always;
        });

        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.Name = "__Host-toss-antiforgery";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.SuppressXFrameOptionsHeader = false;
        });

        // Limit inbound payload sizes
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = builder.Configuration.GetValue<long>("Security:MultipartBodyLimitBytes", 25L * 1024 * 1024);
            options.ValueLengthLimit = builder.Configuration.GetValue<int>("Security:ValueLengthLimit", 1_000_000);
            options.MemoryBufferThreshold = 64 * 1024;
        });

        builder.Services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = builder.Configuration.GetValue<long?>("Security:MaxRequestBodySizeBytes", 30L * 1024 * 1024);
        });

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = builder.Configuration.GetValue<long?>("Security:MaxRequestBodySizeBytes", 30L * 1024 * 1024);
        });

        builder.Services.AddHsts(options =>
        {
            options.Preload = builder.Configuration.GetValue("Security:HstsPreload", true);
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(builder.Configuration.GetValue("Security:HstsDays", 60));
        });

        // Add CORS policy for frontend access (Development only)
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://localhost:3001",
                            "https://localhost:3001")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        // Configure versioned OpenAPI/Swagger
        builder.Services.AddVersionedSwagger(builder.Configuration);
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
