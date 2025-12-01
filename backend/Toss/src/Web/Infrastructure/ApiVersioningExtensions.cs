using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Security;

namespace Toss.Web.Infrastructure;

public static class ApiVersioningExtensions
{
    /// <summary>
    /// Adds API versioning services to the application
    /// </summary>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-API-Version")
            );
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    /// <summary>
    /// Configures NSwag/Swagger to support API versioning
    /// </summary>
    public static IServiceCollection AddVersionedSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApiDocument((document, serviceProvider) =>
        {
            var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
            var apiVersion = apiVersionDescriptionProvider.ApiVersionDescriptions
                .OrderByDescending(x => x.ApiVersion)
                .First();

            document.Title = "TOSS ERP-III API";
            document.Version = apiVersion.ApiVersion.ToString();
            document.Description = "TOSS ERP-III - Mobile-first, offline-first ERP for South African township and rural SMMEs";
            document.DocumentName = apiVersion.GroupName;

            // Add JWT Bearer authentication
            document.AddSecurity("Bearer", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "Enter your JWT token",
                In = OpenApiSecurityApiKeyLocation.Header,
                Name = "Authorization"
            });

            // Set security requirement
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = OpenApiReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Tags are automatically added by WithTags() in WebApplicationExtensions.MapGroup()
        });

        return services;
    }
}

