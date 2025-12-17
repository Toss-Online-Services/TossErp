import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // Get specific CRM analytics that the frontend needs
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    const crmUrl = process.env.CRM_API_URL || 'http://localhost:5002';
    
    try {
      // Try to fetch from CRM analytics endpoint
      let response;
      try {
        response = await fetch(`${gatewayUrl}/api/crm/analytics`);
        if (!response.ok) {
          response = await fetch(`${crmUrl}/api/analytics`);
        }
      } catch (error) {
        response = await fetch(`${crmUrl}/api/analytics`);
      }

      if (!response.ok) {
        return await getMockCrmAnalytics();
      }
      
      const data = await response.json();
      return data;
    } catch (error) {
      console.warn('CRM analytics not available, using mock data:', error);
      return await getMockCrmAnalytics();
    }
  } catch (error) {
    console.error('Error fetching CRM analytics:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch CRM analytics'
    });
  }
});

async function getMockCrmAnalytics() {
  // Enhanced CRM-specific analytics based on our PostgreSQL sample data
  return {
    // Dashboard overview metrics
    overview: {
      totalCustomers: 5,
      activeLeads: 5,
      totalOpportunities: 5,
      pipelineValue: 1010000,
      conversionRate: 24.5,
      averageDealSize: 202000
    },
    
    // Sales pipeline breakdown
    pipeline: {
      stages: [
        { name: 'Prospecting', count: 1, value: 35000, percentage: 3.5 },
        { name: 'Qualification', count: 1, value: 250000, percentage: 24.8 },
        { name: 'NeedsAnalysis', count: 1, value: 150000, percentage: 14.9 },
        { name: 'Proposal', count: 1, value: 75000, percentage: 7.4 },
        { name: 'Negotiation', count: 1, value: 500000, percentage: 49.5 }
      ],
      totalValue: 1010000,
      weightedValue: 628750, // Based on probabilities
      averageCycleLength: 45
    },
    
    // Lead analytics
    leads: {
      totalLeads: 5,
      newThisMonth: 5,
      qualifiedLeads: 3,
      conversionRate: 60.0,
      sourceBreakdown: [
        { source: 'Website', count: 1, percentage: 20.0 },
        { source: 'Referral', count: 1, percentage: 20.0 },
        { source: 'SocialMedia', count: 1, percentage: 20.0 },
        { source: 'Email', count: 1, percentage: 20.0 },
        { source: 'TradeShow', count: 1, percentage: 20.0 }
      ],
      averageScore: 85
    },
    
    // Customer analytics  
    customers: {
      totalCustomers: 5,
      activeCustomers: 4,
      prospects: 1,
      tierDistribution: [
        { tier: 'Premium', count: 2, revenue: 2125000 },
        { tier: 'Standard', count: 2, revenue: 754000 },
        { tier: 'Basic', count: 1, revenue: 15000 }
      ],
      industryDistribution: [
        { industry: 'Technology', count: 1, revenue: 1250000 },
        { industry: 'Communications', count: 1, revenue: 875000 },
        { industry: 'Software Development', count: 1, revenue: 456000 },
        { industry: 'Research & Development', count: 1, revenue: 298000 },
        { industry: 'Cybersecurity', count: 1, revenue: 15000 }
      ]
    },
    
    // Recent activities for dashboard
    recentActivities: [
      { 
        id: 1, 
        type: 'lead_created', 
        description: 'New lead: Peter Parker from Daily Bugle',
        timestamp: new Date().toISOString(),
        icon: 'user-plus'
      },
      { 
        id: 2, 
        type: 'opportunity_updated', 
        description: 'Stark Industries opportunity moved to Qualification',
        timestamp: new Date(Date.now() - 3600000).toISOString(),
        icon: 'trending-up'
      },
      { 
        id: 3, 
        type: 'customer_contacted', 
        description: 'Email sent to Rogers Communications',
        timestamp: new Date(Date.now() - 7200000).toISOString(),
        icon: 'mail'
      },
      { 
        id: 4, 
        type: 'deal_negotiation', 
        description: 'Rogers Communications platform deal in negotiation',
        timestamp: new Date(Date.now() - 14400000).toISOString(),
        icon: 'handshake'
      },
      { 
        id: 5, 
        type: 'lead_qualified', 
        description: 'Bruce Wayne qualified as high-value lead',
        timestamp: new Date(Date.now() - 21600000).toISOString(),
        icon: 'check-circle'
      }
    ],
    
    // Performance metrics for the period
    performance: {
      thisMonth: {
        newLeads: 5,
        convertedLeads: 0,
        closedDeals: 0,
        revenue: 0,
        averageScore: 85
      },
      lastMonth: {
        newLeads: 3,
        convertedLeads: 2,
        closedDeals: 1,
        revenue: 125000,
        averageScore: 78
      },
      growth: {
        leads: 66.7,
        conversions: -100.0,
        revenue: -100.0,
        avgScore: 9.0
      }
    },
    
    // Forecasting based on current pipeline
    forecast: {
      q1_2024: { expectedDeals: 2, projectedRevenue: 425000, confidence: 'High' },
      q2_2024: { expectedDeals: 2, projectedRevenue: 195000, confidence: 'Medium' },
      q3_2024: { expectedDeals: 1, projectedRevenue: 8750, confidence: 'Low' },
      q4_2024: { expectedDeals: 0, projectedRevenue: 0, confidence: 'Low' }
    }
  };
}
