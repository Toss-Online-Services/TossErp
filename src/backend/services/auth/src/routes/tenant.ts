import { FastifyInstance } from 'fastify';
import { TenantController } from '../controllers/tenant';

export async function tenantRoutes(fastify: FastifyInstance) {
  const tenantController = new TenantController(fastify.prisma, fastify.redis, fastify.logger);

  // Get current tenant details
  fastify.get('/', {
    schema: {
      description: 'Get current tenant details',
      tags: ['Tenant'],
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
                name: { type: 'string' },
                type: { type: 'string' },
                subdomain: { type: 'string' },
                settings: { type: 'object' },
                isActive: { type: 'boolean' },
                createdAt: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, tenantController.get.bind(tenantController));

  // Update tenant settings
  fastify.put('/', {
    schema: {
      description: 'Update tenant settings',
      tags: ['Tenant'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        properties: {
          name: { type: 'string', minLength: 1 },
          settings: { type: 'object' },
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
  }, tenantController.update.bind(tenantController));

  // Get tenant statistics
  fastify.get('/stats', {
    schema: {
      description: 'Get tenant statistics',
      tags: ['Tenant'],
      security: [{ bearerAuth: [] }],
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                totalUsers: { type: 'integer' },
                activeUsers: { type: 'integer' },
                totalRoles: { type: 'integer' },
                totalSessions: { type: 'integer' },
                storageUsed: { type: 'integer' },
                apiCalls: { type: 'integer' },
                lastActivity: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, tenantController.getStats.bind(tenantController));

  // Get billing information
  fastify.get('/billing', {
    schema: {
      description: 'Get tenant billing information',
      tags: ['Tenant'],
      security: [{ bearerAuth: [] }],
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                plan: { type: 'string' },
                billingCycle: { type: 'string' },
                nextBillingDate: { type: 'string' },
                usage: { type: 'object' },
                limits: { type: 'object' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, tenantController.getBilling.bind(tenantController));

  // Get audit logs
  fastify.get('/audit-logs', {
    schema: {
      description: 'Get tenant audit logs',
      tags: ['Tenant'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 20 },
          userId: { type: 'string' },
          action: { type: 'string' },
          startDate: { type: 'string', format: 'date-time' },
          endDate: { type: 'string', format: 'date-time' },
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
                  userId: { type: 'string' },
                  action: { type: 'string' },
                  resource: { type: 'string' },
                  details: { type: 'object' },
                  ipAddress: { type: 'string' },
                  userAgent: { type: 'string' },
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
  }, tenantController.getAuditLogs.bind(tenantController));
}
