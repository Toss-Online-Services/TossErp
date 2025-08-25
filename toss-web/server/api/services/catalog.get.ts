// Service Catalog API - List available services for tenant
import { requireTenant } from '~/server/utils/tenant'
import { SERVICE_REGISTRY } from '~/server/utils/services'

export default defineEventHandler(async (event) => {
  const tenant = await requireTenant(event)
  
  // Filter services based on tenant's subscription features
  const availableServices = Object.entries(SERVICE_REGISTRY)
    .filter(([_, service]) => 
      service.requirements.every(req => tenant.subscription.features.includes(req))
    )
    .map(([key, service]) => ({
      id: key,
      name: service.name,
      description: service.description,
      category: service.category,
      outcomes: service.outcomes,
      pricing: service.pricing,
      requirements: service.requirements,
      // Add tenant-specific pricing calculations
      estimatedCost: calculateEstimatedCost(service, tenant),
      estimatedROI: calculateEstimatedROI(service, tenant)
    }))

  return {
    success: true,
    data: {
      tenant: {
        id: tenant.tenantId,
        name: tenant.tenantName,
        plan: tenant.subscription.plan,
        businessType: tenant.settings.businessType
      },
      services: availableServices,
      totalServices: availableServices.length,
      categories: [...new Set(availableServices.map(s => s.category))],
      summary: {
        totalPotentialSavings: availableServices.reduce((sum, s) => sum + (s.estimatedROI || 0), 0),
        averageROI: availableServices.reduce((sum, s) => sum + (s.estimatedROI || 0), 0) / availableServices.length
      }
    }
  }
})

function calculateEstimatedCost(service: any, tenant: any): number {
  switch (service.pricing.model) {
    case 'usage':
      // Estimate based on business size
      const businessSizeMultiplier = tenant.subscription.plan === 'enterprise' ? 3 : 
                                   tenant.subscription.plan === 'professional' ? 2 : 1
      return businessSizeMultiplier * 100 // Base estimation
    
    case 'outcome':
      // Estimate based on historical performance
      return 250 // Average outcome-based pricing
    
    case 'subscription':
      return service.pricing.basePrice || 299
    
    default:
      return 199
  }
}

function calculateEstimatedROI(service: any, tenant: any): number {
  // Business-specific ROI calculations
  const businessMultipliers = {
    'salon': { 'inventory-management': 1200, 'sales-automation': 2500, 'customer-engagement': 1800 },
    'retail': { 'inventory-management': 3500, 'sales-automation': 4200, 'financial-intelligence': 1500 },
    'restaurant': { 'inventory-management': 2800, 'customer-engagement': 2200, 'sales-automation': 3100 }
  }
  
  const businessType = tenant.settings.businessType || 'retail'
  const serviceMap = businessMultipliers[businessType] || businessMultipliers['retail']
  
  return serviceMap[service.name.toLowerCase().replace(/[^a-z]/g, '-')] || 1000
}
