namespace TossErp.HR.API.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup("/api/v1");
        
        // Health endpoint
        apiGroup.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "hr" }))
            .WithName("GetHRHealth")
            .WithTags("Health")
            .WithOpenApi();

        // Status endpoint  
        apiGroup.MapGet("/_status", () => Results.Ok(new { 
            service = "TossErp.HR.API", 
            version = "1.0.0",
            timestamp = DateTime.UtcNow 
        }))
            .WithName("GetHRStatus")
            .WithTags("Status")
            .WithOpenApi();

        return app;
    }
}
