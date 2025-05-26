using Catalog.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class CatalogApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CatalogApplicationServiceCollectionExtensions).Assembly);
        services.AddScoped<IProductService, ProductService>();
        // Add AutoMapper or other services as needed
        return services;
    }
} 
