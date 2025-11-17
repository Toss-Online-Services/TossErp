using Microsoft.EntityFrameworkCore;
using Toss.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Microsoft's recommended approach: Apply migrations first, then seed
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Apply all pending migrations
        await context.Database.MigrateAsync();
        
        // Then seed the database
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.SeedAsync();
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

app.UseAuthorization();

app.Map("/", () => Results.Redirect("/api"));

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

public partial class Program { }
