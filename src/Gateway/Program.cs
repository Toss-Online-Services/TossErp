using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using eShop.ServiceDefaults;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TOSS ERP Gateway API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

// Basic rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// Add YARP for reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add HTTP client for service communication
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TOSS ERP Gateway API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("Default");
app.UseRateLimiter();
app.UseCorrelationId();
app.UseTenantResolution();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    service = "gateway", 
    timestamp = DateTime.UtcNow,
    infrastructure = new {
        postgres = "localhost:5432",
        redis = "localhost:6379",
        rabbitmq = "localhost:5672"
    }
}));

// Basic API endpoints
app.MapGet("/", () => Results.Ok(new { 
    ok = true, 
    service = "gateway",
    message = "TOSS ERP Gateway is running!",
    endpoints = new[] { "/health", "/swagger", "/api" }
}));

app.MapGet("/api/status", () => Results.Ok(new { 
    status = "operational",
    services = new[] { "gateway", "stock-api", "postgres", "redis", "rabbitmq" },
    timestamp = DateTime.UtcNow
}));

// Map the reverse proxy to handle API routes
app.MapReverseProxy();

app.Run();



