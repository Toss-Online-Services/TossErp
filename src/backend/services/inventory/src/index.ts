import { createService, startService } from '../../../shared/service';
import { productRoutes } from './routes/products';
import { stockRoutes } from './routes/stock';
import { locationRoutes } from './routes/locations';
import { purchaseOrderRoutes } from './routes/purchase-orders';

async function main() {
  const serviceContext = await createService({
    name: 'inventory-service',
    port: parseInt(process.env.PORT || '3003'),
    host: '0.0.0.0',
    environment: (process.env.NODE_ENV as 'development' | 'staging' | 'production') || 'development',
    database: {
      url: process.env.DATABASE_URL || 'postgresql://postgres:password@localhost:5432/toss_inventory',
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
      title: 'TOSS ERP III - Inventory API',
      description: 'Inventory Management API for TOSS ERP III',
      version: '1.0.0',
    },
    logging: {
      level: 'info',
      prettyPrint: process.env.NODE_ENV === 'development',
    },
  });

  const { server } = serviceContext;

  // Register routes
  await server.register(productRoutes, { prefix: '/api/v1/products' });
  await server.register(stockRoutes, { prefix: '/api/v1/stock' });
  await server.register(locationRoutes, { prefix: '/api/v1/locations' });
  await server.register(purchaseOrderRoutes, { prefix: '/api/v1/purchase-orders' });

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
  console.error('Failed to start inventory service:', error);
  process.exit(1);
});
