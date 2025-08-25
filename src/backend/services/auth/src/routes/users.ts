import { FastifyInstance } from 'fastify';
import { UserController } from '../controllers/user';

export async function userRoutes(fastify: FastifyInstance) {
  const userController = new UserController(fastify.prisma, fastify.redis, fastify.logger);

  // List users (admin only)
  fastify.get('/', {
    schema: {
      description: 'List all users in tenant',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          search: { type: 'string' },
          role: { type: 'string' },
          isActive: { type: 'boolean' },
        },
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'array',
              items: {
                type: 'object',
                properties: {
                  id: { type: 'string' },
                  email: { type: 'string' },
                  firstName: { type: 'string' },
                  lastName: { type: 'string' },
                  role: { type: 'string' },
                  isActive: { type: 'boolean' },
                  createdAt: { type: 'string' },
                },
              },
            },
            meta: {
              type: 'object',
              properties: {
                total: { type: 'integer' },
                page: { type: 'integer' },
                limit: { type: 'integer' },
                hasNext: { type: 'boolean' },
                hasPrev: { type: 'boolean' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, userController.list.bind(userController));

  // Get user by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get user by ID',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'uuid' },
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
                id: { type: 'string' },
                email: { type: 'string' },
                firstName: { type: 'string' },
                lastName: { type: 'string' },
                role: { type: 'string' },
                isActive: { type: 'boolean' },
                profile: { type: 'object' },
                createdAt: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, userController.get.bind(userController));

  // Create user (admin only)
  fastify.post('/', {
    schema: {
      description: 'Create new user',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['email', 'firstName', 'lastName', 'role'],
        properties: {
          email: { type: 'string', format: 'email' },
          firstName: { type: 'string', minLength: 1 },
          lastName: { type: 'string', minLength: 1 },
          role: { 
            type: 'string',
            enum: ['TENANT_ADMIN', 'MANAGER', 'EMPLOYEE', 'CUSTOMER', 'VENDOR']
          },
          password: { type: 'string', minLength: 8 },
          sendInvite: { type: 'boolean', default: true },
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
                id: { type: 'string' },
                email: { type: 'string' },
                firstName: { type: 'string' },
                lastName: { type: 'string' },
                role: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, userController.create.bind(userController));

  // Update user
  fastify.put('/:id', {
    schema: {
      description: 'Update user',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'uuid' },
        },
      },
      body: {
        type: 'object',
        properties: {
          firstName: { type: 'string', minLength: 1 },
          lastName: { type: 'string', minLength: 1 },
          role: { 
            type: 'string',
            enum: ['TENANT_ADMIN', 'MANAGER', 'EMPLOYEE', 'CUSTOMER', 'VENDOR']
          },
          isActive: { type: 'boolean' },
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
  }, userController.update.bind(userController));

  // Delete user
  fastify.delete('/:id', {
    schema: {
      description: 'Delete user',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'uuid' },
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
  }, userController.delete.bind(userController));

  // Activate/Deactivate user
  fastify.patch('/:id/status', {
    schema: {
      description: 'Activate or deactivate user',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'uuid' },
        },
      },
      body: {
        type: 'object',
        required: ['isActive'],
        properties: {
          isActive: { type: 'boolean' },
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
  }, userController.updateStatus.bind(userController));

  // Reset user password (admin only)
  fastify.post('/:id/reset-password', {
    schema: {
      description: 'Reset user password',
      tags: ['Users'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'uuid' },
        },
      },
      body: {
        type: 'object',
        properties: {
          sendEmail: { type: 'boolean', default: true },
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
  }, userController.resetPassword.bind(userController));
}
