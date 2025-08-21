using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Common.Interfaces;
using Projects.Infrastructure.BackgroundServices;
using Projects.Infrastructure.Data;
using Projects.Infrastructure.HealthChecks;
using Projects.Infrastructure.Hubs;
using Projects.Infrastructure.Repositories;
using Projects.Infrastructure.Services;

namespace Projects.Infrastructure;

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
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Business Services
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITimeTrackingService, TimeTrackingService>();
        services.AddScoped<IReportingService, ReportingService>();

        // SignalR Hubs
        services.AddSignalR();
        services.AddScoped<ProjectUpdatesHub>();
        services.AddScoped<TeamCollaborationHub>();
        services.AddScoped<TimeTrackingHub>();

        // Background Services
        services.AddHostedService<ProjectStatusUpdateService>();
        services.AddHostedService<TimeEntryReminderService>();
        services.AddHostedService<DataCleanupService>();

        // Health Checks
        services.AddScoped<ProjectsHealthCheck>();

        return services;
    }
}
