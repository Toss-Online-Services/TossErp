import fastify, { FastifyInstance, FastifyRequest, FastifyReply } from 'fastify';
import { PrismaClient } from '@prisma/client';
import Redis from 'ioredis';
import { ServiceConfig } from './types';
import { createLogger } from './logger';
import { createJWTPlugin } from './jwt';
import { createValidationPlugin } from './validation';
import { createErrorHandler } from './errors';

export interface ServiceContext {
  config: ServiceConfig;
  logger: any;
  prisma: PrismaClient;
  redis: Redis;
  server: FastifyInstance;
}

export async function createService(config: ServiceConfig): Promise<ServiceContext> {
  const logger = createLogger(config);
  
  // Create Fastify instance
  const server = fastify({
    logger: config.logging.prettyPrint ? {
      transport: {
        target: 'pino-pretty',
        options: {
          colorize: true,
          translateTime: 'SYS:standard',
          ignore: 'pid,hostname',
        },
      },
    } : true,
    trustProxy: true,
  });

  // Initialize Prisma
  const prisma = new PrismaClient({
    log: config.environment === 'development' ? ['query', 'info', 'warn', 'error'] : ['info', 'warn', 'error'],
    datasources: {
      db: {
        url: config.database.url,
      },
    },
  });

  // Initialize Redis
  const redis = new Redis({
    host: config.redis.host,
    port: config.redis.port,
    password: config.redis.password,
    db: config.redis.db,
    enableReadyCheck: false,
    maxRetriesPerRequest: null,
  });

  // Register plugins
  await server.register(require('@fastify/helmet'), {
    contentSecurityPolicy: false,
  });

  await server.register(require('@fastify/cors'), {
    origin: config.cors.origins,
    credentials: config.cors.credentials,
  });

  await server.register(require('@fastify/rate-limit'), {
    max: config.rateLimit.max,
    timeWindow: config.rateLimit.timeWindow,
    redis,
  });

  await server.register(require('@fastify/multipart'), {
    limits: {
      fieldNameSize: 100,
      fieldSize: 100,
      fields: 10,
      fileSize: 10 * 1024 * 1024, // 10MB
      files: 5,
      headerPairs: 2000,
    },
  });

  // Register custom plugins
  await server.register(createJWTPlugin(config));
  await server.register(createValidationPlugin());

  // Add Prisma to Fastify instance
  server.decorate('prisma', prisma);

  // Register Swagger if enabled
  if (config.swagger.enabled) {
    await server.register(require('@fastify/swagger'), {
      swagger: {
        info: {
          title: config.swagger.title,
          description: config.swagger.description,
          version: config.swagger.version,
        },
        host: `localhost:${config.port}`,
        schemes: ['http', 'https'],
        consumes: ['application/json'],
        produces: ['application/json'],
        securityDefinitions: {
          bearerAuth: {
            type: 'apiKey',
            name: 'Authorization',
            in: 'header',
            description: 'JWT token with Bearer prefix',
          },
        },
      },
    });

    await server.register(require('@fastify/swagger-ui'), {
      routePrefix: '/docs',
      uiConfig: {
        docExpansion: 'full',
        deepLinking: false,
      },
      staticCSP: true,
      transformStaticCSP: (header) => header,
    });
  }

  // Register error handler
  server.setErrorHandler(createErrorHandler(logger));

  // Health check endpoint
  server.get('/health', {
    schema: {
      description: 'Health check endpoint',
      tags: ['Health'],
      response: {
        200: {
          type: 'object',
          properties: {
            status: { type: 'string' },
            service: { type: 'string' },
            timestamp: { type: 'string' },
            uptime: { type: 'number' },
            database: { type: 'string' },
            redis: { type: 'string' },
          },
        },
      },
    },
  }, async (request: FastifyRequest, reply: FastifyReply) => {
    try {
      // Check database connection
      await prisma.$queryRaw`SELECT 1`;
      const dbStatus = 'connected';

      // Check Redis connection
      await redis.ping();
      const redisStatus = 'connected';

      return {
        status: 'healthy',
        service: config.name,
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        database: dbStatus,
        redis: redisStatus,
      };
    } catch (error) {
      logger.error('Health check failed', error);
      reply.status(503);
      return {
        status: 'unhealthy',
        service: config.name,
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        database: 'disconnected',
        redis: 'disconnected',
        error: error instanceof Error ? error.message : 'Unknown error',
      };
    }
  });

  // Metrics endpoint
  server.get('/metrics', {
    schema: {
      description: 'Service metrics endpoint',
      tags: ['Metrics'],
      response: {
        200: {
          type: 'object',
          properties: {
            service: { type: 'string' },
            timestamp: { type: 'string' },
            uptime: { type: 'number' },
            memory: { type: 'object' },
            cpu: { type: 'object' },
            connections: { type: 'number' },
          },
        },
      },
    },
  }, async (request: FastifyRequest, reply: FastifyReply) => {
    const memoryUsage = process.memoryUsage();
    const cpuUsage = process.cpuUsage();

    return {
      service: config.name,
      timestamp: new Date().toISOString(),
      uptime: process.uptime(),
      memory: {
        rss: memoryUsage.rss,
        heapTotal: memoryUsage.heapTotal,
        heapUsed: memoryUsage.heapUsed,
        external: memoryUsage.external,
        arrayBuffers: memoryUsage.arrayBuffers,
      },
      cpu: {
        user: cpuUsage.user,
        system: cpuUsage.system,
      },
      connections: server.server.connections || 0,
    };
  });

  // Ready endpoint
  server.get('/ready', {
    schema: {
      description: 'Readiness check endpoint',
      tags: ['Health'],
      response: {
        200: {
          type: 'object',
          properties: {
            status: { type: 'string' },
            service: { type: 'string' },
            timestamp: { type: 'string' },
          },
        },
      },
    },
  }, async (request: FastifyRequest, reply: FastifyReply) => {
    return {
      status: 'ready',
      service: config.name,
      timestamp: new Date().toISOString(),
    };
  });

  // Graceful shutdown
  const gracefulShutdown = async () => {
    logger.info('Received shutdown signal, closing connections...');
    
    try {
      await server.close();
      await prisma.$disconnect();
      await redis.quit();
      logger.info('All connections closed. Goodbye!');
      process.exit(0);
    } catch (error) {
      logger.error('Error during shutdown:', error);
      process.exit(1);
    }
  };

  process.on('SIGTERM', gracefulShutdown);
  process.on('SIGINT', gracefulShutdown);

  return {
    config,
    logger,
    prisma,
    redis,
    server,
  };
}

