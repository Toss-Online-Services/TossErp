using TossErp.HR.Application;
using TossErp.HR.Infrastructure;
using TossErp.HR.Infrastructure.Data;
using TossErp.HR.Application.Common.Interfaces;
using TossErp.HR.API.Services;
using TossErp.HR.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
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

app.MapGet("/", () => "TOSS ERP III HR API is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "hr" }));
app.MapGet("/_status", () => Results.Ok(new { 
    service = "TossErp.HR.API", 
    version = "1.0.0",
    timestamp = DateTime.UtcNow 
}));

// Map health checks
app.MapHealthChecks("/health");

// Map all endpoint groups
app.MapEndpoints();

// Ensure database is migrated and seeded on startup (dev/local)
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<HRDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

app.Run();

public partial class Program { }
