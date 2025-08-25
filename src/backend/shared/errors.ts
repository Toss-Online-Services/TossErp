import { FastifyError, FastifyRequest, FastifyReply } from 'fastify';

export function createErrorHandler(logger: any) {
  return async function errorHandler(
    error: FastifyError,
    request: FastifyRequest,
    reply: FastifyReply
  ) {
    // Log the error
    logger.error({
      error: {
        message: error.message,
        stack: error.stack,
        statusCode: error.statusCode,
      },
      request: {
        method: request.method,
        url: request.url,
        headers: request.headers,
        user: request.user?.id,
      },
    }, 'Request failed');

    // Handle different error types
    if (error.validation) {
      return reply.status(400).send({
        success: false,
        error: {
          code: 'VALIDATION_ERROR',
          message: 'Validation failed',
          details: error.validation,
        },
      });
    }

    if (error.statusCode === 401) {
      return reply.status(401).send({
        success: false,
        error: {
          code: 'UNAUTHORIZED',
          message: 'Authentication required',
        },
      });
    }

    if (error.statusCode === 403) {
      return reply.status(403).send({
        success: false,
        error: {
          code: 'FORBIDDEN',
          message: 'Insufficient permissions',
        },
      });
    }

    if (error.statusCode === 404) {
      return reply.status(404).send({
        success: false,
        error: {
          code: 'NOT_FOUND',
          message: 'Resource not found',
        },
      });
    }

    if (error.statusCode === 429) {
      return reply.status(429).send({
        success: false,
        error: {
          code: 'RATE_LIMITED',
          message: 'Too many requests',
        },
      });
    }

    // Database errors
    if (error.message.includes('Unique constraint')) {
      return reply.status(409).send({
        success: false,
        error: {
          code: 'DUPLICATE_RESOURCE',
          message: 'Resource already exists',
        },
      });
    }

    if (error.message.includes('Foreign key constraint')) {
      return reply.status(400).send({
        success: false,
        error: {
          code: 'INVALID_REFERENCE',
          message: 'Referenced resource does not exist',
        },
      });
    }

    // JWT errors
    if (error.message.includes('jwt')) {
      return reply.status(401).send({
        success: false,
        error: {
          code: 'INVALID_TOKEN',
          message: 'Invalid or expired token',
        },
      });
    }

    // Default server error
    const statusCode = error.statusCode || 500;
    const errorCode = statusCode === 500 ? 'INTERNAL_SERVER_ERROR' : 'CLIENT_ERROR';
    const message = statusCode === 500 ? 'Internal server error' : error.message;

    return reply.status(statusCode).send({
      success: false,
      error: {
        code: errorCode,
        message,
        ...(process.env.NODE_ENV === 'development' && { 
          stack: error.stack,
          details: error 
        }),
      },
    });
  };
}

// Custom error classes
export class ValidationError extends Error {
  statusCode = 400;
  code = 'VALIDATION_ERROR';
  
  constructor(message: string, public details?: any) {
    super(message);
    this.name = 'ValidationError';
  }
}

export class UnauthorizedError extends Error {
  statusCode = 401;
  code = 'UNAUTHORIZED';
  
  constructor(message = 'Authentication required') {
    super(message);
    this.name = 'UnauthorizedError';
  }
}

export class ForbiddenError extends Error {
  statusCode = 403;
  code = 'FORBIDDEN';
  
  constructor(message = 'Insufficient permissions') {
    super(message);
    this.name = 'ForbiddenError';
  }
}

export class NotFoundError extends Error {
  statusCode = 404;
  code = 'NOT_FOUND';
  
  constructor(message = 'Resource not found') {
    super(message);
    this.name = 'NotFoundError';
  }
}

export class ConflictError extends Error {
  statusCode = 409;
  code = 'CONFLICT';
  
  constructor(message = 'Resource conflict') {
    super(message);
    this.name = 'ConflictError';
  }
}

export class TooManyRequestsError extends Error {
  statusCode = 429;
  code = 'TOO_MANY_REQUESTS';
  
  constructor(message = 'Too many requests') {
    super(message);
    this.name = 'TooManyRequestsError';
  }
}

export default {
  createErrorHandler,
  ValidationError,
  UnauthorizedError,
  ForbiddenError,
  NotFoundError,
  ConflictError,
  TooManyRequestsError,
};
