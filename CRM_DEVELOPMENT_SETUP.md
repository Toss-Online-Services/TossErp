# CRM Service Development Setup

This guide explains how to set up and run the CRM service with the web client for development.

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+
- Docker and Docker Compose
- PostgreSQL (if running locally)

## Quick Start with Docker Compose

The easiest way to get everything running is with Docker Compose:

```bash
# Start all services (database, CRM service, web client)
docker-compose -f docker-compose.crm-dev.yml up -d

# View logs
docker-compose -f docker-compose.crm-dev.yml logs -f

# Stop all services
docker-compose -f docker-compose.crm-dev.yml down
```

This will start:
- PostgreSQL database on port 5432
- CRM service on port 5001
- Web client on port 3000

## Manual Development Setup

### 1. Database Setup

Start PostgreSQL database:
```bash
docker run -d \
  --name toss-crm-db \
  -e POSTGRES_DB=toss_crm \
  -e POSTGRES_USER=toss_user \
  -e POSTGRES_PASSWORD=toss_password \
  -p 5432:5432 \
  postgres:15-alpine
```

### 2. CRM Service

```bash
# Navigate to CRM API project
cd src/Services/Crm/Crm.API

# Restore dependencies
dotnet restore

# Apply database migrations (first time only)
dotnet ef database update

# Run the service
dotnet run
```

The CRM service will be available at `http://localhost:5001`

### 3. Web Client

```bash
# Navigate to web client
cd toss-web

# Copy environment variables
cp .env.example .env

# Install dependencies
npm install

# Start development server
npm run dev
```

The web client will be available at `http://localhost:3000`

## Service Configuration

### CRM Service

The CRM service is configured via `appsettings.json` and environment variables:

- **Database**: Connection string in `ConnectionStrings:DefaultConnection`
- **CORS**: Configured to allow requests from the web client
- **Swagger**: Available at `/swagger` in development

### Web Client

Configure the web client via `.env` file:

```env
CRM_SERVICE_URL=http://localhost:5001
NODE_ENV=development
```

## API Endpoints

### CRM Service

- `GET /api/customers` - List customers with optional filters
- `POST /api/customers` - Create a new customer
- `GET /api/customers/{id}` - Get customer by ID
- `PUT /api/customers/{id}` - Update customer
- `POST /api/customers/{id}/purchases` - Record a purchase
- `GET /api/customers/top` - Get top customers by spending
- `GET /api/customers/lapsed` - Get lapsed customers

### Web Client API Routes

- `GET /api/customers` - Proxy to CRM service (with fallback to mock data)
- `POST /api/customers` - Proxy to CRM service (with fallback to mock response)
- `GET /api/analytics` - CRM analytics dashboard data

## Development Features

### CRM Service

✅ **Implemented:**
- Clean Architecture pattern (API, Application, Domain, Infrastructure)
- CQRS with MediatR
- Entity Framework Core with PostgreSQL
- Repository pattern
- Comprehensive error handling
- Logging
- API validation
- Swagger documentation

### Web Client

✅ **Implemented:**
- Vue.js 3 with Composition API
- Server-side API routes
- Responsive CRM dashboard
- Customer management interface
- Real-time data fetching
- Error handling with fallbacks
- Mock data for development

## Database Schema

The CRM service uses Entity Framework Core with the following main entities:

- **Customer**: Core customer information
- **Purchase**: Customer purchase history
- **CustomerSegment**: Customer segmentation data

Migrations are automatically applied when using Docker Compose.

## Testing

### CRM Service Tests

```bash
cd src/Services/Crm/Crm.Tests
dotnet test
```

### Web Client Tests

```bash
cd toss-web
npm run test
```

## Troubleshooting

### Database Connection Issues

1. Ensure PostgreSQL is running
2. Check connection string in `appsettings.json`
3. Verify database credentials

### CRM Service Not Starting

1. Check .NET 8.0 SDK is installed
2. Verify all NuGet packages are restored
3. Check logs for detailed error messages

### Web Client Issues

1. Ensure Node.js 18+ is installed
2. Clear `node_modules` and reinstall: `rm -rf node_modules && npm install`
3. Check environment variables in `.env`

### Service Communication

If the web client can't reach the CRM service:

1. Verify CRM service is running on port 5001
2. Check CORS configuration in CRM service
3. Update `CRM_SERVICE_URL` in web client `.env` file

## Next Steps

1. **Authentication**: Add JWT authentication to both services
2. **Real-time Updates**: Implement SignalR for live data updates
3. **Testing**: Add comprehensive unit and integration tests
4. **Monitoring**: Add health checks and logging
5. **CI/CD**: Set up automated deployment pipelines

## Production Deployment

For production deployment, see the main project documentation for:
- Kubernetes configurations
- Environment-specific settings
- Security considerations
- Monitoring and logging setup
