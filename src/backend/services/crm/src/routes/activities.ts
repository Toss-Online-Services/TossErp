import { FastifyInstance, FastifyPluginAsync } from 'fastify';
import { PrismaClient } from '@prisma/client';

const activityRoutes: FastifyPluginAsync = async (fastify: FastifyInstance) => {
  const prisma = fastify.prisma as PrismaClient;

  // List activities
  fastify.get('/', {
    schema: {
      description: 'List activities with pagination and filtering',
      tags: ['Activities'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          customerId: { type: 'string' },
          opportunityId: { type: 'string' },
          type: { type: 'string', enum: ['CALL', 'EMAIL', 'MEETING', 'TASK', 'NOTE'] },
          status: { type: 'string', enum: ['PLANNED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED'] },
          assignedTo: { type: 'string' },
          dateFrom: { type: 'string', format: 'date' },
          dateTo: { type: 'string', format: 'date' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { 
      page = 1, 
      limit = 10, 
      customerId,
      opportunityId,
      type,
      status,
      assignedTo,
      dateFrom,
      dateTo
    } = request.query as any;

    const { tenantId } = (request as any).user;
    const skip = (page - 1) * limit;

    const where: any = {
      OR: [
        { customer: { tenantId } },
        { opportunity: { customer: { tenantId } } },
      ],
    };
    
    if (customerId) where.customerId = customerId;
    if (opportunityId) where.opportunityId = opportunityId;
    if (type) where.type = type;
    if (status) where.status = status;
    if (assignedTo) where.assignedTo = assignedTo;
    
    if (dateFrom || dateTo) {
      where.dueDate = {};
      if (dateFrom) where.dueDate.gte = new Date(dateFrom);
      if (dateTo) where.dueDate.lte = new Date(dateTo);
    }

    const [activities, total] = await Promise.all([
      prisma.activity.findMany({
        where,
        include: {
          customer: {
            select: { id: true, name: true, customerCode: true },
          },
          opportunity: {
            select: { id: true, name: true },
          },
        },
        skip,
        take: limit,
        orderBy: { dueDate: 'asc' },
      }),
      prisma.activity.count({ where }),
    ]);

    return {
      success: true,
      data: activities,
      meta: {
        total,
        page,
        limit,
        hasNext: skip + limit < total,
        hasPrev: page > 1,
      },
    };
  });

  // Create activity
  fastify.post('/', {
    schema: {
      description: 'Create a new activity',
      tags: ['Activities'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['type', 'subject'],
        properties: {
          type: { type: 'string', enum: ['CALL', 'EMAIL', 'MEETING', 'TASK', 'NOTE'] },
          subject: { type: 'string', minLength: 1 },
          description: { type: 'string' },
          customerId: { type: 'string' },
          opportunityId: { type: 'string' },
          dueDate: { type: 'string', format: 'date-time' },
          duration: { type: 'integer', minimum: 0 },
          priority: { type: 'string', enum: ['LOW', 'MEDIUM', 'HIGH'], default: 'MEDIUM' },
          assignedTo: { type: 'string' },
          location: { type: 'string' },
          attendees: { type: 'array', items: { type: 'string' } },
          reminderMinutes: { type: 'integer', minimum: 0 },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId, userId } = (request as any).user;
    const activityData = request.body as any;

    const activity = await prisma.activity.create({
      data: {
        ...activityData,
        assignedTo: activityData.assignedTo || userId,
        createdBy: userId,
        status: 'PLANNED',
      },
      include: {
        customer: {
          select: { id: true, name: true, customerCode: true },
        },
        opportunity: {
          select: { id: true, name: true },
        },
      },
    });

    reply.status(201);
    return { success: true, data: activity };
  });

  // Get activity by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get activity by ID',
      tags: ['Activities'],
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

    const activity = await prisma.activity.findFirst({
      where: {
        id,
        OR: [
          { customer: { tenantId } },
          { opportunity: { customer: { tenantId } } },
        ],
      },
      include: {
        customer: true,
        opportunity: true,
      },
    });

    if (!activity) {
      reply.status(404);
      return { success: false, error: 'Activity not found' };
    }

    return { success: true, data: activity };
  });

  // Update activity
  fastify.put('/:id', {
    schema: {
      description: 'Update activity',
      tags: ['Activities'],
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
          subject: { type: 'string', minLength: 1 },
          description: { type: 'string' },
          dueDate: { type: 'string', format: 'date-time' },
          duration: { type: 'integer', minimum: 0 },
          priority: { type: 'string', enum: ['LOW', 'MEDIUM', 'HIGH'] },
          status: { type: 'string', enum: ['PLANNED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED'] },
          assignedTo: { type: 'string' },
          location: { type: 'string' },
          attendees: { type: 'array', items: { type: 'string' } },
          reminderMinutes: { type: 'integer', minimum: 0 },
          outcome: { type: 'string' },
          followUpDate: { type: 'string', format: 'date-time' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;
    const updateData = request.body as any;

    // Auto-set completion date if status is being set to completed
    if (updateData.status === 'COMPLETED' && !updateData.completedAt) {
      updateData.completedAt = new Date();
    }

    const activity = await prisma.activity.update({
      where: {
        id,
        OR: [
          { customer: { tenantId } },
          { opportunity: { customer: { tenantId } } },
        ],
      },
      data: {
        ...updateData,
        updatedAt: new Date(),
      },
      include: {
        customer: {
          select: { id: true, name: true, customerCode: true },
        },
        opportunity: {
          select: { id: true, name: true },
        },
      },
    });

    return { success: true, data: activity };
  });

  // Delete activity
  fastify.delete('/:id', {
    schema: {
      description: 'Delete activity',
      tags: ['Activities'],
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

    await prisma.activity.delete({
      where: {
        id,
        OR: [
          { customer: { tenantId } },
          { opportunity: { customer: { tenantId } } },
        ],
      },
    });

    return { success: true, message: 'Activity deleted successfully' };
  });

  // Get activity statistics
  fastify.get('/stats/summary', {
    schema: {
      description: 'Get activity summary statistics',
      tags: ['Activities'],
      security: [{ bearerAuth: [] }],
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId } = (request as any).user;

    const [
      totalActivities,
      pendingActivities,
      completedToday,
      overdue,
      byType,
      byStatus
    ] = await Promise.all([
      prisma.activity.count({
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
        },
      }),
      prisma.activity.count({
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
          status: { in: ['PLANNED', 'IN_PROGRESS'] },
        },
      }),
      prisma.activity.count({
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
          status: 'COMPLETED',
          completedAt: {
            gte: new Date(new Date().setHours(0, 0, 0, 0)),
            lte: new Date(new Date().setHours(23, 59, 59, 999)),
          },
        },
      }),
      prisma.activity.count({
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
          status: { in: ['PLANNED', 'IN_PROGRESS'] },
          dueDate: {
            lt: new Date(),
          },
        },
      }),
      prisma.activity.groupBy({
        by: ['type'],
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
        },
        _count: { _all: true },
      }),
      prisma.activity.groupBy({
        by: ['status'],
        where: {
          OR: [
            { customer: { tenantId } },
            { opportunity: { customer: { tenantId } } },
          ],
        },
        _count: { _all: true },
      }),
    ]);

    return {
      success: true,
      data: {
        totalActivities,
        pendingActivities,
        completedToday,
        overdue,
        byType,
        byStatus,
      },
    };
  });

  // Get upcoming activities (today and next 7 days)
  fastify.get('/upcoming', {
    schema: {
      description: 'Get upcoming activities for the next 7 days',
      tags: ['Activities'],
      security: [{ bearerAuth: [] }],
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId } = (request as any).user;
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);

    const activities = await prisma.activity.findMany({
      where: {
        OR: [
          { customer: { tenantId } },
          { opportunity: { customer: { tenantId } } },
        ],
        status: { in: ['PLANNED', 'IN_PROGRESS'] },
        dueDate: {
          gte: new Date(),
          lte: nextWeek,
        },
      },
      include: {
        customer: {
          select: { id: true, name: true, customerCode: true },
        },
        opportunity: {
          select: { id: true, name: true },
        },
      },
      orderBy: { dueDate: 'asc' },
    });

    return {
      success: true,
      data: activities,
    };
  });
};

export { activityRoutes };
