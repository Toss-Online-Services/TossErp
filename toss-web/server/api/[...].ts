/**
 * Catch-all API proxy route
 * Forwards all /api/* requests to the backend API
 * This avoids CORS and SSL certificate issues in development
 */
export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const backendURL = config.public.apiBase
  
  // Get the full path after /api/
  const path = event.path.replace(/^\/api\//, '')
  
  // Get query parameters
  const query = getQuery(event)
  
  // Build the target URL
  const targetURL = `${backendURL}/api/${path}`

  const isPrematureClose = (err: unknown) => {
    const anyErr = err as { code?: string; message?: string } | null
    const message = anyErr?.message ?? ''
    return anyErr?.code === 'ERR_STREAM_PREMATURE_CLOSE' || /premature close/i.test(message)
  }

  const isGetLike = event.method === 'GET' || event.method === 'HEAD'
  
  try {
    let body: unknown

    if (!isGetLike) {
      try {
        body = await readBody(event)
      } catch (err) {
        // If the client closed the connection while we were reading the body,
        // don't crash the dev server by throwing an unhandled error.
        if (event.node.req.aborted || isPrematureClose(err)) {
          return
        }

        throw err
      }
    }

    // Forward the request to the backend
    const response = await $fetch(targetURL, {
      method: event.method,
      query: query,
      body: isGetLike ? undefined : body,
      headers: {
        // Forward relevant headers
        'Content-Type': getHeader(event, 'content-type') || 'application/json',
        'Accept': getHeader(event, 'accept') || 'application/json'
      }
    })
    
    return response
  } catch (error: any) {
    // Aborted requests can throw stream errors like "Premature close".
    // These should not bring down the dev server.
    if (event.node.req.aborted || isPrematureClose(error)) {
      return
    }

    console.error('API Proxy Error:', error)
    throw createError({
      statusCode: error.statusCode || 500,
      message: error.message || 'Backend API request failed'
    })
  }
})