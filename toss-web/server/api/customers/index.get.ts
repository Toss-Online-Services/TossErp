import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // Gateway API URL - pointing to the gateway which routes to CRM service
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    
    try {
      // Try to fetch from CRM service via Gateway first
      const response = await $fetch(`${gatewayUrl}/api/crm/customers`);
      return response;
    } catch (crmError) {
      console.warn('CRM service not available via gateway, using mock data:', crmError);
      
      // Fallback to mock data if CRM service is not available
      const mockCustomers = [
        {
          id: '1',
          firstName: 'John',
          lastName: 'Smith',
          fullName: 'John Smith',
          email: 'john@abccorp.co.za',
          phone: '+27 11 123 4567',
          address: '123 Business St, Johannesburg',
          dateOfBirth: '1980-01-15',
          status: 'Active',
          segment: 'Gold',
          createdAt: '2024-01-01T00:00:00Z',
          lastPurchaseDate: '2024-08-15T00:00:00Z',
          totalSpent: 125000,
          purchaseCount: 15,
          loyaltyPoints: 2500,
          isLapsed: false,
          isHighValue: true
        },
        {
          id: '2',
          firstName: 'Sarah',
          lastName: 'Johnson',
          fullName: 'Sarah Johnson',
          email: 'sarah@xyzltd.co.za',
          phone: '+27 21 987 6543',
          address: '456 Commerce Ave, Cape Town',
          dateOfBirth: '1985-06-22',
          status: 'Active',
          segment: 'Silver',
          createdAt: '2024-02-15T00:00:00Z',
          lastPurchaseDate: '2024-08-20T00:00:00Z',
          totalSpent: 45000,
          purchaseCount: 8,
          loyaltyPoints: 900,
          isLapsed: false,
          isHighValue: false
        },
        {
          id: '3',
          firstName: 'Mike',
          lastName: 'Wilson',
          fullName: 'Mike Wilson',
          email: 'mike@techsol.co.za',
          phone: '+27 31 555 7890',
          address: '789 Tech Park, Durban',
          dateOfBirth: '1978-11-08',
          status: 'Active',
          segment: 'Premium',
          createdAt: '2024-03-01T00:00:00Z',
          lastPurchaseDate: '2024-08-25T00:00:00Z',
          totalSpent: 85000,
          purchaseCount: 12,
          loyaltyPoints: 1700,
          isLapsed: false,
          isHighValue: true
        }
      ];

      return mockCustomers;
    }
  } catch (error) {
    console.error('Error fetching customers:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch customers'
    });
  }
});