export async function startService(context: ServiceContext): Promise<void> {
  const { config, logger, server } = context;

  try {
    await server.listen({ 
      port: config.port, 
      host: config.host 
    });
    
    logger.info(`🚀 ${config.name} service started`);
    logger.info(`🌍 Server listening on http://${config.host}:${config.port}`);
    
    if (config.swagger.enabled) {
      logger.info(`📚 API documentation available at http://${config.host}:${config.port}/docs`);
    }
  } catch (error) {
    logger.error('Failed to start service:', error);
    process.exit(1);
  }
}

// Utility function to create a basic CRUD controller
export function createCRUDController<T extends { id: string }>(
  modelName: string,
  prismaModel: any,
  schema: any
) {
  return {
    // Create
    async create(request: FastifyRequest<{ Body: Omit<T, 'id' | 'createdAt' | 'updatedAt'> }>) {
      const data = request.body;
      const result = await prismaModel.create({
        data: {
          ...data,
          id: require('uuid').v4(),
        },
      });
      return { success: true, data: result };
    },

    // Read (list with pagination)
    async list(request: FastifyRequest<{ Querystring: any }>) {
      const { page = 1, limit = 10, sortBy = 'createdAt', sortOrder = 'desc', search } = request.query;
      
      const skip = (page - 1) * limit;
      const where = search ? {
        OR: [
          { name: { contains: search, mode: 'insensitive' } },
          { description: { contains: search, mode: 'insensitive' } },
        ],
      } : {};

      const [items, total] = await Promise.all([
        prismaModel.findMany({
          where,
          skip,
          take: limit,
          orderBy: { [sortBy]: sortOrder },
        }),
        prismaModel.count({ where }),
      ]);

      return {
        success: true,
        data: items,
        meta: {
          total,
          page,
          limit,
          hasNext: skip + limit < total,
          hasPrev: page > 1,
        },
      };
    },

    // Read (single item)
    async get(request: FastifyRequest<{ Params: { id: string } }>) {
      const { id } = request.params;
      const result = await prismaModel.findUnique({
        where: { id },
      });

      if (!result) {
        throw new Error(`${modelName} not found`);
      }

      return { success: true, data: result };
    },

    // Update
    async update(request: FastifyRequest<{ 
      Params: { id: string };
      Body: Partial<Omit<T, 'id' | 'createdAt' | 'updatedAt'>>;
    }>) {
      const { id } = request.params;
      const data = request.body;

      const result = await prismaModel.update({
        where: { id },
        data: {
          ...data,
          updatedAt: new Date(),
        },
      });

      return { success: true, data: result };
    },

    // Delete
    async delete(request: FastifyRequest<{ Params: { id: string } }>) {
      const { id } = request.params;
      
      await prismaModel.delete({
        where: { id },
      });

      return { success: true, message: `${modelName} deleted successfully` };
    },
  };
}

export default {
  createService,
  startService,
  createCRUDController,
};
