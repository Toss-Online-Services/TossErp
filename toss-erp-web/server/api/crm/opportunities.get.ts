import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // Return mock data based on our PostgreSQL sample data for fast response
    return await getMockOpportunities();
  } catch (error) {
    console.error('Error fetching opportunities:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'An error occurred while retrieving opportunities'
    });
  }
});

async function getMockOpportunities() {
  // Mock data based on our PostgreSQL sample data
  return [
    {
      id: "ffffffff-ffff-ffff-ffff-ffffffffffff",
      name: "Stark Industries Enterprise License",
      customerId: "11111111-1111-1111-1111-111111111111",
      customerName: "Stark Industries",
      stage: "Qualification",
      type: "NewBusiness", 
      priority: "High",
      expectedCloseDate: "2024-06-01T00:00:00Z",
      estimatedValueAmount: 250000,
      estimatedValueCurrency: "USD",
      probability: 75,
      description: "Technology licensing deal",
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "11111111-2222-3333-4444-555555555555",
      name: "Parker Tech Development Contract",
      customerId: "22222222-2222-2222-2222-222222222222",
      customerName: "Parker Technologies",
      stage: "Proposal",
      type: "NewBusiness",
      priority: "Medium", 
      expectedCloseDate: "2024-04-01T00:00:00Z",
      estimatedValueAmount: 75000,
      estimatedValueCurrency: "USD",
      probability: 60,
      description: "Custom software development",
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "22222222-3333-4444-5555-666666666666",
      name: "Banner Labs Research Partnership",
      customerId: "33333333-3333-3333-3333-333333333333",
      customerName: "Banner Labs",
      stage: "NeedsAnalysis",
      type: "Upsell",
      priority: "Medium",
      expectedCloseDate: "2024-05-01T00:00:00Z", 
      estimatedValueAmount: 150000,
      estimatedValueCurrency: "USD",
      probability: 50,
      description: "Research collaboration agreement",
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "33333333-4444-5555-6666-777777777777",
      name: "Rogers Communications Platform",
      customerId: "44444444-4444-4444-4444-444444444444",
      customerName: "Rogers Communications",
      stage: "Negotiation",
      type: "NewBusiness",
      priority: "High",
      expectedCloseDate: "2024-03-01T00:00:00Z",
      estimatedValueAmount: 500000,
      estimatedValueCurrency: "USD", 
      probability: 85,
      description: "Communication platform deployment",
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "44444444-5555-6666-7777-888888888888",
      name: "Romanoff Security Consulting",
      customerId: "55555555-5555-5555-5555-555555555555",
      customerName: "Romanoff Security",
      stage: "Prospecting",
      type: "NewBusiness",
      priority: "Low",
      expectedCloseDate: "2024-07-01T00:00:00Z",
      estimatedValueAmount: 35000,
      estimatedValueCurrency: "USD",
      probability: 25,
      description: "Security audit and consulting",
      createdAt: new Date().toISOString(),
      isDeleted: false
    }
  ];
}
