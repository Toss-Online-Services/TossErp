import { FastifyInstance, FastifyPluginAsync } from 'fastify';
import { PrismaClient } from '@prisma/client';

const opportunityRoutes: FastifyPluginAsync = async (fastify: FastifyInstance) => {
  const prisma = fastify.prisma as PrismaClient;

  // List opportunities
  fastify.get('/', {
    schema: {
      description: 'List opportunities with pagination and filtering',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          customerId: { type: 'string' },
          stage: { type: 'string' },
          status: { type: 'string', enum: ['OPEN', 'WON', 'LOST', 'CLOSED'] },
          ownerId: { type: 'string' },
          search: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { 
      page = 1, 
      limit = 10, 
      customerId,
      stage,
      status,
      ownerId,
      search
    } = request.query as any;

    const { tenantId } = (request as any).user;
    const skip = (page - 1) * limit;

    const where: any = { customer: { tenantId } };
    
    if (customerId) where.customerId = customerId;
    if (stage) where.stage = stage;
    if (status) where.status = status;
    if (ownerId) where.ownerId = ownerId;
    
    if (search) {
      where.OR = [
        { name: { contains: search, mode: 'insensitive' } },
        { description: { contains: search, mode: 'insensitive' } },
      ];
    }

    const [opportunities, total] = await Promise.all([
      prisma.opportunity.findMany({
        where,
        include: {
          customer: {
            select: { id: true, name: true, customerCode: true },
          },
          activities: {
            take: 3,
            orderBy: { createdAt: 'desc' },
          },
        },
        skip,
        take: limit,
        orderBy: { createdAt: 'desc' },
      }),
      prisma.opportunity.count({ where }),
    ]);

    return {
      success: true,
      data: opportunities,
      meta: {
        total,
        page,
        limit,
        hasNext: skip + limit < total,
        hasPrev: page > 1,
      },
    };
  });

  // Create opportunity
  fastify.post('/', {
    schema: {
      description: 'Create a new opportunity',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['customerId', 'name', 'stage'],
        properties: {
          customerId: { type: 'string' },
          name: { type: 'string', minLength: 1 },
          description: { type: 'string' },
          amount: { type: 'number', minimum: 0 },
          currency: { type: 'string', default: 'ZAR' },
          stage: { type: 'string' },
          probability: { type: 'integer', minimum: 0, maximum: 100 },
          source: { type: 'string' },
          expectedCloseDate: { type: 'string', format: 'date-time' },
          ownerId: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId, userId } = (request as any).user;
    const opportunityData = request.body as any;

    const opportunity = await prisma.opportunity.create({
      data: {
        ...opportunityData,
        ownerId: opportunityData.ownerId || userId,
        createdBy: userId,
        status: 'OPEN',
      },
      include: {
        customer: {
          select: { id: true, name: true, customerCode: true },
        },
      },
    });

    reply.status(201);
    return { success: true, data: opportunity };
  });

  // Get opportunity by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get opportunity by ID',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;

    const opportunity = await prisma.opportunity.findFirst({
      where: { id, customer: { tenantId } },
      include: {
        customer: true,
        activities: {
          orderBy: { createdAt: 'desc' },
        },
        documents: {
          orderBy: { createdAt: 'desc' },
        },
        notes: {
          orderBy: { createdAt: 'desc' },
        },
        products: true,
      },
    });

    if (!opportunity) {
      reply.status(404);
      return { success: false, error: 'Opportunity not found' };
    }

    return { success: true, data: opportunity };
  });

  // Update opportunity
  fastify.put('/:id', {
    schema: {
      description: 'Update opportunity',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string' },
        },
      },
      body: {
        type: 'object',
        properties: {
          name: { type: 'string', minLength: 1 },
          description: { type: 'string' },
          amount: { type: 'number', minimum: 0 },
          stage: { type: 'string' },
          probability: { type: 'integer', minimum: 0, maximum: 100 },
          expectedCloseDate: { type: 'string', format: 'date-time' },
          status: { type: 'string', enum: ['OPEN', 'WON', 'LOST', 'CLOSED'] },
          lostReason: { type: 'string' },
          ownerId: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;
    const updateData = request.body as any;

    // Handle status changes
    if (updateData.status && ['WON', 'LOST', 'CLOSED'].includes(updateData.status)) {
      updateData.actualCloseDate = new Date();
    }

    const opportunity = await prisma.opportunity.update({
      where: { id, customer: { tenantId } },
      data: {
        ...updateData,
        updatedAt: new Date(),
      },
    });

    return { success: true, data: opportunity };
  });

  // Delete opportunity
  fastify.delete('/:id', {
    schema: {
      description: 'Delete opportunity',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
      params: {
        type: 'object',
        required: ['id'],
        properties: {
          id: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;

    await prisma.opportunity.delete({
      where: { id, customer: { tenantId } },
    });

    return { success: true, message: 'Opportunity deleted successfully' };
  });

  // Get opportunity pipeline stats
  fastify.get('/stats/pipeline', {
    schema: {
      description: 'Get opportunity pipeline statistics',
      tags: ['Opportunities'],
      security: [{ bearerAuth: [] }],
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId } = (request as any).user;

    const [
      totalOpportunities,
      totalValue,
      byStage,
      byStatus,
      conversionRate
    ] = await Promise.all([
      prisma.opportunity.count({ where: { customer: { tenantId } } }),
      prisma.opportunity.aggregate({
        where: { customer: { tenantId }, status: 'OPEN' },
        _sum: { amount: true },
      }),
      prisma.opportunity.groupBy({
        by: ['stage'],
        where: { customer: { tenantId }, status: 'OPEN' },
        _count: { _all: true },
        _sum: { amount: true },
      }),
      prisma.opportunity.groupBy({
        by: ['status'],
        where: { customer: { tenantId } },
        _count: { _all: true },
        _sum: { amount: true },
      }),
      prisma.opportunity.findMany({
        where: { customer: { tenantId } },
        select: { status: true },
      }),
    ]);

    const wonCount = conversionRate.filter(o => o.status === 'WON').length;
    const rate = totalOpportunities > 0 ? (wonCount / totalOpportunities) * 100 : 0;

    return {
      success: true,
      data: {
        totalOpportunities,
        totalValue: totalValue._sum.amount || 0,
        conversionRate: Math.round(rate * 100) / 100,
        byStage,
        byStatus,
      },
    };
  });
};

export { opportunityRoutes };
