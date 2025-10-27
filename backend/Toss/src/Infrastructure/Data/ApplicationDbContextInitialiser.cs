using Toss.Domain.Constants;
using Toss.Domain.Entities;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Toss.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // Check if database can be connected
            if (await _context.Database.CanConnectAsync())
            {
                // Check if there are pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Pending migrations detected: {Migrations}", 
                        string.Join(", ", pendingMigrations));
                    
                    // If database has tables but no migration history, 
                    // assume it was created manually and skip migration
                    if (!appliedMigrations.Any() && await _context.Shops.AnyAsync())
                    {
                        _logger.LogWarning("Database has tables but no migration history. " +
                            "Skipping automatic migration. Please apply migrations manually if needed.");
                        _logger.LogWarning("To mark migrations as applied, see: MarkBaseMigrationApplied.sql");
                        return;
                    }
                    
                    // Apply pending migrations
                    try
                    {
                        await _context.Database.MigrateAsync();
                        _logger.LogInformation("Database migrations applied successfully.");
                    }
                    catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P07")
                    {
                        // Table already exists - likely migration history is out of sync
                        _logger.LogWarning(pgEx, 
                            "Migration failed: Table already exists. Database may need manual migration sync. " +
                            "See APPLY_MIGRATION_INSTRUCTIONS.md for details.");
                    }
                }
                else
                {
                    _logger.LogInformation("Database is up to date. No pending migrations.");
                }
            }
            else
            {
                _logger.LogWarning("Cannot connect to database. Skipping migration check.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            // Don't throw - allow app to start even if migration fails
            // This is important for NSwag generation and development scenarios
            _logger.LogWarning("Database initialisation failed, but application will continue. " +
                "Some features may not work until database is properly configured.");
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
            _logger.LogError(ex, "An error occurred while seeding the database.");
            // Don't throw - allow app to start even if seeding fails
            _logger.LogWarning("Database seeding failed, but application will continue. " +
                "Default users and roles will not be created until database is properly configured.");
        }
    }

    public async Task TrySeedAsync()
    {
        try
        {
            // Check if Identity tables exist before attempting to seed
            var identityTablesExist = await CheckIdentityTablesExistAsync();
            if (!identityTablesExist)
            {
                _logger.LogWarning("Identity tables do not exist. Skipping seed data. " +
                    "Please run 'dotnet ef database update' to create the Identity schema.");
                return;
            }

            // Default roles
            var administratorRole = new IdentityRole(Roles.Administrator);

            if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
                _logger.LogInformation("Created Administrator role.");
            }

            // Default users
            var administrator = new ApplicationUser 
            { 
                UserName = "administrator@localhost", 
                Email = "administrator@localhost",
                FirstName = "Admin",
                LastName = "User"
            };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "Administrator1!");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
                }
                _logger.LogInformation("Created default administrator user.");
            }

            // TODO: Add TOSS seed data (shops, products, suppliers) here if needed
        }
        catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P01")
        {
            // Table does not exist
            _logger.LogWarning(pgEx, 
                "Identity tables do not exist. Skipping seed data. " +
                "Please run 'dotnet ef database update' to create the Identity schema.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task<bool> CheckIdentityTablesExistAsync()
    {
        try
        {
            // Check if AspNetRoles table exists
            var result = await _context.Database.ExecuteSqlRawAsync(
                @"SELECT COUNT(*) 
                  FROM information_schema.tables 
                  WHERE table_schema = 'public' 
                  AND table_name = 'AspNetRoles'");
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not check for Identity tables existence.");
            return false;
        }
    }
}
