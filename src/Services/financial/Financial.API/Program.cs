using Microsoft.EntityFrameworkCore;
using Financial.Infrastructure.Data;
using Financial.Application;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add application services (includes MediatR, FluentValidation, AutoMapper)
builder.Services.AddFinancialApplication();

// Add database context
var connectionString = builder.Configuration.GetConnectionString("TossErpDb") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<FinancialDbContext>(options =>
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "financial");
        }));
}

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "TOSS Financial Services API", 
        Version = "v1",
        Description = "API for financial services including microfinance, insurance, and banking integration"
    });
    c.EnableAnnotations();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FinancialDbContext>("financial-db");

// Add logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TOSS Financial Services API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseRouting();
app.UseCors("AllowAll");

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

// Add request logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Processing request: {Method} {Path}", 
        context.Request.Method, context.Request.Path);
    
    await next();
    
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add info endpoint
app.MapGet("/info", () => new
{
    Service = "TOSS Financial Services API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Features = new[]
    {
        "Microfinance Loans",
        "Insurance Policies & Claims",
        "Banking Integration",
        "Transaction Management",
        "Financial Reporting"
    }
});

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<FinancialDbContext>();
        
        // Only attempt migration if connection string is available
        if (!string.IsNullOrEmpty(connectionString))
        {
            await context.Database.EnsureCreatedAsync();
            
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Financial database initialized successfully");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not initialize database. This is expected if running without database connection.");
    }
}

app.Run();
