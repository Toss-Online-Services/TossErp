export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  
  try {
    // Proxy the request to the Gateway
    const response = await fetch('http://localhost:8080/api/crm/customers')
    const data = await response.json()
    
    return data
  } catch (error) {
    console.error('Error fetching customers:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch customers'
    })
  }
})
