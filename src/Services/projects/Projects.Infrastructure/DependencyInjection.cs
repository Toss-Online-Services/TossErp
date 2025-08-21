using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Projects.Infrastructure.Data;
using TossErp.Projects.Infrastructure.Repositories;
using TossErp.Projects.Infrastructure.Services;

namespace TossErp.Projects.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<ProjectsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ProjectsDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
            
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching(true);
            options.EnableDetailedErrors(false);
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(ProjectsRepository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();

        // Unit of Work
        services.AddScoped<IProjectsUnitOfWork, ProjectsUnitOfWork>();

        // Services
        services.AddScoped<IProjectReportingService, ProjectReportingService>();

        return services;
    }
}
