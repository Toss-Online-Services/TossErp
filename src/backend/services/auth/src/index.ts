import dotenv from 'dotenv';
import { createService, startService } from '../../../shared/service';
import { createServiceConfig } from '../../../shared/types';
import { authRoutes } from './routes/auth';
import { userRoutes } from './routes/users';
import { tenantRoutes } from './routes/tenants';

// Load environment variables
dotenv.config();

async function main() {
  const config = createServiceConfig('auth-service', 3001);
  const context = await createService(config);

  // Register routes
  await context.server.register(authRoutes, { prefix: '/api/v1/auth' });
  await context.server.register(userRoutes, { prefix: '/api/v1/users' });
  await context.server.register(tenantRoutes, { prefix: '/api/v1/tenants' });

  // Start the service
  await startService(context);
}

main().catch((error) => {
  console.error('Failed to start auth service:', error);
  process.exit(1);
});
