export default defineEventHandler(async (event) => {
  try {
    const body = await readBody(event)
    
    // Proxy the request to the Gateway
    const response = await fetch('http://localhost:8080/api/crm/customers', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(body)
    })
    
    if (!response.ok) {
      throw createError({
        statusCode: response.status,
        statusMessage: 'Failed to create customer'
      })
    }
    
    const data = await response.json()
    return data
  } catch (error) {
    console.error('Error creating customer:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to create customer'
    })
  }
})
