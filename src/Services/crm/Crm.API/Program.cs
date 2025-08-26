var builder = WebApplication.CreateBuilder(args);

// Add logging to see what's happening
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Add exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetService<ILogger<Program>>();
        logger?.LogError("Unhandled exception occurred");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal Server Error");
    });
});

// Add request logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetService<ILogger<Program>>();
    logger?.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    try
    {
        await next();
        logger?.LogInformation($"Response: {context.Response.StatusCode}");
    }
    catch (Exception ex)
    {
        logger?.LogError(ex, "Exception during request processing");
        throw;
    }
});

app.UseCors("AllowAll");
app.MapControllers();

Console.WriteLine("Starting API on http://localhost:5049...");
app.Run();
