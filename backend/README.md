# TOSS ERP Backend API

## Overview

TOSS (Township One-Stop Solution) ERP III backend API built with .NET 9, following Clean Architecture principles with CQRS pattern.

## Architecture

```
src/
├── TossErp.API/              # Presentation Layer (Controllers, Middleware)
├── TossErp.Application/      # Application Layer (CQRS Handlers, DTOs)
├── TossErp.Domain/           # Domain Layer (Entities, Business Logic, Events)
└── TossErp.Infrastructure/   # Infrastructure Layer (Data Access, External Services)
```

## Tech Stack

- **.NET 9** - Latest .NET framework
- **Entity Framework Core 9** - ORM with PostgreSQL
- **PostgreSQL 16** - Primary database
- **Redis** - Distributed caching
- **MediatR** - CQRS implementation
- **FluentValidation** - Request validation
- **Swashbuckle** - API documentation
- **JWT** - Authentication

## Prerequisites

- .NET 9 SDK
- PostgreSQL 16+
- Redis 7+
- Docker & Docker Compose (optional)

## Environment Variables

Create an `appsettings.Development.json` or configure environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tosserp;Username=postgres;Password=YOUR_PASSWORD",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "YOUR-SUPER-SECRET-KEY-MIN-32-CHARACTERS",
    "Issuer": "TossErpAPI",
    "Audience": "TossErpClient",
    "ExpirationMinutes": 60
  },
  "AllowedOrigins": [
    "http://localhost:3000"
  ]
}
```

## Getting Started

### Option 1: Run with Docker Compose

```bash
# Start all services (PostgreSQL, Redis, API)
docker-compose up -d

# View logs
docker-compose logs -f tosserp-api

# Stop all services
docker-compose down
```

The API will be available at `http://localhost:5000`

### Option 2: Run Locally

1. **Install Dependencies**
   ```bash
   dotnet restore
   ```

2. **Start PostgreSQL and Redis**
   ```bash
   # Using Docker
   docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tosserp postgres:16-alpine
   docker run -d -p 6379:6379 redis:7-alpine
   ```

3. **Apply Migrations**
   ```bash
   cd src/TossErp.API
   dotnet ef database update --project ../TossErp.Infrastructure
   ```

4. **Run the API**
   ```bash
   dotnet run --project src/TossErp.API
   ```

The API will be available at `http://localhost:5000`
Swagger UI at `http://localhost:5000` (root)

## Core Modules Implemented

### ✅ Sales Management
- **Endpoints**: `/api/sales/*`
- Sales transactions, payments, customer management
- Sales analytics and summaries

### ✅ Inventory Management
- **Endpoints**: `/api/inventory/*`, `/api/products/*`
- Product catalog, stock levels, warehouses
- Stock movements and transfers

### ✅ Finance & Accounting
- **Endpoints**: `/api/finance/*`
- Chart of accounts, journal entries
- Balance sheet, income statement reports

### ✅ Procurement
- **Endpoints**: `/api/procurement/*`
- Supplier management, purchase orders
- Supplier performance tracking

### ✅ Human Resources
- **Endpoints**: `/api/hr/*`
- Employee management, leave requests
- Attendance tracking

### ✅ Customer Management
- **Endpoints**: `/api/customers/*`
- Customer profiles, loyalty programs
- Customer analytics

## API Documentation

Once running, visit `http://localhost:5000` for interactive Swagger documentation.

### Key Endpoints

**Sales:**
- `GET /api/sales` - Get all sales
- `POST /api/sales` - Create new sale
- `POST /api/sales/{id}/complete` - Complete sale
- `GET /api/sales/summary` - Sales analytics

**Products:**
- `GET /api/products` - Get all products
- `POST /api/products` - Create product
- `GET /api/products/search/{code}` - Search by SKU/barcode
- `GET /api/products/{id}/stock` - Get stock levels

**Finance:**
- `GET /api/finance/accounts` - Chart of accounts
- `POST /api/finance/journal-entries` - Create journal entry
- `GET /api/finance/reports/balance-sheet` - Balance sheet
- `GET /api/finance/reports/income-statement` - P&L report

