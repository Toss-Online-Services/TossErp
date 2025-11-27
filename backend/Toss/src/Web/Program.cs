using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.Services.Tenancy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthLimiter", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 5;
        limiterOptions.QueueLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Microsoft's recommended approach: Apply migrations first, then seed
    // Note: This is wrapped in try-catch to allow builds to succeed even when database is not available (e.g., during NSwag generation)
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Check if database is available before attempting migration
            if (await context.Database.CanConnectAsync())
            {
                // Apply all pending migrations
                await context.Database.MigrateAsync();
                
                // Then seed the database
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                await initialiser.SeedAsync();
            }
        }
    }
    catch (Exception ex)
    {
        // Log but don't fail - allows build to succeed when database is not available
        // This is especially important for NSwag code generation during build
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database migration/seeding skipped - database may not be available. This is normal during build/NSwag generation.");
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Only redirect to HTTPS in non-development environments
// In development, allow HTTP to avoid CORS issues with localhost
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

// Configure exception handling based on environment
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Correct middleware order for CORS (per Microsoft best practices)
app.UseRouting();

// Enable CORS in Development only (must come after UseRouting and before UseAuthorization)
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontend");
}

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BusinessContextMiddleware>();

app.Map("/", () => Results.Redirect("/api"));

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

public partial class Program { }
