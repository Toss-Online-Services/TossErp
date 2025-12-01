using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetEscapades.AspNetCore.SecurityHeaders;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
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
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Database.CanConnectAsync())
        {
            await context.Database.MigrateAsync();
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database migration/seeding skipped - database may not be available. This is normal during build/NSwag generation.");
    }
}

var securitySection = app.Configuration.GetSection("Security");
var enforceHttps = securitySection.GetValue("EnforceHttps", !app.Environment.IsDevelopment());
var hstsDays = securitySection.GetValue("HstsDays", 60);
var preloadHsts = securitySection.GetValue("HstsPreload", true);

HeaderPolicyCollection securityHeadersPolicy = new HeaderPolicyCollection()
    .AddDefaultSecurityHeaders();

if (enforceHttps)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
}

app.UseSecurityHeaders(securityHeadersPolicy);

// Enable response caching (must come before UseStaticFiles)
app.UseResponseCaching();

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
app.UseCookiePolicy();

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
