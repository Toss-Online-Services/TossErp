/**
 * Catch-all API proxy route
 * Forwards all /api/* requests to the backend API
 * This avoids CORS and SSL certificate issues in development
 */
export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const backendURL = config.public.apiBase || 'https://localhost:5001'
  
  // Get the full path after /api/
  const path = event.path.replace(/^\/api\//, '')
  
  // Get query parameters
  const query = getQuery(event)
  
  // Build the target URL
  const targetURL = `${backendURL}/api/${path}`
  
  try {
    // Forward the request to the backend
    const response = await $fetch(targetURL, {
      method: event.method,
      query: query,
      body: event.method !== 'GET' && event.method !== 'HEAD' ? await readBody(event) : undefined,
      headers: {
        // Forward relevant headers
        'Content-Type': getHeader(event, 'content-type') || 'application/json',
        'Accept': getHeader(event, 'accept') || 'application/json'
      }
    })
    
    return response
  } catch (error: any) {
    console.error('API Proxy Error:', error)
    throw createError({
      statusCode: error.statusCode || 500,
      message: error.message || 'Backend API request failed'
    })
  }
})