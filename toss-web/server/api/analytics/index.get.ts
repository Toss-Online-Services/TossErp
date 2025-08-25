import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // Gateway API URL - pointing to the gateway which routes to CRM service
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    
    try {
      // Try to fetch from CRM service via Gateway first
      const response = await $fetch(`${gatewayUrl}/api/crm/customers/analytics`);
      return response;
    } catch (crmError) {
      console.warn('CRM analytics not available via gateway, using mock data:', crmError);
      
      // Fallback to mock analytics data
      const mockAnalytics = {
        totalCustomers: 1247,
        activeLeads: 89,
        conversionRate: 24.5,
        pipelineValue: 450000,
        customerSegments: {
          regular: 456,
          silver: 387,
          gold: 267,
          premium: 137
        },
        monthlyGrowth: {
          customers: 12.5,
          revenue: 18.3,
          loyaltyPoints: 25.7
        },
        topPerformingRegions: [
          { name: 'Gauteng', customers: 523, revenue: 1250000 },
          { name: 'Western Cape', customers: 387, revenue: 980000 },
          { name: 'KwaZulu-Natal', customers: 234, revenue: 567000 },
          { name: 'Eastern Cape', customers: 103, revenue: 234000 }
        ]
      };
      
      return mockAnalytics;
    }
  } catch (error) {
    console.error('Error fetching analytics:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch analytics'
    });
  }
});
