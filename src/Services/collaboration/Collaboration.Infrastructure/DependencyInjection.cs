namespace Collaboration.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Collaboration.Domain.Repositories;
using Collaboration.Infrastructure.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddCollaborationInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICampaignParticipantRepository, CampaignParticipantRepository>();
        services.AddScoped<ICampaignAllocationRepository, CampaignAllocationRepository>();
        services.AddScoped<ISupplierQuotationRepository, SupplierQuotationRepository>();

        return services;
    }
}
