using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TossErp.HR.Application.Common.Interfaces;
using TossErp.HR.Infrastructure.Data;

namespace TossErp.HR.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<HRDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IHRDbContext>(provider => provider.GetRequiredService<HRDbContext>());

        services.AddScoped<HRDbContextInitialiser>();

        return services;
    }
}
