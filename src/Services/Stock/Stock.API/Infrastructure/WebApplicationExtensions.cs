using System.Reflection;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.API.Infrastructure;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GetType().Name.ToLowerInvariant();

        return app
            .MapGroup($"api/{groupName}")
            .WithGroupName(groupName)
            .WithTags(groupName);
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);
        var assembly = Assembly.GetExecutingAssembly();

        // Use GetAllTypes() instead of GetExportedTypes() to catch all types
        var endpointGroupTypes = assembly.GetTypes()
            .Where(t => t.IsClass && 
                       !t.IsAbstract && 
                       t.IsSubclassOf(endpointGroupType) &&
                       t.Namespace?.StartsWith("TossErp.Stock.API.Endpoints") == true)
            .ToList();

        // Log discovered types for debugging
        var logger = app.Services.GetService<ILogger<object>>();
        logger?.LogInformation("Discovered {Count} endpoint group types: {Types}", 
            endpointGroupTypes.Count, 
            string.Join(", ", endpointGroupTypes.Select(t => t.Name)));

        foreach (var type in endpointGroupTypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is EndpointGroupBase instance)
                {
                    instance.Map(app);
                    logger?.LogInformation("Mapped endpoint group: {TypeName}", type.Name);
                }
                else
                {
                    logger?.LogWarning("Failed to create instance of endpoint group: {TypeName}", type.Name);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error mapping endpoint group {TypeName}: {Message}", type.Name, ex.Message);
            }
        }

        return app;
    }
}
