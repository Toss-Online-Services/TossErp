// Error handling utility for consistent error handling across the application

export interface AppError {
  message: string
  code?: string
  details?: any
  timestamp: Date
  userFriendly?: boolean
}

export interface ErrorResponse {
  success: false
  error: AppError
}

export interface SuccessResponse<T> {
  success: true
  data: T
  message?: string
}

export type ApiResponse<T> = SuccessResponse<T> | ErrorResponse

// Error types
export enum ErrorType {
  VALIDATION = 'VALIDATION',
  NETWORK = 'NETWORK',
  AUTHENTICATION = 'AUTHENTICATION',
  AUTHORIZATION = 'AUTHORIZATION',
  NOT_FOUND = 'NOT_FOUND',
  SERVER_ERROR = 'SERVER_ERROR',
  UNKNOWN = 'UNKNOWN'
}

// Create application errors
export function createError(
  message: string,
  code?: string,
  details?: any,
  userFriendly: boolean = true
): AppError {
  return {
    message,
    code,
    details,
    timestamp: new Date(),
    userFriendly
  }
}

// Create validation error
export function createValidationError(message: string, field?: string): AppError {
  return createError(
    message,
    ErrorType.VALIDATION,
    { field },
    true
  )
}

// Create network error
export function createNetworkError(message: string, details?: any): AppError {
  return createError(
    message,
    ErrorType.NETWORK,
    details,
    true
  )
}

// Create authentication error
export function createAuthError(message: string): AppError {
  return createError(
    message,
    ErrorType.AUTHENTICATION,
    undefined,
    true
  )
}

// Create server error
export function createServerError(message: string, details?: any): AppError {
  return createError(
    message,
    ErrorType.SERVER_ERROR,
    details,
    false
  )
}

// Handle API errors
export function handleApiError(error: any): AppError {
  if (error instanceof Error) {
    // Network or fetch errors
    if (error.name === 'AbortError') {
      return createNetworkError('Request was cancelled or timed out')
    }
    
    if (error.message.includes('Failed to fetch')) {
      return createNetworkError('Unable to connect to the server. Please check your internet connection.')
    }
    
    if (error.message.includes('timeout')) {
      return createNetworkError('Request timed out. Please try again.')
    }
    
    return createError(error.message, ErrorType.UNKNOWN, undefined, true)
  }
  
  // HTTP response errors
  if (error.statusCode) {
    switch (error.statusCode) {
      case 400:
        return createValidationError(error.message || 'Invalid request data')
      case 401:
        return createAuthError('Authentication required. Please log in again.')
      case 403:
        return createError('Access denied. You do not have permission to perform this action.', ErrorType.AUTHORIZATION)
      case 404:
        return createError('The requested resource was not found.', ErrorType.NOT_FOUND)
      case 422:
        return createValidationError(error.message || 'Validation failed')
      case 429:
        return createError('Too many requests. Please try again later.', ErrorType.NETWORK)
      case 500:
        return createServerError('Internal server error. Please try again later.')
      case 502:
      case 503:
      case 504:
        return createNetworkError('Service temporarily unavailable. Please try again later.')
      default:
        return createError(
          error.message || 'An unexpected error occurred',
          ErrorType.UNKNOWN,
          error
        )
    }
  }
  
  // Unknown error
  return createError(
    'An unexpected error occurred. Please try again.',
    ErrorType.UNKNOWN,
    error,
    true
  )
}

// Format error for display
export function formatError(error: AppError): string {
  if (error.userFriendly) {
    return error.message
  }
  
  // For non-user-friendly errors, provide a generic message
  return 'An unexpected error occurred. Please try again or contact support if the problem persists.'
}

// Log error for debugging
export function logError(error: AppError, context?: string): void {
  const logData = {
    message: error.message,
    code: error.code,
    details: error.details,
    timestamp: error.timestamp.toISOString(),
    context,
    userAgent: navigator.userAgent,
    url: window.location.href
  }
  
  if (process.env.NODE_ENV === 'development') {
    console.error('Application Error:', logData)
  } else {
    // In production, you might want to send this to a logging service
    console.error('Error occurred:', error.message)
  }
}

// Retry logic for failed requests
export interface RetryOptions {
  maxAttempts: number
  delay: number
  backoffMultiplier: number
  maxDelay: number
}

export const defaultRetryOptions: RetryOptions = {
  maxAttempts: 3,
  delay: 1000,
  backoffMultiplier: 2,
  maxDelay: 10000
}

export async function withRetry<T>(
  operation: () => Promise<T>,
  options: Partial<RetryOptions> = {}
): Promise<T> {
  const config = { ...defaultRetryOptions, ...options }
  let lastError: Error
  
  for (let attempt = 1; attempt <= config.maxAttempts; attempt++) {
    try {
      return await operation()
    } catch (error) {
      lastError = error as Error
      
      if (attempt === config.maxAttempts) {
        break
      }
      
      // Don't retry on certain error types
      if (error instanceof Error) {
        if (error.message.includes('401') || error.message.includes('403') || error.message.includes('404')) {
          break
        }
      }
      
      // Calculate delay with exponential backoff
      const delay = Math.min(
        config.delay * Math.pow(config.backoffMultiplier, attempt - 1),
        config.maxDelay
      )
      
      await new Promise(resolve => setTimeout(resolve, delay))
    }
  }
  
  throw lastError!
}

// Error boundary helper for components
export function isErrorResponse(response: any): response is ErrorResponse {
  return response && response.success === false && response.error
}

export function isSuccessResponse<T>(response: any): response is SuccessResponse<T> {
  return response && response.success === true && response.data !== undefined
}

// Toast notification helper (you can integrate with your preferred toast library)
export function showErrorToast(error: AppError): void {
  const message = formatError(error)
  
  // For now, we'll use console.error and alert
  // In a real app, you'd integrate with a toast library like vue-toastification
  console.error('Error:', message)
  
  if (process.env.NODE_ENV === 'development') {
    alert(`Error: ${message}`)
  }
}

export function showSuccessToast(message: string): void {
  // In a real app, you'd integrate with a toast library
  console.log('Success:', message)
}

// Global error handler
export function setupGlobalErrorHandler(): void {
  window.addEventListener('error', (event) => {
    const error = createError(
      event.message || 'JavaScript error occurred',
      'JS_ERROR',
      {
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno,
        error: event.error
      },
      false
    )
    
    logError(error, 'Global Error Handler')
  })
  
  window.addEventListener('unhandledrejection', (event) => {
    const error = createError(
      'Unhandled promise rejection',
      'PROMISE_REJECTION',
      {
        reason: event.reason
      },
      false
    )
    
    logError(error, 'Global Error Handler')
  })
}

// Error recovery strategies
export function canRetry(error: AppError): boolean {
  if (!error.code) return false
  
  const retryableErrors = [
    ErrorType.NETWORK,
    ErrorType.SERVER_ERROR
  ]
  
  return retryableErrors.includes(error.code as ErrorType)
}

export function shouldShowRetryButton(error: AppError): boolean {
  return canRetry(error) && error.userFriendly !== false
}
