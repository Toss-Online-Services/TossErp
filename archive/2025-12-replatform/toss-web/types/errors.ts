// Error handling types for TOSS ERP III

export enum ErrorCode {
  // Authentication errors
  AUTH_INVALID_CREDENTIALS = 'AUTH_INVALID_CREDENTIALS',
  AUTH_TOKEN_EXPIRED = 'AUTH_TOKEN_EXPIRED',
  AUTH_TOKEN_INVALID = 'AUTH_TOKEN_INVALID',
  AUTH_UNAUTHORIZED = 'AUTH_UNAUTHORIZED',
  AUTH_FORBIDDEN = 'AUTH_FORBIDDEN',
  
  // Validation errors
  VALIDATION_REQUIRED_FIELD = 'VALIDATION_REQUIRED_FIELD',
  VALIDATION_INVALID_FORMAT = 'VALIDATION_INVALID_FORMAT',
  VALIDATION_OUT_OF_RANGE = 'VALIDATION_OUT_OF_RANGE',
  
  // Business logic errors
  BUSINESS_INSUFFICIENT_STOCK = 'BUSINESS_INSUFFICIENT_STOCK',
  BUSINESS_DUPLICATE_ENTRY = 'BUSINESS_DUPLICATE_ENTRY',
  BUSINESS_INVALID_OPERATION = 'BUSINESS_INVALID_OPERATION',
  
  // Network errors
  NETWORK_TIMEOUT = 'NETWORK_TIMEOUT',
  NETWORK_CONNECTION_ERROR = 'NETWORK_CONNECTION_ERROR',
  NETWORK_SERVER_ERROR = 'NETWORK_SERVER_ERROR',
  
  // Resource errors
  RESOURCE_NOT_FOUND = 'RESOURCE_NOT_FOUND',
  RESOURCE_CONFLICT = 'RESOURCE_CONFLICT',
  RESOURCE_GONE = 'RESOURCE_GONE',
  
  // System errors
  SYSTEM_INTERNAL_ERROR = 'SYSTEM_INTERNAL_ERROR',
  SYSTEM_MAINTENANCE = 'SYSTEM_MAINTENANCE',
  SYSTEM_UNAVAILABLE = 'SYSTEM_UNAVAILABLE',
}

export interface AppError {
  code: ErrorCode
  message: string
  details?: Record<string, any>
  timestamp: Date
  path?: string
  statusCode?: number
}

export interface ValidationError {
  field: string
  message: string
  code: ErrorCode
  value?: any
}

export interface ApiErrorResponse {
  error: AppError
  validationErrors?: ValidationError[]
}

export class TossError extends Error {
  public readonly code: ErrorCode
  public readonly details?: Record<string, any>
  public readonly timestamp: Date
  public readonly statusCode?: number

  constructor(
    code: ErrorCode,
    message: string,
    details?: Record<string, any>,
    statusCode?: number
  ) {
    super(message)
    this.name = 'TossError'
    this.code = code
    this.details = details
    this.timestamp = new Date()
    this.statusCode = statusCode
    
    // Maintains proper stack trace for where our error was thrown
    if (Error.captureStackTrace) {
      Error.captureStackTrace(this, TossError)
    }
  }

  toJSON(): AppError {
    return {
      code: this.code,
      message: this.message,
      details: this.details,
      timestamp: this.timestamp,
      statusCode: this.statusCode,
    }
  }
}

// Error factory functions
export const createAuthError = (message: string, details?: Record<string, any>): TossError => {
  return new TossError(ErrorCode.AUTH_UNAUTHORIZED, message, details, 401)
}

export const createValidationError = (message: string, details?: Record<string, any>): TossError => {
  return new TossError(ErrorCode.VALIDATION_INVALID_FORMAT, message, details, 400)
}

export const createNotFoundError = (resource: string, id?: string | number): TossError => {
  return new TossError(
    ErrorCode.RESOURCE_NOT_FOUND,
    `${resource} not found${id ? `: ${id}` : ''}`,
    { resource, id },
    404
  )
}

export const createBusinessError = (message: string, details?: Record<string, any>): TossError => {
  return new TossError(ErrorCode.BUSINESS_INVALID_OPERATION, message, details, 422)
}

export const createNetworkError = (message: string, details?: Record<string, any>): TossError => {
  return new TossError(ErrorCode.NETWORK_CONNECTION_ERROR, message, details, 503)
}

// Error handler utility
export const handleError = (error: unknown): AppError => {
  if (error instanceof TossError) {
    return error.toJSON()
  }
  
  if (error instanceof Error) {
    return {
      code: ErrorCode.SYSTEM_INTERNAL_ERROR,
      message: error.message,
      timestamp: new Date(),
    }
  }
  
  return {
    code: ErrorCode.SYSTEM_INTERNAL_ERROR,
    message: 'An unknown error occurred',
    timestamp: new Date(),
  }
}


