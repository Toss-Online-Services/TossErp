using TossErp.Stock.Application;
using TossErp.Stock.Infrastructure;
using TossErp.Stock.Infrastructure.Data;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.API.Services;
using TossErp.Stock.API.Infrastructure;
using TossErp.Stock.API.Endpoints;
using eShop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (health checks, OpenTelemetry, service discovery, resilience)
builder.AddServiceDefaults();

// Add services to the container.
builder.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add HTTP context accessor for current user service
builder.Services.AddHttpContextAccessor();

// Register current user service
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Web UI
var webOrigin = builder.Configuration["Web:Origin"] ?? "http://localhost:3000";
var devOrigins = new[]
{
    webOrigin,
    "http://localhost:3000",
    "http://localhost:3001",
    "http://localhost:3002",
    "http://localhost:3003",
    "http://localhost:3004",
    "http://localhost:3005"
};
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
        policy.WithOrigins(devOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("WebClient");

app.MapGet("/", () => "TOSS ERP III Stock API is running");

// Map default endpoints (health, alive, optional prometheus)
app.MapDefaultEndpoints();

// Map all endpoint groups
app.MapEndpoints();

app.Run();

public partial class Program { }
