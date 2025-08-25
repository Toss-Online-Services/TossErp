import { FastifyInstance, FastifyPluginAsync } from 'fastify';
import { PrismaClient } from '@prisma/client';
import { createCRUDController } from '../../../../shared/service';

const customerRoutes: FastifyPluginAsync = async (fastify: FastifyInstance) => {
  const prisma = fastify.prisma as PrismaClient;

  // Customer CRUD operations
  const customerController = createCRUDController(
    'Customer',
    prisma.customer,
    {} // Schema will be added later
  );

  // List customers
  fastify.get('/', {
    schema: {
      description: 'List customers with pagination and filtering',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          search: { type: 'string', description: 'Search in name, email' },
          type: { 
            type: 'string', 
            enum: ['INDIVIDUAL', 'COMPANY', 'PARTNERSHIP', 'NGO', 'GOVERNMENT'],
            description: 'Filter by customer type'
          },
          status: { 
            type: 'string', 
            enum: ['ACTIVE', 'INACTIVE', 'PROSPECT', 'LEAD', 'BLACKLISTED'],
            description: 'Filter by customer status'
          },
          industry: { type: 'string', description: 'Filter by industry' },
          sortBy: { type: 'string', default: 'createdAt' },
          sortOrder: { type: 'string', enum: ['asc', 'desc'], default: 'desc' },
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
                  customerCode: { type: 'string' },
                  name: { type: 'string' },
                  type: { type: 'string' },
                  email: { type: 'string' },
                  phone: { type: 'string' },
                  status: { type: 'string' },
                  industry: { type: 'string' },
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
  }, async (request, reply) => {
    const { 
      page = 1, 
      limit = 10, 
      search, 
      type, 
      status, 
      industry,
      sortBy = 'createdAt',
      sortOrder = 'desc'
    } = request.query as any;

    const { tenantId } = (request as any).user;
    const skip = (page - 1) * limit;

    // Build where clause
    const where: any = { tenantId };
    
    if (search) {
      where.OR = [
        { name: { contains: search, mode: 'insensitive' } },
        { email: { contains: search, mode: 'insensitive' } },
        { displayName: { contains: search, mode: 'insensitive' } },
      ];
    }

    if (type) where.type = type;
    if (status) where.status = status;
    if (industry) where.industry = { contains: industry, mode: 'insensitive' };

    const [customers, total] = await Promise.all([
      prisma.customer.findMany({
        where,
        include: {
          addresses: {
            where: { isPrimary: true },
            take: 1,
          },
          contacts: {
            where: { isPrimary: true },
            take: 1,
          },
          _count: {
            select: {
              opportunities: true,
              activities: true,
            },
          },
        },
        skip,
        take: limit,
        orderBy: { [sortBy]: sortOrder },
      }),
      prisma.customer.count({ where }),
    ]);

    return {
      success: true,
      data: customers,
      meta: {
        total,
        page,
        limit,
        hasNext: skip + limit < total,
        hasPrev: page > 1,
      },
    };
  });

  // Get customer by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get customer by ID with full details',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'cuid' },
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
                customerCode: { type: 'string' },
                name: { type: 'string' },
                type: { type: 'string' },
                email: { type: 'string' },
                phone: { type: 'string' },
                website: { type: 'string' },
                status: { type: 'string' },
                industry: { type: 'string' },
                addresses: { type: 'array' },
                contacts: { type: 'array' },
                opportunities: { type: 'array' },
                activities: { type: 'array' },
                createdAt: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;

    const customer = await prisma.customer.findFirst({
      where: { id, tenantId },
      include: {
        addresses: true,
        contacts: true,
        opportunities: {
          include: {
            activities: {
              take: 5,
              orderBy: { createdAt: 'desc' },
            },
          },
        },
        activities: {
          take: 10,
          orderBy: { createdAt: 'desc' },
        },
        notes: {
          take: 5,
          orderBy: { createdAt: 'desc' },
        },
        documents: {
          take: 5,
          orderBy: { createdAt: 'desc' },
        },
        tags: true,
      },
    });

    if (!customer) {
      reply.status(404);
      return { success: false, error: 'Customer not found' };
    }

    return { success: true, data: customer };
  });

  // Create customer
  fastify.post('/', {
    schema: {
      description: 'Create a new customer',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['name', 'type'],
        properties: {
          name: { type: 'string', minLength: 1 },
          type: { 
            type: 'string', 
            enum: ['INDIVIDUAL', 'COMPANY', 'PARTNERSHIP', 'NGO', 'GOVERNMENT']
          },
          displayName: { type: 'string' },
          email: { type: 'string', format: 'email' },
          phone: { type: 'string' },
          website: { type: 'string' },
          industry: { type: 'string' },
          employeeCount: { type: 'integer', minimum: 0 },
          annualRevenue: { type: 'number', minimum: 0 },
          source: { type: 'string' },
          customFields: { type: 'object' },
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
                customerCode: { type: 'string' },
                name: { type: 'string' },
                type: { type: 'string' },
                status: { type: 'string' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId, userId } = (request as any).user;
    const customerData = request.body as any;

    // Generate customer code
    const count = await prisma.customer.count({ where: { tenantId } });
    const customerCode = `CUST-${String(count + 1).padStart(6, '0')}`;

    const customer = await prisma.customer.create({
      data: {
        ...customerData,
        tenantId,
        customerCode,
        createdBy: userId,
        status: 'PROSPECT',
      },
    });

    reply.status(201);
    return { success: true, data: customer };
  });

  // Update customer
  fastify.put('/:id', {
    schema: {
      description: 'Update customer information',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'cuid' },
        },
      },
      body: {
        type: 'object',
        properties: {
          name: { type: 'string', minLength: 1 },
          displayName: { type: 'string' },
          email: { type: 'string', format: 'email' },
          phone: { type: 'string' },
          website: { type: 'string' },
          industry: { type: 'string' },
          employeeCount: { type: 'integer', minimum: 0 },
          annualRevenue: { type: 'number', minimum: 0 },
          status: { 
            type: 'string', 
            enum: ['ACTIVE', 'INACTIVE', 'PROSPECT', 'LEAD', 'BLACKLISTED']
          },
          customFields: { type: 'object' },
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
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;
    const updateData = request.body as any;

    const customer = await prisma.customer.update({
      where: { id, tenantId },
      data: {
        ...updateData,
        updatedAt: new Date(),
      },
    });

    return { success: true, data: customer };
  });

  // Delete customer
  fastify.delete('/:id', {
    schema: {
      description: 'Delete customer (soft delete)',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'cuid' },
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
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;

    await prisma.customer.update({
      where: { id, tenantId },
      data: {
        status: 'INACTIVE',
        updatedAt: new Date(),
      },
    });

    return { success: true, message: 'Customer deleted successfully' };
  });

  // Get customer statistics
  fastify.get('/:id/stats', {
    schema: {
      description: 'Get customer statistics and metrics',
      tags: ['Customers'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string', format: 'cuid' },
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
                totalOpportunities: { type: 'integer' },
                totalRevenue: { type: 'number' },
                activitiesCount: { type: 'integer' },
                lastActivity: { type: 'string' },
                conversionRate: { type: 'number' },
              },
            },
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;

    const [
      totalOpportunities,
      wonOpportunities,
      totalRevenue,
      activitiesCount,
      lastActivity
    ] = await Promise.all([
      prisma.opportunity.count({ where: { customerId: id, customer: { tenantId } } }),
      prisma.opportunity.count({ where: { customerId: id, status: 'WON', customer: { tenantId } } }),
      prisma.opportunity.aggregate({
        where: { customerId: id, status: 'WON', customer: { tenantId } },
        _sum: { amount: true },
      }),
      prisma.activity.count({ where: { customerId: id, customer: { tenantId } } }),
      prisma.activity.findFirst({
        where: { customerId: id, customer: { tenantId } },
        orderBy: { createdAt: 'desc' },
        select: { createdAt: true },
      }),
    ]);

    const conversionRate = totalOpportunities > 0 ? (wonOpportunities / totalOpportunities) * 100 : 0;

    return {
      success: true,
      data: {
        totalOpportunities,
        totalRevenue: totalRevenue._sum.amount || 0,
        activitiesCount,
        lastActivity: lastActivity?.createdAt,
        conversionRate: Math.round(conversionRate * 100) / 100,
      },
    };
  });
};

export { customerRoutes };
