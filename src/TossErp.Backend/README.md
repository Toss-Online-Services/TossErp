# TOSS ERP Backend

## Overview
.NET Core microservices backend for TOSS ERP III - Inventory Management System

## Architecture
Following Clean Architecture and eShopOnContainers patterns:

```
TossErp.Api/              # Web API Layer
├── Controllers/           # API Controllers
├── Middleware/           # Custom middleware
└── Program.cs           # Application entry point

TossErp.Application/       # Application Layer
├── Services/             # Application services
├── DTOs/                 # Data Transfer Objects
├── Interfaces/           # Repository interfaces
└── Validators/           # Input validation

TossErp.Domain/           # Domain Layer
├── Entities/             # Domain entities
├── Enums/                # Domain enums
├── Exceptions/           # Domain exceptions
└── ValueObjects/         # Value objects

TossErp.Infrastructure/   # Infrastructure Layer
├── Data/                 # Data access
├── Repositories/         # Repository implementations
├── Services/             # External services
└── Configuration/        # App configuration
```

## Features (MVP)
- Stock item management (CRUD operations)
- Stock movement tracking
- Category management
- Real-time stock level monitoring
- Low-stock alerts and notifications
- RESTful API endpoints

## Technology Stack
- **Framework:** .NET 8
- **Database:** PostgreSQL with Entity Framework Core
- **Authentication:** JWT tokens
- **API Documentation:** Swagger/OpenAPI
- **Logging:** Serilog
- **Testing:** xUnit

## Getting Started
1. Install .NET 8 SDK
2. Run `dotnet restore`
3. Run `dotnet build`
4. Run `dotnet run --project src/TossErp.Api`

## Development Phases
1. **Phase 1:** Basic API structure and domain entities
2. **Phase 2:** Database integration and repositories
3. **Phase 3:** Authentication and authorization
4. **Phase 4:** Advanced features and optimization

## API Endpoints
- `GET /api/stock-items` - Get all stock items
- `GET /api/stock-items/{id}` - Get stock item by ID
- `POST /api/stock-items` - Create new stock item
- `PUT /api/stock-items/{id}` - Update stock item
- `DELETE /api/stock-items/{id}` - Delete stock item
- `GET /api/stock-movements` - Get stock movements
- `POST /api/stock-movements` - Record stock movement 