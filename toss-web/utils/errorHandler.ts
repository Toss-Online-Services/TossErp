/**
 * Error handling utilities for user-facing error messages
 * Sanitizes technical details and provides user-friendly messages
 */

export interface UserFriendlyError {
  message: string
  type: 'error' | 'warning' | 'info'
  action?: string
}

/**
 * Sanitizes error messages to remove technical details and security information
 * @param error - The error object or message
 * @param context - Optional context about where the error occurred
 * @returns User-friendly error message
 */
export function sanitizeError(error: any, context?: string): UserFriendlyError {
  // Default user-friendly message
  let message = 'Something went wrong. Please try again.'
  let type: 'error' | 'warning' | 'info' = 'error'
  let action = 'Please try again or contact support if the problem persists.'

  // Handle different error types
  if (typeof error === 'string') {
    // Simple string error - check for common patterns
    const lowerError = error.toLowerCase()
    
    if (lowerError.includes('network') || lowerError.includes('connection') || lowerError.includes('fetch')) {
      message = 'Unable to connect to the server'
      action = 'Please check your internet connection and try again.'
    } else if (lowerError.includes('timeout')) {
      message = 'The request took too long to complete'
      action = 'Please try again in a moment.'
    } else if (lowerError.includes('unauthorized') || lowerError.includes('401')) {
      message = 'You need to sign in again'
      action = 'Please refresh the page and sign in.'
    } else if (lowerError.includes('forbidden') || lowerError.includes('403')) {
      message = 'You don\'t have permission to perform this action'
      action = 'Please contact your administrator.'
    } else if (lowerError.includes('not found') || lowerError.includes('404')) {
      message = 'The requested information could not be found'
      action = 'Please try refreshing the page.'
    } else if (lowerError.includes('server') || lowerError.includes('500')) {
      message = 'Server is temporarily unavailable'
      action = 'Please try again in a few minutes.'
    }
  } else if (error && typeof error === 'object') {
    // Handle error objects
    const errorMessage = error.message || error.error || error.statusText || ''
    const statusCode = error.status || error.statusCode || error.code
    
    // Check status codes
    if (statusCode) {
      switch (statusCode) {
        case 400:
          message = 'Invalid request'
          action = 'Please check your input and try again.'
          break
        case 401:
          message = 'You need to sign in again'
          action = 'Please refresh the page and sign in.'
          break
        case 403:
          message = 'You don\'t have permission to perform this action'
          action = 'Please contact your administrator.'
          break
        case 404:
          message = 'The requested information could not be found'
          action = 'Please try refreshing the page.'
          break
        case 409:
          message = 'This action conflicts with existing data'
          action = 'Please refresh the page and try again.'
          break
        case 422:
          message = 'Some information is missing or invalid'
          action = 'Please check your input and try again.'
          break
        case 429:
          message = 'Too many requests'
          action = 'Please wait a moment and try again.'
          type = 'warning'
          break
        case 500:
        case 502:
        case 503:
        case 504:
          message = 'Server is temporarily unavailable'
          action = 'Please try again in a few minutes.'
          break
        default:
          if (statusCode >= 400 && statusCode < 500) {
            message = 'There was a problem with your request'
            action = 'Please check your input and try again.'
          } else if (statusCode >= 500) {
            message = 'Server is temporarily unavailable'
            action = 'Please try again in a few minutes.'
          }
      }
    } else if (errorMessage) {
      // Recursively sanitize the error message
      return sanitizeError(errorMessage, context)
    }
  }

  // Context-specific messages
  if (context) {
    switch (context) {
      case 'load_data':
        message = 'Unable to load data'
        action = 'Please refresh the page or try again later.'
        break
      case 'save_data':
        message = 'Unable to save your changes'
        action = 'Please try again or check your connection.'
        break
      case 'payment':
        message = 'Payment could not be processed'
        action = 'Please try a different payment method or try again.'
        break
      case 'product_search':
        message = 'Unable to search products'
        action = 'Please try again or refresh the page.'
        break
      case 'customer_lookup':
        message = 'Unable to find customer information'
        action = 'Please try again or check the customer details.'
        break
      case 'order_creation':
        message = 'Unable to create order'
        action = 'Please check your order details and try again.'
        break
      case 'invoice_creation':
        message = 'Unable to create invoice'
        action = 'Please check your invoice details and try again.'
        break
    }
  }

  return { message, type, action }
}

/**
 * Creates a user-friendly notification message
 * @param error - The error to sanitize
 * @param context - Optional context
 * @returns Formatted notification message
 */
export function getErrorNotification(error: any, context?: string): string {
  const sanitized = sanitizeError(error, context)
  return `⚠️ ${sanitized.message}`
}

/**
 * Creates a detailed error message for display in error states
 * @param error - The error to sanitize
 * @param context - Optional context
 * @returns Formatted error message with action
 */
export function getDetailedErrorMessage(error: any, context?: string): { title: string; message: string; action: string } {
  const sanitized = sanitizeError(error, context)
  
  return {
    title: sanitized.type === 'warning' ? 'Please Wait' : 'Something Went Wrong',
    message: sanitized.message,
    action: sanitized.action || 'Please try again.'
  }
}

/**
 * Logs the original error for debugging while showing sanitized message to user
 * @param error - The original error
 * @param context - Context where error occurred
 * @param userMessage - The sanitized message shown to user
 */
export function logError(error: any, context?: string, userMessage?: string): void {
  const timestamp = new Date().toISOString()
  const logContext = context ? `[${context}]` : '[Unknown]'
  
  console.error(`${timestamp} ${logContext} Original error:`, error)
  if (userMessage) {
    console.info(`${timestamp} ${logContext} User message: ${userMessage}`)
  }
}







