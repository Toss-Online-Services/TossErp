import { ref } from 'vue'
import type { AppError } from '~/types/errors'
import { handleError, ErrorCode } from '~/types/errors'

export const useErrorHandler = () => {
  const currentError = ref<AppError | null>(null)
  const errors = ref<AppError[]>([])

  const captureError = (error: unknown, context?: string): AppError => {
    const appError = handleError(error)
    
    if (context) {
      appError.path = context
    }
    
    currentError.value = appError
    errors.value.push(appError)
    
    // Log to console in development
    if (process.dev) {
      console.error('[TOSS Error]', appError)
    }
    
    // Show notification for user-facing errors
    if (shouldNotifyUser(appError.code)) {
      const notificationStore = useNotificationStore()
      notificationStore.add({
        type: 'error',
        title: getErrorTitle(appError.code),
        message: appError.message,
      })
    }
    
    return appError
  }

  const clearError = () => {
    currentError.value = null
  }

  const clearAllErrors = () => {
    currentError.value = null
    errors.value = []
  }

  const shouldNotifyUser = (code: ErrorCode): boolean => {
    // Don't notify for certain error types
    const silentErrors = [
      ErrorCode.AUTH_TOKEN_EXPIRED, // Handled by auth system
    ]
    return !silentErrors.includes(code)
  }

  const getErrorTitle = (code: ErrorCode): string => {
    const titles: Record<ErrorCode, string> = {
      [ErrorCode.AUTH_INVALID_CREDENTIALS]: 'Authentication Failed',
      [ErrorCode.AUTH_TOKEN_EXPIRED]: 'Session Expired',
      [ErrorCode.AUTH_TOKEN_INVALID]: 'Invalid Session',
      [ErrorCode.AUTH_UNAUTHORIZED]: 'Unauthorized',
      [ErrorCode.AUTH_FORBIDDEN]: 'Access Denied',
      [ErrorCode.VALIDATION_REQUIRED_FIELD]: 'Validation Error',
      [ErrorCode.VALIDATION_INVALID_FORMAT]: 'Invalid Format',
      [ErrorCode.VALIDATION_OUT_OF_RANGE]: 'Value Out of Range',
      [ErrorCode.BUSINESS_INSUFFICIENT_STOCK]: 'Insufficient Stock',
      [ErrorCode.BUSINESS_DUPLICATE_ENTRY]: 'Duplicate Entry',
      [ErrorCode.BUSINESS_INVALID_OPERATION]: 'Invalid Operation',
      [ErrorCode.NETWORK_TIMEOUT]: 'Request Timeout',
      [ErrorCode.NETWORK_CONNECTION_ERROR]: 'Connection Error',
      [ErrorCode.NETWORK_SERVER_ERROR]: 'Server Error',
      [ErrorCode.RESOURCE_NOT_FOUND]: 'Not Found',
      [ErrorCode.RESOURCE_CONFLICT]: 'Conflict',
      [ErrorCode.RESOURCE_GONE]: 'Resource Unavailable',
      [ErrorCode.SYSTEM_INTERNAL_ERROR]: 'System Error',
      [ErrorCode.SYSTEM_MAINTENANCE]: 'System Maintenance',
      [ErrorCode.SYSTEM_UNAVAILABLE]: 'System Unavailable',
    }
    return titles[code] || 'Error'
  }

  const isAuthError = (error: AppError): boolean => {
    return error.code.startsWith('AUTH_')
  }

  const isValidationError = (error: AppError): boolean => {
    return error.code.startsWith('VALIDATION_')
  }

  const isNetworkError = (error: AppError): boolean => {
    return error.code.startsWith('NETWORK_')
  }

  return {
    currentError: readonly(currentError),
    errors: readonly(errors),
    captureError,
    clearError,
    clearAllErrors,
    isAuthError,
    isValidationError,
    isNetworkError,
  }
}