**Inventory:**
- `GET /api/inventory/stock-levels` - All stock levels
- `POST /api/inventory/stock-levels/{id}/adjust` - Adjust stock
- `POST /api/inventory/stock-movements/transfer` - Stock transfer
- `GET /api/inventory/warehouses` - Get warehouses

**HR:**
- `GET /api/hr/employees` - Get employees
- `POST /api/hr/employees` - Create employee
- `POST /api/hr/leave` - Create leave request
- `POST /api/hr/attendance` - Record attendance

## Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project src/TossErp.Infrastructure --startup-project src/TossErp.API --output-dir Data/Migrations

# Apply migrations
dotnet ef database update --project src/TossErp.Infrastructure --startup-project src/TossErp.API

# Remove last migration
dotnet ef migrations remove --project src/TossErp.Infrastructure --startup-project src/TossErp.API
```

## Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Health Checks

- **Health endpoint**: `GET /health`
- Checks PostgreSQL and Redis connectivity

## Security Features

- ✅ JWT-based authentication
- ✅ Role-based authorization
- ✅ CORS configuration
- ✅ HTTPS redirection
- ✅ Input validation
- ✅ Soft delete for data protection
- ✅ Audit trails (CreatedBy, UpdatedBy)
- ✅ SQL injection prevention (EF Core)

## Performance Features

- ✅ Redis distributed caching
- ✅ Database connection retry logic
- ✅ Efficient EF Core queries with projections
- ✅ Pagination support
- ✅ Asynchronous operations throughout
- ✅ Global query filters

## Development

### Code Structure

Follow Clean Architecture principles:

1. **Domain** - Core business logic, entities, events (no dependencies)
2. **Application** - Use cases, DTOs, interfaces
3. **Infrastructure** - Data access, external services
4. **API** - Controllers, middleware, presentation

### Adding a New Module

1. Create domain entities in `Domain/Entities/{ModuleName}/`
2. Create EF configurations in `Infrastructure/Data/Configurations/`
3. Add DbSets to `ApplicationDbContext`
4. Create controller in `API/Controllers/`
5. Create migration: `dotnet ef migrations add Add{ModuleName}`
6. Apply migration: `dotnet ef database update`

### Domain Events

Entities inherit from `BaseEntity` and can raise domain events:

```csharp
public void Complete()
{
    Status = SaleStatus.Completed;
    AddDomainEvent(new SaleCompletedEvent(Id, SaleNumber, TotalAmount));
}
```

## Deployment

### Production Checklist

- [ ] Update JWT secret key
- [ ] Configure production database connection
- [ ] Set up proper CORS origins
- [ ] Configure Redis connection
- [ ] Set up SSL certificates
- [ ] Configure logging (Application Insights, Serilog)
- [ ] Set up monitoring and alerting
- [ ] Configure backup strategy
- [ ] Review security headers
- [ ] Set up CI/CD pipeline

### Docker Build

```bash
# Build image
docker build -t tosserp-api:latest .

# Run container
docker run -d -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=tosserp;..." \
  tosserp-api:latest
```

## Future Enhancements

### Phase 4: Extended Modules (Planned)
- Manufacturing & Production Planning
- Supply Chain Management
- Project Management
- Warehouse Management System
- Marketing Automation
- E-commerce Integration

### Phase 5: Collaboration Features (Planned)
- Group Buying Networks
- Shared Logistics
- Asset Sharing
- Pooled Credit
- Community Forums

### Phase 6: AI Integration (Planned)
- Natural language query interface
- Predictive analytics
- Inventory optimization
- Pricing recommendations
- Demand forecasting

### Phase 7: Advanced Features (Planned)
- Multi-tenant support
- Event sourcing
- Message queue (RabbitMQ)
- Microservices migration path
- Mobile API optimization

## Support

For issues or questions, please contact the development team.

## License

Proprietary - TOSS ERP System

