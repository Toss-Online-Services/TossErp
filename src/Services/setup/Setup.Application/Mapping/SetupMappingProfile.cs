using AutoMapper;
using TossErp.Setup.Domain.Entities;
using TossErp.Setup.Domain.Enums;
using TossErp.Setup.Application.DTOs;
using TossErp.Setup.Application.Commands;

namespace TossErp.Setup.Application.Mapping;

/// <summary>
/// AutoMapper profile for Setup Application
/// </summary>
public class SetupMappingProfile : Profile
{
    public SetupMappingProfile()
    {
        CreateTenantMappings();
        CreateUserMappings();
        CreateRoleMappings();
        CreatePermissionMappings();
        CreateSystemConfigMappings();
        CreateAuditLogMappings();
        CreateUsageStatsMappings();
        CreateReverseMappings();
    }

    private void CreateTenantMappings()
    {
        // Tenant mappings
        CreateMap<Tenant, TenantDto>()
            .ForMember(d => d.SubscriptionPlanName, opt => opt.MapFrom(s => s.SubscriptionPlan.ToString()))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Status == TenantStatus.Active))
            .ForMember(d => d.IsTrialExpired, opt => opt.MapFrom(s => IsTrialExpired(s)))
            .ForMember(d => d.TrialExpiresAt, opt => opt.MapFrom(s => CalculateTrialExpiry(s)))
            .ForMember(d => d.UserCount, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.ActiveUserCount, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.StorageUsedBytes, opt => opt.Ignore()); // Set by query handler

        CreateMap<Tenant, TenantSummaryDto>()
            .ForMember(d => d.SubscriptionPlanName, opt => opt.MapFrom(s => s.SubscriptionPlan.ToString()))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Status == TenantStatus.Active))
            .ForMember(d => d.UserCount, opt => opt.Ignore()); // Set by query handler
    }

    private void CreateUserMappings()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Status == UserStatus.Active))
            .ForMember(d => d.EmailVerified, opt => opt.MapFrom(s => s.EmailVerifiedAt.HasValue))
            .ForMember(d => d.HasValidSession, opt => opt.MapFrom(s => HasValidSession(s)))
            .ForMember(d => d.TenantName, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.Roles))
            .ForMember(d => d.Permissions, opt => opt.Ignore()); // Set by query handler

        CreateMap<User, UserSummaryDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Status == UserStatus.Active))
            .ForMember(d => d.RoleCount, opt => opt.MapFrom(s => s.Roles.Count));
    }

    private void CreateRoleMappings()
    {
        // UserRole mappings
        CreateMap<UserRole, UserRoleDto>()
            .ForMember(d => d.TypeName, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.IsSystemRole, opt => opt.MapFrom(s => s.Type == RoleType.System))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => true)) // Simplified
            .ForMember(d => d.TenantName, opt => opt.Ignore()) // Set by query handler if needed
            .ForMember(d => d.UserCount, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.Permissions));
    }

    private void CreatePermissionMappings()
    {
        // Permission mappings
        CreateMap<Permission, PermissionDto>()
            .ForMember(d => d.IsSystemPermission, opt => opt.MapFrom(s => true)) // Simplified
            .ForMember(d => d.RoleCount, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.UserCount, opt => opt.Ignore()); // Set by query handler
    }

    private void CreateSystemConfigMappings()
    {
        // SystemConfig mappings
        CreateMap<SystemConfig, SystemConfigDto>()
            .ForMember(d => d.IsGlobal, opt => opt.MapFrom(s => !s.TenantId.HasValue))
            .ForMember(d => d.IsEncrypted, opt => opt.MapFrom(s => s.IsEncrypted))
            .ForMember(d => d.TenantName, opt => opt.Ignore()); // Set by query handler if needed
    }

    private void CreateAuditLogMappings()
    {
        // AuditLog mappings
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(d => d.UserName, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.TenantName, opt => opt.Ignore()); // Set by query handler
    }

    private void CreateUsageStatsMappings()
    {
        // TenantUsageStats mappings
        CreateMap<TenantUsageStats, TenantUsageStatsDto>()
            .ForMember(d => d.StorageUsedFormatted, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.ApiCallsLastMonth, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.UtilizationPercentage, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.SubscriptionPlan, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.SubscriptionPlanName, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.IsOverLimit, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.LimitExceeded, opt => opt.Ignore()) // Set by query handler
            .ForMember(d => d.ModuleUsage, opt => opt.Ignore()); // Set by query handler
    }

    private void CreateReverseMappings()
    {
        // Reverse mappings for create/update commands
        
        // Tenant command mappings
        CreateMap<CreateTenantCommand, Tenant>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DatabaseName, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(s => TenantStatus.Active))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.ActivatedAt, opt => opt.Ignore())
            .ForMember(d => d.SuspendedAt, opt => opt.Ignore())
            .ForMember(d => d.SuspensionReason, opt => opt.Ignore());

        CreateMap<UpdateTenantCommand, Tenant>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Domain, opt => opt.Ignore())
            .ForMember(d => d.DatabaseName, opt => opt.Ignore())
            .ForMember(d => d.SubscriptionPlan, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.ActivatedAt, opt => opt.Ignore())
            .ForMember(d => d.SuspendedAt, opt => opt.Ignore())
            .ForMember(d => d.SuspensionReason, opt => opt.Ignore());

        // User command mappings
        CreateMap<CreateUserCommand, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(s => UserStatus.Active))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.LastLoginAt, opt => opt.Ignore())
            .ForMember(d => d.ActivatedAt, opt => opt.Ignore())
            .ForMember(d => d.DeactivatedAt, opt => opt.Ignore())
            .ForMember(d => d.DeactivationReason, opt => opt.Ignore())
            .ForMember(d => d.EmailVerifiedAt, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.SecurityStamp, opt => opt.Ignore())
            .ForMember(d => d.Roles, opt => opt.Ignore());

        CreateMap<UpdateUserCommand, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Email, opt => opt.Ignore())
            .ForMember(d => d.Username, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.LastLoginAt, opt => opt.Ignore())
            .ForMember(d => d.ActivatedAt, opt => opt.Ignore())
            .ForMember(d => d.DeactivatedAt, opt => opt.Ignore())
            .ForMember(d => d.DeactivationReason, opt => opt.Ignore())
            .ForMember(d => d.EmailVerifiedAt, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.SecurityStamp, opt => opt.Ignore())
            .ForMember(d => d.Roles, opt => opt.Ignore());

        // Role command mappings
        CreateMap<CreateRoleCommand, UserRole>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.Users, opt => opt.Ignore())
            .ForMember(d => d.Permissions, opt => opt.Ignore());

        // Permission command mappings
        CreateMap<CreatePermissionCommand, Permission>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.Roles, opt => opt.Ignore());

        // SystemConfig command mappings
        CreateMap<SetSystemConfigCommand, SystemConfig>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.IsEncrypted, opt => opt.Ignore());
    }

    // Helper methods for calculated properties
    private static bool IsTrialExpired(Tenant tenant)
    {
        if (tenant.SubscriptionPlan != SubscriptionPlan.Trial)
            return false;

        var trialEnd = tenant.CreatedAt.AddDays(30); // 30-day trial
        return DateTime.UtcNow > trialEnd;
    }

    private static DateTime? CalculateTrialExpiry(Tenant tenant)
    {
        if (tenant.SubscriptionPlan != SubscriptionPlan.Trial)
            return null;

        return tenant.CreatedAt.AddDays(30); // 30-day trial
    }

    private static bool HasValidSession(User user)
    {
        // Simplified - would check actual session validity
        return user.Status == UserStatus.Active && 
               user.LastLoginAt.HasValue && 
               user.LastLoginAt.Value > DateTime.UtcNow.AddDays(-7);
    }
}
