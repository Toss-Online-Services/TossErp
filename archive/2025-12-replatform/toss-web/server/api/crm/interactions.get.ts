import { defineEventHandler, readBody, getMethod, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const method = getMethod(event);
  
  try {
    const gatewayUrl = process.env.GATEWAY_URL || 'http://localhost:8081';
    const crmUrl = process.env.CRM_API_URL || 'http://localhost:5002';
    
    if (method === 'GET') {
      // Get all interactions
      try {
        let response;
        try {
          response = await fetch(`${gatewayUrl}/api/crm/interactions`);
          if (!response.ok) {
            response = await fetch(`${crmUrl}/api/interactions`);
          }
        } catch (error) {
          response = await fetch(`${crmUrl}/api/interactions`);
        }

        if (!response.ok) {
          return getMockInteractions();
        }
        
        const data = await response.json();
        return data;
      } catch (error) {
        console.warn('CRM interactions not available, using mock data:', error);
        return getMockInteractions();
      }
    } else if (method === 'POST') {
      // Create new interaction
      const body = await readBody(event);
      
      try {
        let response;
        try {
          response = await fetch(`${gatewayUrl}/api/crm/interactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
          });
          if (!response.ok) {
            response = await fetch(`${crmUrl}/api/interactions`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(body)
            });
          }
        } catch (error) {
          response = await fetch(`${crmUrl}/api/interactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
          });
        }

        if (!response.ok) {
          return createMockInteraction(body);
        }
        
        const data = await response.json();
        return data;
      } catch (error) {
        console.warn('CRM interaction creation failed, creating mock:', error);
        return createMockInteraction(body);
      }
    } else {
      throw createError({
        statusCode: 405,
        statusMessage: 'Method not allowed'
      });
    }
  } catch (error) {
    console.error('Error handling interactions:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to handle interactions'
    });
  }
});

function getMockInteractions() {
  // Mock interactions based on our Marvel-themed sample data
  return [
    {
      id: 1,
      customerId: 1, // Stark Industries
      leadId: null,
      type: 'Phone Call',
      subject: 'Technology Partnership Discussion',
      notes: 'Discussed potential collaboration on advanced manufacturing processes. Tony interested in upgrading their systems.',
      outcome: 'Positive',
      nextAction: 'Send detailed proposal',
      scheduledDate: new Date().toISOString(),
      completedDate: new Date().toISOString(),
      duration: 45,
      createdAt: new Date(Date.now() - 86400000).toISOString(),
      updatedAt: new Date(Date.now() - 86400000).toISOString()
    },
    {
      id: 2,
      customerId: null,
      leadId: 1, // Peter Parker
      type: 'Email',
      subject: 'Initial Contact - Software Solutions',
      notes: 'Sent introduction email about our software development services. Peter is looking for web presence solutions.',
      outcome: 'Pending',
      nextAction: 'Follow up call scheduled',
      scheduledDate: new Date(Date.now() + 86400000).toISOString(),
      completedDate: null,
      duration: null,
      createdAt: new Date(Date.now() - 43200000).toISOString(),
      updatedAt: new Date(Date.now() - 43200000).toISOString()
    },
    {
      id: 3,
      customerId: 2, // Parker Technologies
      leadId: null,
      type: 'Meeting',
      subject: 'Project Requirements Review',
      notes: 'In-person meeting to review technical requirements for their new platform. Very detailed discussion about scalability needs.',
      outcome: 'Positive',
      nextAction: 'Prepare technical architecture proposal',
      scheduledDate: new Date(Date.now() - 172800000).toISOString(),
      completedDate: new Date(Date.now() - 172800000).toISOString(),
      duration: 120,
      createdAt: new Date(Date.now() - 172800000).toISOString(),
      updatedAt: new Date(Date.now() - 172800000).toISOString()
    },
    {
      id: 4,
      customerId: 4, // Rogers Communications
      leadId: null,
      type: 'Video Call',
      subject: 'Quarterly Business Review',
      notes: 'Reviewed progress on current initiatives. Steve very satisfied with our communication platform improvements.',
      outcome: 'Positive',
      nextAction: 'Schedule next phase planning',
      scheduledDate: new Date(Date.now() - 259200000).toISOString(),
      completedDate: new Date(Date.now() - 259200000).toISOString(),
      duration: 60,
      createdAt: new Date(Date.now() - 259200000).toISOString(),
      updatedAt: new Date(Date.now() - 259200000).toISOString()
    },
    {
      id: 5,
      customerId: null,
      leadId: 3, // Bruce Wayne
      type: 'Phone Call',
      subject: 'Security Consultation',
      notes: 'Initial consultation about cybersecurity needs. Bruce is very interested in our enterprise security solutions.',
      outcome: 'Positive',
      nextAction: 'Security assessment proposal',
      scheduledDate: new Date().toISOString(),
      completedDate: new Date().toISOString(),
      duration: 30,
      createdAt: new Date(Date.now() - 3600000).toISOString(),
      updatedAt: new Date(Date.now() - 3600000).toISOString()
    }
  ];
}

function createMockInteraction(data) {
  // Create a new mock interaction
  return {
    id: Math.floor(Math.random() * 10000),
    customerId: data.customerId || null,
    leadId: data.leadId || null,
    type: data.type || 'Phone Call',
    subject: data.subject || 'New Interaction',
    notes: data.notes || '',
    outcome: data.outcome || 'Pending',
    nextAction: data.nextAction || '',
    scheduledDate: data.scheduledDate || new Date().toISOString(),
    completedDate: data.completedDate || null,
    duration: data.duration || null,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  };
}
