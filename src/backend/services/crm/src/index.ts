import { createService, startService } from '../../../shared/service';
import { customerRoutes } from './routes/customers';
import { contactRoutes } from './routes/contacts';
import { opportunityRoutes } from './routes/opportunities';
import { activityRoutes } from './routes/activities';

async function main() {
  const serviceContext = await createService({
    name: 'crm-service',
    port: parseInt(process.env.PORT || '3002'),
    host: '0.0.0.0',
    environment: (process.env.NODE_ENV as 'development' | 'staging' | 'production') || 'development',
    database: {
      url: process.env.DATABASE_URL || 'postgresql://postgres:password@localhost:5432/toss_crm',
    },
    redis: {
      host: process.env.REDIS_HOST || 'localhost',
      port: parseInt(process.env.REDIS_PORT || '6379'),
      password: process.env.REDIS_PASSWORD,
      db: 0,
    },
    jwt: {
      secret: process.env.JWT_SECRET || 'dev-jwt-secret',
      expiresIn: '24h',
      refreshExpiresIn: '7d',
    },
    cors: {
      origins: ['http://localhost:3000', 'http://localhost:3100'],
      credentials: true,
    },
    rateLimit: {
      max: 100,
      timeWindow: '1 minute',
    },
    swagger: {
      enabled: true,
      title: 'TOSS ERP III - CRM API',
      description: 'Customer Relationship Management API for TOSS ERP III',
      version: '1.0.0',
    },
    logging: {
      level: 'info',
      prettyPrint: process.env.NODE_ENV === 'development',
    },
  });

  const { server } = serviceContext;

  // Register routes
  await server.register(customerRoutes, { prefix: '/api/v1/customers' });
  await server.register(contactRoutes, { prefix: '/api/v1/contacts' });
  await server.register(opportunityRoutes, { prefix: '/api/v1/opportunities' });
  await server.register(activityRoutes, { prefix: '/api/v1/activities' });

  // Start the service
  await startService(serviceContext);
}

// Handle unhandled rejections
process.on('unhandledRejection', (reason, promise) => {
  console.error('Unhandled Rejection at:', promise, 'reason:', reason);
  process.exit(1);
});

// Start the service
main().catch((error) => {
  console.error('Failed to start CRM service:', error);
  process.exit(1);
});
