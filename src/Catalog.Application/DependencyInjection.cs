using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using AutoMapper;
using Catalog.Application.MappingProfiles;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(cfg => cfg.AddProfile<CatalogMappingProfile>(), Assembly.GetExecutingAssembly());
        // Register other services, validators, etc.
        return services;
    }
} 
