using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

namespace TossErp.HR.Infrastructure.Data;

public class HRDbContextInitialiser
{
    private readonly ILogger<HRDbContextInitialiser> _logger;
    private readonly HRDbContext _context;

    public HRDbContextInitialiser(ILogger<HRDbContextInitialiser> logger, HRDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default employees, if any
        await _context.SaveChangesAsync();
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<HRDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        if (app.Environment.IsDevelopment())
        {
            await initialiser.SeedAsync();
        }
    }
}
