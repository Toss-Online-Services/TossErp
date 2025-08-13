# Stock Service

## Overview
The Stock Service is a microservice responsible for managing inventory, stock levels, and item catalog operations in the TossErp system.

## Architecture
- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **CQRS Pattern**: Separate read and write models for optimal performance
- **Event-Driven**: Uses domain events for loose coupling
- **API-First**: RESTful API with OpenAPI documentation

## Structure
```
stock-service/
├── src/
│   ├── Stock.API/           # Presentation layer (controllers, endpoints)
│   ├── Stock.Application/   # Application layer (use cases, DTOs)
│   ├── Stock.Domain/        # Domain layer (entities, value objects)
│   ├── Stock.Infrastructure/# Infrastructure layer (data access, external services)
│   └── Stock.Agent/         # AI-powered automation and insights
├── tests/
│   ├── Stock.API.Tests/     # Integration tests
│   ├── Stock.Application.Tests/
│   ├── Stock.Domain.Tests/
│   └── Stock.Infrastructure.Tests/
├── Dockerfile               # Container configuration
├── docker-compose.yml       # Local development setup
└── README.md               # This file
```

## Features
- **Item Management**: CRUD operations for inventory items
- **Stock Tracking**: Real-time stock level monitoring
- **AI Integration**: LangChain-powered automation and insights
- **Event Publishing**: Domain events for system integration
- **Health Checks**: Built-in health monitoring

## API Endpoints
- `GET /api/items` - List all items
- `POST /api/items` - Create new item
- `GET /api/items/{id}` - Get item by ID
- `PUT /api/items/{id}` - Update item
- `DELETE /api/items/{id}` - Delete item
- `POST /api/ai/query` - AI-powered natural language queries
- `POST /api/ai/automate` - Automated stock operations

## Development
```bash
# Run locally
dotnet run --project src/Stock.API

# Run with Docker
docker-compose up

# Run tests
dotnet test
```

## Configuration
- Database connection string via `ConnectionStrings:DefaultConnection`
- AI service configuration via `AIService` section
- Logging configuration via `Logging` section

## Dependencies
- ASP.NET Core 8.0
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Serilog
- LangChain.NET
