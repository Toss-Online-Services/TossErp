# TOSS ERP III - Development Setup Guide

## Prerequisites

Before setting up the development environment, ensure you have the following installed:

1. **Node.js (v18 or higher)**
   - Download from: https://nodejs.org/
   - Verify installation: `node --version` and `npm --version`

2. **Docker & Docker Compose**
   - Download from: https://www.docker.com/get-started
   - Verify installation: `docker --version` and `docker-compose --version`

3. **Git**
   - Download from: https://git-scm.com/
   - Verify installation: `git --version`

## Quick Setup with Docker

If you prefer to use Docker for development (recommended for consistency):

```bash
# Navigate to project root
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp

# Start the development environment
docker-compose -f docker-compose.dev.yml up -d

# This will start:
# - PostgreSQL database
# - Redis cache
# - All microservices in development mode
```

## Local Development Setup

### 1. Install Node.js Dependencies

```bash
# Navigate to backend directory
cd src/backend

# Install main backend dependencies
npm install

# Install auth service dependencies
cd services/auth
npm install
npx prisma generate
npx prisma db push

# Install other services (repeat for each service)
cd ../crm
npm install
# ... repeat for other services
```

### 2. Environment Configuration

Create `.env` files in each service directory:

**src/backend/services/auth/.env:**
```env
DATABASE_URL="postgresql://postgres:password@localhost:5432/toss_auth"
JWT_SECRET="your-super-secret-jwt-key-here"
REFRESH_TOKEN_SECRET="your-refresh-token-secret-here"
REDIS_URL="redis://localhost:6379"
EMAIL_FROM="noreply@toss-erp.com"
EMAIL_SMTP_HOST="smtp.gmail.com"
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USER="your-email@gmail.com"
EMAIL_SMTP_PASS="your-app-password"
```

### 3. Database Setup

```bash
# Start PostgreSQL (if not using Docker)
# Create databases for each service
createdb toss_auth
createdb toss_crm
createdb toss_inventory
# ... etc for each service

# Run Prisma migrations for each service
cd services/auth
npx prisma db push
npx prisma generate

cd ../crm
npx prisma db push
npx prisma generate
# ... repeat for other services
```

### 4. Start Development Servers

```bash
# From the main backend directory
npm run dev

# This will start all services concurrently:
# - Auth Service: http://localhost:3001
# - CRM Service: http://localhost:3002
# - Inventory Service: http://localhost:3003
# - ... etc
```

### 5. Frontend Development

```bash
# Navigate to frontend directory
cd toss-web

# Install dependencies
npm install

# Start development server
npm run dev

# Frontend will be available at: http://localhost:3000
```

## Docker Development Environment

For easier development, use the provided Docker configuration:

**docker-compose.dev.yml** (create this file):
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
      POSTGRES_DB: toss_erp
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  auth-service:
    build:
      context: .
      dockerfile: src/backend/services/auth/Dockerfile.dev
    ports:
      - "3001:3001"
    environment:
      DATABASE_URL: "postgresql://postgres:password@postgres:5432/toss_erp"
      REDIS_URL: "redis://redis:6379"
    depends_on:
      - postgres
      - redis
    volumes:
      - ./src/backend/services/auth:/app
      - /app/node_modules

  # Add other services as needed...

volumes:
  postgres_data:
```

## API Documentation

Once the services are running, you can access the API documentation:

- Auth Service: http://localhost:3001/docs
- CRM Service: http://localhost:3002/docs
- Inventory Service: http://localhost:3003/docs
- ... etc

## Development Workflow

1. **Code Changes**: Make changes to service code
2. **Auto-Reload**: Services will automatically reload on code changes
3. **Database Changes**: Run `npx prisma db push` to apply schema changes
4. **Testing**: Run `npm test` in each service directory
5. **API Testing**: Use the Swagger docs or tools like Postman

## Troubleshooting

### Common Issues

1. **Port Conflicts**: If ports are already in use, modify the port numbers in package.json scripts
2. **Database Connection**: Ensure PostgreSQL is running and connection string is correct
3. **Redis Connection**: Ensure Redis is running and accessible
4. **Node.js Version**: Ensure you're using Node.js v18 or higher

### Windows-Specific Setup

If you're on Windows and encounter issues:

1. **Use PowerShell as Administrator**
2. **Enable Windows Subsystem for Linux (WSL)** for better Docker performance
3. **Install Node.js via Chocolatey**: `choco install nodejs`
4. **Use Git Bash** for better terminal compatibility

### Docker Issues

If Docker isn't starting properly:

1. **Enable Hyper-V** (Windows Pro/Enterprise) or **WSL 2** (Windows Home)
2. **Increase Docker memory allocation** to at least 4GB
3. **Check Docker Desktop settings** for WSL integration

## Next Steps

After setup is complete:

1. **Test the APIs** using the Swagger documentation
2. **Run the test suites** to ensure everything is working
3. **Explore the codebase** to understand the architecture
4. **Start developing features** based on the PRD requirements

## Support

If you encounter issues during setup:

1. Check the logs: `docker-compose logs [service-name]`
2. Verify environment variables are set correctly
3. Ensure all dependencies are installed
4. Check that ports are not being used by other applications

For additional help, refer to the troubleshooting documentation in `docs/troubleshooting/`.
