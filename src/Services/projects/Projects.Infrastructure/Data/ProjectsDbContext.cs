using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TossErp.Projects.Infrastructure.Data.Configurations;
using TossErp.ServiceDefaults.Application.Common.Interfaces;
using TossErp.ServiceDefaults.Domain.SeedWork;

namespace TossErp.Projects.Infrastructure.Data;

/// <summary>
/// Database context for Projects service with multi-tenant support
/// </summary>
public class ProjectsDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options, ICurrentTenantService currentTenantService)
        : base(options)
    {
        _currentTenantService = currentTenantService;
    }

    // Project Management
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
    public DbSet<ProjectMilestone> ProjectMilestones { get; set; } = null!;
    public DbSet<ProjectRisk> ProjectRisks { get; set; } = null!;
    public DbSet<ProjectIssue> ProjectIssues { get; set; } = null!;

    // Resource Management
    public DbSet<ResourceAllocation> ResourceAllocations { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;

    // Supporting Entities
    public DbSet<TaskComment> TaskComments { get; set; } = null!;
    public DbSet<TaskAttachment> TaskAttachments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectTaskConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMilestoneConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectRiskConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectIssueConfiguration());
        modelBuilder.ApplyConfiguration(new ResourceAllocationConfiguration());
        modelBuilder.ApplyConfiguration(new TimeEntryConfiguration());
        modelBuilder.ApplyConfiguration(new TaskCommentConfiguration());
        modelBuilder.ApplyConfiguration(new TaskAttachmentConfiguration());

        // Global query filters for multi-tenancy
        var tenantId = _currentTenantService.TenantId;
        if (!string.IsNullOrEmpty(tenantId))
        {
            modelBuilder.Entity<Project>().HasQueryFilter(e => e.TenantId == tenantId);
        }

        // Configure schema
        modelBuilder.HasDefaultSchema("projects");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTenantIdForNewEntities();
        SetAuditFields();

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency conflicts
            throw;
        }
    }

    public override int SaveChanges()
    {
        SetTenantIdForNewEntities();
        SetAuditFields();

        try
        {
            return base.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency conflicts
            throw;
        }
    }

    private void SetTenantIdForNewEntities()
    {
        var tenantId = _currentTenantService.TenantId;
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Entity is ITenantEntity tenantEntity)
            {
                if (string.IsNullOrEmpty(tenantEntity.TenantId))
                {
                    var entity = entry.Entity;
                    var property = entity.GetType().GetProperty("TenantId");
                    property?.SetValue(entity, tenantId);
                }
            }
        }
    }

    private void SetAuditFields()
    {
        var userId = _currentTenantService.UserId ?? "system";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                var entity = entry.Entity;
                
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.GetType().GetProperty("CreatedAt")?.SetValue(entity, now);
                        entity.GetType().GetProperty("CreatedBy")?.SetValue(entity, userId);
                        break;
                    
                    case EntityState.Modified:
                        entity.GetType().GetProperty("ModifiedAt")?.SetValue(entity, now);
                        entity.GetType().GetProperty("ModifiedBy")?.SetValue(entity, userId);
                        break;
                }
            }
        }
    }
}
