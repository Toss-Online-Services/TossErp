import { FastifyInstance, FastifyRequest, FastifyReply } from 'fastify';
import { AuthController } from '../controllers/auth';

export async function authRoutes(fastify: FastifyInstance) {
  const authController = new AuthController(fastify.prisma, fastify.redis, fastify.logger);

  // Register
  fastify.post('/register', {
    schema: {
      description: 'Register a new user and tenant',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['email', 'password', 'firstName', 'lastName', 'tenantName', 'tenantType'],
        properties: {
          email: { type: 'string', format: 'email' },
          password: { type: 'string', minLength: 8 },
          firstName: { type: 'string', minLength: 1 },
          lastName: { type: 'string', minLength: 1 },
          tenantName: { type: 'string', minLength: 1 },
          tenantType: { 
            type: 'string', 
            enum: [
              'RETAIL_TRADE', 'FOOD_HOSPITALITY', 'PERSONAL_SERVICES', 
              'TRADES_TECHNICAL', 'AGRICULTURE', 'MANUFACTURING',
              'TRANSPORTATION', 'CONSTRUCTION', 'TECHNOLOGY', 
              'HEALTHCARE', 'EDUCATION', 'CREATIVE_ARTS',
              'PROFESSIONAL_SERVICES', 'FINANCIAL_SERVICES', 
              'ENERGY_UTILITIES', 'WASTE_RECYCLING',
              'ENTERTAINMENT_EVENTS', 'SECURITY_SERVICES', 
              'CLEANING_MAINTENANCE', 'COMMUNITY_SERVICES', 'OTHER'
            ]
          },
          phone: { type: 'string' },
          mobile: { type: 'string' },
        },
      },
      response: {
        201: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                user: {
                  type: 'object',
                  properties: {
                    id: { type: 'string' },
                    email: { type: 'string' },
                    firstName: { type: 'string' },
                    lastName: { type: 'string' },
                    role: { type: 'string' },
                  },
                },
                tenant: {
                  type: 'object',
                  properties: {
                    id: { type: 'string' },
                    name: { type: 'string' },
                    type: { type: 'string' },
                  },
                },
                tokens: {
                  type: 'object',
                  properties: {
                    accessToken: { type: 'string' },
                    refreshToken: { type: 'string' },
                  },
                },
              },
            },
          },
        },
      },
    },
  }, authController.register.bind(authController));

  // Login
  fastify.post('/login', {
    schema: {
      description: 'Authenticate user and get tokens',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['email', 'password'],
        properties: {
          email: { type: 'string', format: 'email' },
          password: { type: 'string' },
          rememberMe: { type: 'boolean', default: false },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                user: {
                  type: 'object',
                  properties: {
                    id: { type: 'string' },
                    email: { type: 'string' },
                    firstName: { type: 'string' },
                    lastName: { type: 'string' },
                    role: { type: 'string' },
                    tenantId: { type: 'string' },
                  },
                },
                tokens: {
                  type: 'object',
                  properties: {
                    accessToken: { type: 'string' },
                    refreshToken: { type: 'string' },
                  },
                },
              },
            },
          },
        },
      },
    },
  }, authController.login.bind(authController));

  // Refresh Token
  fastify.post('/refresh', {
    schema: {
      description: 'Refresh access token using refresh token',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['refreshToken'],
        properties: {
          refreshToken: { type: 'string' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                accessToken: { type: 'string' },
                refreshToken: { type: 'string' },
              },
            },
          },
        },
      },
    },
  }, authController.refresh.bind(authController));

  // Logout
  fastify.post('/logout', {
    schema: {
      description: 'Logout user and revoke tokens',
      tags: ['Authentication'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        properties: {
          refreshToken: { type: 'string' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            message: { type: 'string' },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, authController.logout.bind(authController));

  // Verify Email
  fastify.post('/verify-email', {
    schema: {
      description: 'Verify user email address',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['token'],
        properties: {
          token: { type: 'string' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            message: { type: 'string' },
          },
        },
      },
    },
  }, authController.verifyEmail.bind(authController));

  // Request Password Reset
  fastify.post('/forgot-password', {
    schema: {
      description: 'Request password reset email',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['email'],
        properties: {
          email: { type: 'string', format: 'email' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            message: { type: 'string' },
          },
        },
      },
    },
  }, authController.forgotPassword.bind(authController));

  // Reset Password
  fastify.post('/reset-password', {
    schema: {
      description: 'Reset password using reset token',
      tags: ['Authentication'],
      body: {
        type: 'object',
        required: ['token', 'password'],
        properties: {
          token: { type: 'string' },
          password: { type: 'string', minLength: 8 },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            message: { type: 'string' },
          },
        },
      },
    },
  }, authController.resetPassword.bind(authController));

  // Get Profile
  fastify.get('/me', {
    schema: {
      description: 'Get current user profile',
      tags: ['Authentication'],
      security: [{ bearerAuth: [] }],
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                id: { type: 'string' },
                email: { type: 'string' },
                firstName: { type: 'string' },
                lastName: { type: 'string' },
                role: { type: 'string' },
                tenantId: { type: 'string' },
                isActive: { type: 'boolean' },
                emailVerified: { type: 'boolean' },
                profile: { type: 'object' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, authController.getProfile.bind(authController));

  // Update Profile
  fastify.put('/me', {
    schema: {
      description: 'Update current user profile',
      tags: ['Authentication'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        properties: {
          firstName: { type: 'string' },
          lastName: { type: 'string' },
          phone: { type: 'string' },
          mobile: { type: 'string' },
          street: { type: 'string' },
          city: { type: 'string' },
          province: { type: 'string' },
          postalCode: { type: 'string' },
          country: { type: 'string' },
          dateOfBirth: { type: 'string', format: 'date' },
          gender: { type: 'string', enum: ['MALE', 'FEMALE', 'OTHER', 'PREFER_NOT_TO_SAY'] },
          language: { type: 'string' },
          timezone: { type: 'string' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: { type: 'object' },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, authController.updateProfile.bind(authController));

  // Change Password
  fastify.post('/change-password', {
    schema: {
      description: 'Change user password',
      tags: ['Authentication'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['currentPassword', 'newPassword'],
        properties: {
          currentPassword: { type: 'string' },
          newPassword: { type: 'string', minLength: 8 },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            message: { type: 'string' },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, authController.changePassword.bind(authController));
}
