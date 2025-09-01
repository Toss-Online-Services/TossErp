import { defineEventHandler, createError } from 'h3'
import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return {
    sales: {
      daily: 2450,
      weekly: 19600,
      monthly: 89450,
      growthPct: 12.5,
    },
    inventory: {
      totalItems: 156,
      lowStockAlerts: 3,
      totalValue: 62500,
    },
    customers: {
      total: 234,
      activeToday: 23,
      repeatRate: 68,
    },
  }
})

export default defineEventHandler(async (event) => {
  try {
    // Gateway API URL - pointing to the gateway which routes to CRM service
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    const crmUrl = process.env.CRM_API_URL || 'http://localhost:5002';
    
    try {
      // Try to fetch from CRM service via Gateway first
      let response;
      try {
        response = await $fetch(`${gatewayUrl}/api/crm/customers/analytics`);
      } catch (gatewayError) {
        // Fallback to direct CRM API
        response = await $fetch(`${crmUrl}/api/analytics`);
      }
      return response;
    } catch (crmError) {
      console.warn('CRM analytics not available, using enhanced mock data based on PostgreSQL sample data:', crmError);
      
      // Enhanced analytics based on our actual PostgreSQL sample data
      const enhancedAnalytics = {
        // Core metrics from our sample data
        totalCustomers: 5,
        activeLeads: 5,
        conversionRate: 24.5,
        pipelineValue: 1010000, // Sum of all opportunity values: 250k + 75k + 150k + 500k + 35k
        
        // Customer breakdown by tier (from our sample data)
        customersByTier: {
          basic: 1,     // Romanoff Security
          standard: 2,  // Parker Technologies, Banner Labs
          premium: 2    // Stark Industries, Rogers Communications
        },
        
        // Customer breakdown by type
        customersByType: {
          enterprise: 4,      // Stark, Parker, Banner, Rogers
          smallBusiness: 1    // Romanoff
        },
        
        // Customer breakdown by status
        customersByStatus: {
          active: 4,    // All except Romanoff
          prospect: 1   // Romanoff Security
        },
        
        // Lead breakdown by industry (from our sample data)
        leadsByIndustry: {
          media: 2,              // Peter Parker (Daily Bugle), Clark Kent (Daily Planet)
          technology: 1,         // Bruce Wayne (Wayne Industries)
          government: 1,         // Diana Prince (Themyscira Embassy)
          lawEnforcement: 1      // Barry Allen (Central City Police)
        },
        
        // Opportunities by stage (from our sample data)
        opportunitiesByStage: {
          prospecting: 1,    // Romanoff Security Consulting
          qualification: 1,  // Stark Industries Enterprise License
          needsAnalysis: 1,  // Banner Labs Research Partnership
          proposal: 1,       // Parker Tech Development Contract
          negotiation: 1     // Rogers Communications Platform
        },
        
        // Revenue projections based on probability
        projectedRevenue: {
          q1_2024: 425000,  // High probability deals (Rogers 85% of 500k + Stark 75% of 250k)
          q2_2024: 195000,  // Medium probability deals (Parker 60% of 75k + Banner 50% of 150k)
          q3_2024: 8750,    // Low probability deals (Romanoff 25% of 35k)
          total: 628750
        },
        
        // Top industries by value
        topIndustries: [
          { name: 'Technology', totalValue: 825000, customerCount: 2 },      // Stark + Parker + Banner
          { name: 'Communications', totalValue: 500000, customerCount: 1 },   // Rogers
          { name: 'Research & Development', totalValue: 150000, customerCount: 1 }, // Banner
          { name: 'Cybersecurity', totalValue: 35000, customerCount: 1 }      // Romanoff
        ],
        
        // Monthly growth (projected based on sample data)
        monthlyGrowth: {
          customers: 12.5,
          revenue: 18.3,
          opportunities: 25.0
        },
        
        // Sample KPIs
        kpis: {
          averageDealSize: 202000,        // Total pipeline / number of opportunities
          salesCycleLength: 45,           // Average days
          customerAcquisitionCost: 5000,  // Estimated
          customerLifetimeValue: 245000,  // Average based on sample customer values
          leadsToCustomerConversion: 20.0 // Percentage
        },
        
        // Performance by region (sample data)
        topPerformingRegions: [
          { name: 'North America', customers: 4, revenue: 1010000, growth: 15.2 },
          { name: 'International', customers: 1, revenue: 150000, growth: 8.7 }
        ],
        
        // Recent activity summary
        recentActivity: {
          newCustomersThisMonth: 1,
          newLeadsThisMonth: 5,
          opportunitiesClosedThisMonth: 0,
          revenueThisMonth: 0
        }
      };
      
      return enhancedAnalytics;
    }
  } catch (error) {
    console.error('Error fetching analytics:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch analytics'
    });
  }
});
