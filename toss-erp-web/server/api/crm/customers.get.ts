import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // For now, return mock data based on our PostgreSQL sample data
    // This ensures fast response times while we work on the backend integration
    return await getMockCustomers();
  } catch (error) {
    console.error('Error fetching customers:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'An error occurred while retrieving customers'
    });
  }
});

async function getMockCustomers() {
  // Mock data based on our PostgreSQL sample data structure
  return [
    {
      id: "11111111-1111-1111-1111-111111111111",
      name: "Stark Industries",
      fullName: "Stark Industries",
      type: "Enterprise",
      status: "Active",
      tier: "Premium", 
      industry: "Technology",
      employeeCount: 1000,
      email: "contact@starkindustries.com",
      phone: "+1-555-0199",
      address: "200 Park Avenue, New York, NY 10003",
      website: "https://starkindustries.com",
      subscriptionStatus: "Active",
      totalSpent: 1250000,
      lastPurchaseDate: "2024-01-15T10:30:00Z",
      segment: "Premium",
      purchaseCount: 25,
      loyaltyPoints: 12500,
      createdAt: "2024-01-01T00:00:00Z",
      isLapsed: false
    },
    {
      id: "22222222-2222-2222-2222-222222222222", 
      name: "Parker Technologies",
      fullName: "Parker Technologies",
      type: "Enterprise",
      status: "Active",
      tier: "Standard",
      industry: "Software Development", 
      employeeCount: 150,
      email: "admin@parkertech.com",
      phone: "+1-555-0188",
      address: "123 Tech Drive, Silicon Valley, CA 94025",
      website: "https://parkertech.com",
      subscriptionStatus: "Active",
      totalSpent: 456000,
      lastPurchaseDate: "2024-01-10T14:20:00Z",
      segment: "Gold",
      purchaseCount: 18,
      loyaltyPoints: 4560,
      createdAt: "2024-01-01T00:00:00Z",
      isLapsed: false
    },
    {
      id: "33333333-3333-3333-3333-333333333333",
      name: "Banner Labs",
      fullName: "Banner Labs", 
      type: "Enterprise",
      status: "Active",
      tier: "Standard",
      industry: "Research & Development",
      employeeCount: 75,
      email: "info@bannerlabs.com",
      phone: "+1-555-0177", 
      address: "456 Science Way, Boston, MA 02101",
      website: "https://bannerlabs.com",
      subscriptionStatus: "Active",
      totalSpent: 298000,
      lastPurchaseDate: "2024-01-08T09:15:00Z",
      segment: "Silver",
      purchaseCount: 12,
      loyaltyPoints: 2980,
      createdAt: "2024-01-01T00:00:00Z",
      isLapsed: false
    },
    {
      id: "44444444-4444-4444-4444-444444444444",
      name: "Rogers Communications",
      fullName: "Rogers Communications",
      type: "Enterprise", 
      status: "Active",
      tier: "Premium",
      industry: "Communications",
      employeeCount: 500,
      email: "business@rogers.com",
      phone: "+1-555-0166",
      address: "789 Communication Blvd, Toronto, ON M5V 3A8",
      website: "https://rogers.com",
      subscriptionStatus: "Active", 
      totalSpent: 875000,
      lastPurchaseDate: "2024-01-12T16:45:00Z",
      segment: "Premium",
      purchaseCount: 22,
      loyaltyPoints: 8750,
      createdAt: "2024-01-01T00:00:00Z",
      isLapsed: false
    },
    {
      id: "55555555-5555-5555-5555-555555555555",
      name: "Romanoff Security",
      fullName: "Romanoff Security",
      type: "SmallBusiness",
      status: "Prospect",
      tier: "Basic",
      industry: "Cybersecurity",
      employeeCount: 25,
      email: "contact@romanoffsec.com", 
      phone: "+1-555-0155",
      address: "321 Security Lane, Washington, DC 20001",
      website: "https://romanoffsec.com",
      subscriptionStatus: "Trial",
      totalSpent: 15000,
      lastPurchaseDate: "2023-12-20T11:30:00Z",
      segment: "Regular", 
      purchaseCount: 3,
      loyaltyPoints: 150,
      createdAt: "2024-01-01T00:00:00Z",
      isLapsed: false
    }
  ];
}
