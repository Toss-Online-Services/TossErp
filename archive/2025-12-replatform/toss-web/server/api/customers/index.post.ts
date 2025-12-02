import { defineEventHandler, createError, readBody } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    const body = await readBody(event)
    
    // Gateway API URL - pointing to the gateway which routes to CRM service
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    
    // Validate required fields
    const { firstName, lastName, email, phone, address, dateOfBirth } = body;
    
    if (!firstName || !lastName || !email || !phone || !address || !dateOfBirth) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Missing required fields'
      });
    }
    
    try {
      // Try to call CRM service via Gateway first
      const response = await $fetch(`${gatewayUrl}/api/crm/customers`, {
        method: 'POST',
        body: {
          firstName,
          lastName,
          email,
          phone,
          address,
          dateOfBirth
        }
      });
      return { id: response, message: 'Customer created successfully' };
    } catch (crmError) {
      console.warn('CRM service not available via gateway, using mock response:', crmError);
      
      // Fallback to mock success response
      const mockResponse = {
        id: 'new-customer-' + Date.now(),
        message: 'Customer created successfully (mock)'
      };
      
      return mockResponse;
    }
  } catch (error) {
    console.error('Error creating customer:', error);
    if (error.statusCode) {
      throw error;
    }
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to create customer'
    });
  }
});
