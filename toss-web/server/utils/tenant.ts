// Multi-tenant utility functions and middleware
import type { H3Event } from 'h3'
import { getHeader, getCookie, createError } from 'h3'

export interface TenantContext {
  tenantId: string
  tenantName: string
  subscription: {
    plan: 'basic' | 'professional' | 'enterprise'
    status: 'active' | 'suspended' | 'cancelled'
    features: string[]
    usageLimits: {
      users: number
      storage: number // GB
      apiCalls: number // per month
      aiOperations: number // per month
    }
  }
  settings: {
    timezone: string
    currency: string
    language: string
    businessType: string
    customization: Record<string, any>
  }
}

export interface ServiceContext {
  tenantId: string
  userId: string
  userRole: string
  requestId: string
  timestamp: Date
}

// Extract tenant context from request
export async function getTenantContext(event: H3Event): Promise<TenantContext | null> {
  try {
    // Get tenant from subdomain, header, or JWT token
    let tenantId = getHeader(event, 'x-tenant-id') || 
                   getCookie(event, 'tenant-id') ||
                   extractTenantFromSubdomain(event)

    // If not found, try to extract from JWT token
    if (!tenantId) {
      const token = getCookie(event, 'auth-token') || getHeader(event, 'authorization')?.replace('Bearer ', '')
      if (token) {
        try {
          // For demo purposes, assume token contains tenant info in a simple format
          // In production, you would properly decode JWT here
          const parts = token.split('.')
          if (parts.length > 1) {
            // Simple extraction without JWT library for demo
            tenantId = 'demo-salon' // fallback for demo
          }
        } catch (error) {
          console.warn('Failed to decode token for tenant extraction:', error)
        }
      }
    }

    if (!tenantId) return null

    // Load tenant configuration from database
    const tenant = await loadTenantConfig(tenantId)
    return tenant
  } catch (error) {
    console.error('Error getting tenant context:', error)
    return null
  }
}

// Extract tenant from subdomain (e.g., client1.toss.co.za)
function extractTenantFromSubdomain(event: H3Event): string | null {
  const host = getHeader(event, 'host')
  if (!host) return null

  const parts = host.split('.')
  if (parts.length >= 3 && !['www', 'api', 'admin'].includes(parts[0])) {
    return parts[0]
  }
  return null
}

// Load tenant configuration from database/cache
async function loadTenantConfig(tenantId: string): Promise<TenantContext | null> {
  // TODO: Replace with actual database query
  // This would typically load from Redis cache first, then database
  
  // Demo tenant configurations
  const demoTenants: Record<string, TenantContext> = {
    'demo-salon': {
      tenantId: 'demo-salon',
      tenantName: 'Beautiful Hair Salon',
      subscription: {
        plan: 'professional',
        status: 'active',
        features: ['inventory', 'sales', 'crm', 'analytics', 'ai-assistant'],
        usageLimits: {
          users: 10,
          storage: 50,
          apiCalls: 10000,
          aiOperations: 1000
        }
      },
      settings: {
        timezone: 'Africa/Johannesburg',
        currency: 'ZAR',
        language: 'en',
        businessType: 'salon',
        customization: {
          brandColor: '#8B5CF6',
          features: {
            appointmentBooking: true,
            productCatalog: true,
            loyaltyProgram: true
          }
        }
      }
    },
    'demo-shop': {
      tenantId: 'demo-shop',
      tenantName: 'Township Spaza Shop',
      subscription: {
        plan: 'basic',
        status: 'active',
        features: ['inventory', 'sales', 'basic-analytics'],
        usageLimits: {
          users: 3,
          storage: 10,
          apiCalls: 2000,
          aiOperations: 100
        }
      },
      settings: {
        timezone: 'Africa/Johannesburg',
        currency: 'ZAR',
        language: 'en',
        businessType: 'retail',
        customization: {
          brandColor: '#10B981',
          features: {
            barcodescanning: true,
            creditSales: true,
            mobilePOS: true
          }
        }
      }
    }
  }

  return demoTenants[tenantId] || null
}

// Middleware to ensure tenant context
export async function requireTenant(event: H3Event): Promise<TenantContext> {
  const tenant = await getTenantContext(event)
  if (!tenant) {
    throw createError({
      statusCode: 401,
      statusMessage: 'Tenant context required'
    })
  }
  return tenant
}

// Check if tenant has access to a specific feature
export function hasFeature(tenant: TenantContext, feature: string): boolean {
  return tenant.subscription.features.includes(feature)
}

// Check usage limits
export function checkUsageLimit(tenant: TenantContext, limitType: keyof TenantContext['subscription']['usageLimits'], currentUsage: number): boolean {
  const limit = tenant.subscription.usageLimits[limitType]
  return currentUsage < limit
}

// Create service context for tracking
export function createServiceContext(tenant: TenantContext, userId: string, userRole: string): ServiceContext {
  return {
    tenantId: tenant.tenantId,
    userId,
    userRole,
    requestId: generateRequestId(),
    timestamp: new Date()
  }
}

function generateRequestId(): string {
  return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
}

// Tenant data isolation utility
export function addTenantFilter(query: any, tenantId: string, table?: string): any {
  const prefix = table ? `${table}.` : ''
  return {
    ...query,
    [`${prefix}tenant_id`]: tenantId
  }
}

// Service usage tracking
export async function trackServiceUsage(context: ServiceContext, service: string, operation: string, metadata?: any) {
  // TODO: Implement usage tracking to database/analytics
  console.log('Service Usage:', {
    tenantId: context.tenantId,
    userId: context.userId,
    service,
    operation,
    timestamp: context.timestamp,
    requestId: context.requestId,
    metadata
  })
}
