import { FastifyInstance, FastifyPluginAsync } from 'fastify';
import { PrismaClient } from '@prisma/client';

const contactRoutes: FastifyPluginAsync = async (fastify: FastifyInstance) => {
  const prisma = fastify.prisma as PrismaClient;

  // List contacts
  fastify.get('/', {
    schema: {
      description: 'List contacts with pagination and filtering',
      tags: ['Contacts'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          customerId: { type: 'string', description: 'Filter by customer ID' },
          search: { type: 'string', description: 'Search in name, email' },
          sortBy: { type: 'string', default: 'createdAt' },
          sortOrder: { type: 'string', enum: ['asc', 'desc'], default: 'desc' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { 
      page = 1, 
      limit = 10, 
      customerId,
      search, 
      sortBy = 'createdAt',
      sortOrder = 'desc'
    } = request.query as any;

    const { tenantId } = (request as any).user;
    const skip = (page - 1) * limit;

    const where: any = { tenantId };
    
    if (customerId) where.customerId = customerId;
    
    if (search) {
      where.OR = [
        { firstName: { contains: search, mode: 'insensitive' } },
        { lastName: { contains: search, mode: 'insensitive' } },
        { email: { contains: search, mode: 'insensitive' } },
      ];
    }

    const [contacts, total] = await Promise.all([
      prisma.contact.findMany({
        where,
        include: {
          customer: {
            select: { id: true, name: true, customerCode: true },
          },
        },
        skip,
        take: limit,
        orderBy: { [sortBy]: sortOrder },
      }),
      prisma.contact.count({ where }),
    ]);

    return {
      success: true,
      data: contacts,
      meta: {
        total,
        page,
        limit,
        hasNext: skip + limit < total,
        hasPrev: page > 1,
      },
    };
  });

  // Create contact
  fastify.post('/', {
    schema: {
      description: 'Create a new contact',
      tags: ['Contacts'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['customerId', 'firstName', 'lastName'],
        properties: {
          customerId: { type: 'string' },
          firstName: { type: 'string', minLength: 1 },
          lastName: { type: 'string', minLength: 1 },
          email: { type: 'string', format: 'email' },
          phone: { type: 'string' },
          mobile: { type: 'string' },
          position: { type: 'string' },
          department: { type: 'string' },
          isPrimary: { type: 'boolean', default: false },
          preferredContact: { 
            type: 'string', 
            enum: ['EMAIL', 'PHONE', 'SMS', 'WHATSAPP', 'IN_PERSON'],
            default: 'EMAIL'
          },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId, userId } = (request as any).user;
    const contactData = request.body as any;

    const contact = await prisma.contact.create({
      data: {
        ...contactData,
        tenantId,
        fullName: `${contactData.firstName} ${contactData.lastName}`,
        createdBy: userId,
      },
      include: {
        customer: {
          select: { id: true, name: true, customerCode: true },
        },
      },
    });

    reply.status(201);
    return { success: true, data: contact };
  });

  // Get contact by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get contact by ID',
      tags: ['Contacts'],
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

    const contact = await prisma.contact.findFirst({
      where: { id, tenantId },
      include: {
        customer: true,
        activities: {
          take: 10,
          orderBy: { createdAt: 'desc' },
        },
        notes: {
          take: 5,
          orderBy: { createdAt: 'desc' },
        },
      },
    });

    if (!contact) {
      reply.status(404);
      return { success: false, error: 'Contact not found' };
    }

    return { success: true, data: contact };
  });

  // Update contact
  fastify.put('/:id', {
    schema: {
      description: 'Update contact information',
      tags: ['Contacts'],
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
          firstName: { type: 'string', minLength: 1 },
          lastName: { type: 'string', minLength: 1 },
          email: { type: 'string', format: 'email' },
          phone: { type: 'string' },
          mobile: { type: 'string' },
          position: { type: 'string' },
          department: { type: 'string' },
          isPrimary: { type: 'boolean' },
          preferredContact: { 
            type: 'string', 
            enum: ['EMAIL', 'PHONE', 'SMS', 'WHATSAPP', 'IN_PERSON']
          },
          isActive: { type: 'boolean' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;
    const updateData = request.body as any;

    // Update fullName if first or last name changed
    if (updateData.firstName || updateData.lastName) {
      const existingContact = await prisma.contact.findFirst({
        where: { id, tenantId },
        select: { firstName: true, lastName: true },
      });

      if (existingContact) {
        const firstName = updateData.firstName || existingContact.firstName;
        const lastName = updateData.lastName || existingContact.lastName;
        updateData.fullName = `${firstName} ${lastName}`;
      }
    }

    const contact = await prisma.contact.update({
      where: { id, tenantId },
      data: {
        ...updateData,
        updatedAt: new Date(),
      },
    });

    return { success: true, data: contact };
  });

  // Delete contact
  fastify.delete('/:id', {
    schema: {
      description: 'Delete contact',
      tags: ['Contacts'],
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

    await prisma.contact.delete({
      where: { id, tenantId },
    });

    return { success: true, message: 'Contact deleted successfully' };
  });
};

export { contactRoutes };
