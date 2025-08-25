import { FastifyInstance, FastifyPluginAsync } from 'fastify';
import { PrismaClient } from '@prisma/client';

const productRoutes: FastifyPluginAsync = async (fastify: FastifyInstance) => {
  const prisma = fastify.prisma as PrismaClient;

  // List products
  fastify.get('/', {
    schema: {
      description: 'List products with pagination and filtering',
      tags: ['Products'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 100, default: 10 },
          search: { type: 'string' },
          category: { type: 'string' },
          type: { type: 'string', enum: ['RAW_MATERIAL', 'COMPONENT', 'FINISHED_GOODS', 'SERVICE', 'DIGITAL', 'CONSUMABLE'] },
          status: { type: 'string', enum: ['ACTIVE', 'INACTIVE', 'DISCONTINUED', 'DRAFT'] },
          trackInventory: { type: 'boolean' },
          lowStock: { type: 'boolean' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { 
      page = 1, 
      limit = 10, 
      search,
      category,
      type,
      status,
      trackInventory,
      lowStock
    } = request.query as any;

    const { tenantId } = (request as any).user;
    const skip = (page - 1) * limit;

    const where: any = { tenantId };
    
    if (search) {
      where.OR = [
        { name: { contains: search, mode: 'insensitive' } },
        { sku: { contains: search, mode: 'insensitive' } },
        { description: { contains: search, mode: 'insensitive' } },
      ];
    }
    
    if (category) where.category = category;
    if (type) where.type = type;
    if (status) where.status = status;
    if (trackInventory !== undefined) where.trackInventory = trackInventory;

    // For low stock filter, we need to join with stock items
    let include: any = {
      stockLocations: {
        select: {
          id: true,
          quantityOnHand: true,
          quantityAvailable: true,
          reorderPoint: true,
          location: {
            select: { id: true, name: true, code: true },
          },
        },
      },
      _count: {
        select: { stockMovements: true },
      },
    };

    if (lowStock) {
      include = {
        ...include,
        stockLocations: {
          ...include.stockLocations,
          where: {
            quantityOnHand: {
              lte: { reorderPoint: true },
            },
          },
        },
      };
    }

    const [products, total] = await Promise.all([
      prisma.product.findMany({
        where,
        include,
        skip,
        take: limit,
        orderBy: { createdAt: 'desc' },
      }),
      prisma.product.count({ where }),
    ]);

    return {
      success: true,
      data: products,
      meta: {
        total,
        page,
        limit,
        hasNext: skip + limit < total,
        hasPrev: page > 1,
      },
    };
  });

  // Create product
  fastify.post('/', {
    schema: {
      description: 'Create a new product',
      tags: ['Products'],
      security: [{ bearerAuth: [] }],
      body: {
        type: 'object',
        required: ['sku', 'name', 'type'],
        properties: {
          sku: { type: 'string', minLength: 1 },
          name: { type: 'string', minLength: 1 },
          description: { type: 'string' },
          category: { type: 'string' },
          subcategory: { type: 'string' },
          brand: { type: 'string' },
          manufacturer: { type: 'string' },
          model: { type: 'string' },
          type: { type: 'string', enum: ['RAW_MATERIAL', 'COMPONENT', 'FINISHED_GOODS', 'SERVICE', 'DIGITAL', 'CONSUMABLE'] },
          unitPrice: { type: 'number', minimum: 0 },
          costPrice: { type: 'number', minimum: 0 },
          currency: { type: 'string', default: 'ZAR' },
          weight: { type: 'number', minimum: 0 },
          dimensions: { type: 'string' },
          baseUnit: { type: 'string', default: 'EACH' },
          trackInventory: { type: 'boolean', default: true },
          isSharedResource: { type: 'boolean', default: false },
          shareableQuantity: { type: 'number', minimum: 0 },
          groupBuyingEligible: { type: 'boolean', default: false },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId, userId } = (request as any).user;
    const productData = request.body as any;

    // Check if SKU already exists for this tenant
    const existingSku = await prisma.product.findFirst({
      where: { sku: productData.sku, tenantId },
    });

    if (existingSku) {
      reply.status(400);
      return { success: false, error: 'SKU already exists' };
    }

    const product = await prisma.product.create({
      data: {
        ...productData,
        tenantId,
        createdBy: userId,
        status: 'ACTIVE',
      },
      include: {
        stockLocations: {
          include: {
            location: {
              select: { id: true, name: true, code: true },
            },
          },
        },
      },
    });

    reply.status(201);
    return { success: true, data: product };
  });

  // Get product by ID
  fastify.get('/:id', {
    schema: {
      description: 'Get product by ID',
      tags: ['Products'],
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

    const product = await prisma.product.findFirst({
      where: { id, tenantId },
      include: {
        stockLocations: {
          include: {
            location: true,
          },
        },
        suppliers: {
          where: { isActive: true },
        },
        stockMovements: {
          take: 10,
          orderBy: { movementDate: 'desc' },
          include: {
            location: {
              select: { id: true, name: true, code: true },
            },
          },
        },
      },
    });

    if (!product) {
      reply.status(404);
      return { success: false, error: 'Product not found' };
    }

    return { success: true, data: product };
  });

  // Update product
  fastify.put('/:id', {
    schema: {
      description: 'Update product',
      tags: ['Products'],
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
          category: { type: 'string' },
          subcategory: { type: 'string' },
          brand: { type: 'string' },
          manufacturer: { type: 'string' },
          model: { type: 'string' },
          type: { type: 'string', enum: ['RAW_MATERIAL', 'COMPONENT', 'FINISHED_GOODS', 'SERVICE', 'DIGITAL', 'CONSUMABLE'] },
          status: { type: 'string', enum: ['ACTIVE', 'INACTIVE', 'DISCONTINUED', 'DRAFT'] },
          unitPrice: { type: 'number', minimum: 0 },
          costPrice: { type: 'number', minimum: 0 },
          weight: { type: 'number', minimum: 0 },
          dimensions: { type: 'string' },
          trackInventory: { type: 'boolean' },
          isSharedResource: { type: 'boolean' },
          shareableQuantity: { type: 'number', minimum: 0 },
          groupBuyingEligible: { type: 'boolean' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { id } = request.params as { id: string };
    const { tenantId } = (request as any).user;
    const updateData = request.body as any;

    const product = await prisma.product.update({
      where: { id, tenantId },
      data: {
        ...updateData,
        updatedAt: new Date(),
      },
      include: {
        stockLocations: {
          include: {
            location: {
              select: { id: true, name: true, code: true },
            },
          },
        },
      },
    });

    return { success: true, data: product };
  });

  // Delete product
  fastify.delete('/:id', {
    schema: {
      description: 'Delete product',
      tags: ['Products'],
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

    // Check if product has stock movements
    const movementCount = await prisma.stockMovement.count({
      where: { productId: id, tenantId },
    });

    if (movementCount > 0) {
      reply.status(400);
      return { 
        success: false, 
        error: 'Cannot delete product with existing stock movements. Consider marking it as inactive instead.' 
      };
    }

    await prisma.product.delete({
      where: { id, tenantId },
    });

    return { success: true, message: 'Product deleted successfully' };
  });

  // Get product categories
  fastify.get('/meta/categories', {
    schema: {
      description: 'Get all product categories',
      tags: ['Products'],
      security: [{ bearerAuth: [] }],
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { tenantId } = (request as any).user;

    const categories = await prisma.product.findMany({
      where: { tenantId, category: { not: null } },
      select: { category: true, subcategory: true },
      distinct: ['category', 'subcategory'],
    });

    const groupedCategories = categories.reduce((acc: any, item) => {
      if (!acc[item.category!]) {
        acc[item.category!] = [];
      }
      if (item.subcategory && !acc[item.category!].includes(item.subcategory)) {
        acc[item.category!].push(item.subcategory);
      }
      return acc;
    }, {});

    return { success: true, data: groupedCategories };
  });

  // Get low stock products
  fastify.get('/reports/low-stock', {
    schema: {
      description: 'Get products with low stock levels',
      tags: ['Products'],
      security: [{ bearerAuth: [] }],
      querystring: {
        type: 'object',
        properties: {
          locationId: { type: 'string' },
        },
      },
    },
    preHandler: [fastify.authenticate],
  }, async (request, reply) => {
    const { locationId } = request.query as any;
    const { tenantId } = (request as any).user;

    const where: any = {
      product: { tenantId },
      quantityOnHand: {
        lte: prisma.stockItem.fields.reorderPoint,
      },
    };

    if (locationId) {
      where.locationId = locationId;
    }

    const lowStockItems = await prisma.stockItem.findMany({
      where,
      include: {
        product: {
          select: {
            id: true,
            sku: true,
            name: true,
            category: true,
            type: true,
          },
        },
        location: {
          select: {
            id: true,
            name: true,
            code: true,
          },
        },
      },
      orderBy: [
        { quantityOnHand: 'asc' },
        { product: { name: 'asc' } },
      ],
    });

    return { success: true, data: lowStockItems };
  });
};

export { productRoutes };
