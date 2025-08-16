# Stock Module

A comprehensive stock management system built with .NET 8, following Domain-Driven Design (DDD) principles and Clean Architecture patterns.

## 🏗️ Architecture Overview

The Stock module is organized into the following layers:

```
Stock/
├── Stock.API/           # Presentation layer - REST API endpoints
├── Stock.Application/   # Application layer - Use cases, commands, queries
├── Stock.Domain/        # Domain layer - Business logic, entities, aggregates
├── Stock.Infrastructure/# Infrastructure layer - Data access, external services
├── Stock.Agent/         # AI-powered features and intelligent automation
└── Stock.Processor/     # Background processing and scheduled tasks
```

### Core Components

- **Stock.API**: RESTful API endpoints for stock operations
- **Stock.Application**: Application services, command/query handlers, validation
- **Stock.Domain**: Domain entities, aggregates, value objects, business rules
- **Stock.Infrastructure**: Repository implementations, database context, external integrations
- **Stock.Agent**: AI-powered features including intelligent stock recommendations
- **Stock.Processor**: Background services for stock processing, alerts, and reconciliation

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- PostgreSQL 15+
- RabbitMQ 3.8+
- Redis 7+
- Docker & Docker Compose (for containerized deployment)

### Local Development

1. **Clone and navigate to the Stock module:**
   ```bash
   cd src/Services/Stock
   ```

2. **Start dependencies using Docker Compose:**
   ```bash
   docker-compose up -d
   ```

3. **Run the API:**
   ```bash
   cd Stock.API
   dotnet run
   ```

4. **Run the Processor:**
   ```bash
   cd Stock.Processor
   dotnet run
   ```

### Production Deployment

#### Docker Compose (Recommended for small to medium deployments)

1. **Set environment variables:**
   ```bash
   cp .env.prod.example .env.prod
   # Edit .env.prod with your production values
   ```

2. **Deploy using Docker Compose:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

#### Kubernetes (Recommended for large deployments)

1. **Deploy to Kubernetes:**
   ```bash
   cd deploy/kubernetes/stock
   ./deploy-stock.sh deploy
   ```

2. **Check deployment status:**
   ```bash
   ./deploy-stock.sh status
   ```

## 📚 API Reference

### Base URL
- **Local**: `http://localhost:5001`
- **Production**: `https://stock-api.tosserp.com`

### Authentication
All endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

### Endpoints

#### Items Management
- `GET /api/items` - List all items
- `GET /api/items/{id}` - Get item by ID
- `POST /api/items` - Create new item
- `PUT /api/items/{id}` - Update item
- `DELETE /api/items/{id}` - Delete item

#### Stock Operations
- `POST /api/stock-operations/issue` - Issue stock from warehouse
- `POST /api/stock-operations/receive` - Receive stock into warehouse
- `POST /api/stock-operations/transfer` - Transfer stock between locations
- `POST /api/stock-operations/adjust` - Adjust stock levels

#### Stock Levels
- `GET /api/stock-levels` - Get stock levels for all items
- `GET /api/stock-levels/{itemId}` - Get stock levels for specific item
- `GET /api/stock-levels/low-stock` - Get items with low stock

### Example API Usage

#### Create an Item
```bash
curl -X POST "https://stock-api.tosserp.com/api/items" \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "itemCode": "ITEM001",
    "itemName": "Sample Item",
    "description": "A sample item for testing",
    "itemGroup": "Electronics",
    "stockUOM": "PCS",
    "itemType": "Stock",
    "valuationMethod": "FIFO",
    "company": "TossErp"
  }'
```

#### Issue Stock
```bash
curl -X POST "https://stock-api.tosserp.com/api/stock-operations/issue" \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "itemId": "item-guid-here",
    "warehouseId": "warehouse-guid-here",
    "binId": "bin-guid-here",
    "quantity": 10,
    "reason": "Customer order",
    "reference": "ORD-001"
  }'
```

## 🏛️ Domain Model

### Core Aggregates

#### ItemAggregate
- **Purpose**: Manages item information and variants
- **Key Properties**: ItemCode, ItemName, Description, StockUOM, ItemType
- **Business Rules**: 
  - Item codes must be unique
  - Stock items must have a unit of measure
  - Items can have multiple variants

#### StockEntryAggregate
- **Purpose**: Manages stock movements and transactions
- **Key Properties**: EntryType, EntryDate, Status, Details
- **Business Rules**:
  - Stock entries must have valid item references
  - Quantities cannot be negative
  - Stock entries must be processed before affecting stock levels

#### StockLevelAggregate
- **Purpose**: Tracks current stock levels across locations
- **Key Properties**: ItemId, WarehouseId, BinId, Quantity, ReservedQuantity
- **Business Rules**:
  - Stock levels cannot go below zero
  - Reserved quantities cannot exceed available stock

### Value Objects

- **ItemCode**: Unique identifier for items
- **Quantity**: Numeric value with unit validation
- **Rate**: Monetary value with currency
- **UnitOfMeasure**: Standardized measurement units

### Domain Events

- `ItemCreatedEvent`: Fired when a new item is created
- `StockIssuedEvent`: Fired when stock is issued
- `StockReceivedEvent`: Fired when stock is received
- `LowStockAlertEvent`: Fired when stock levels fall below threshold

## 🔧 Configuration

### Application Settings

The Stock module can be configured through `appsettings.json` or environment variables:

```json
{
  "Stock": {
    "Agent": {
      "OpenAI": {
        "ApiKey": "your-api-key",
        "Model": "gpt-4",
        "MaxTokens": 4000
      }
    },
    "Processor": {
      "LowStockAlert": {
        "Enabled": true,
        "CheckIntervalMinutes": 15
      },
      "Reconciliation": {
        "Enabled": true,
        "CheckIntervalHours": 24
      },
      "Processing": {
        "Enabled": true,
        "BatchSize": 100,
        "MaxRetries": 3
      }
    }
  }
}
```

### Database Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=stock_db;Username=postgres;Password=password;",
    "EventBus": "Host=localhost;Port=5672;Username=guest;Password=guest"
  }
}
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Stock.Application.UnitTests/
dotnet test tests/Stock.Domain.UnitTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **Functional Tests**: Test complete workflows

## 📊 Monitoring & Observability

### Health Checks

The Stock module exposes health check endpoints:

- `/health/live` - Liveness probe
- `/health/ready` - Readiness probe
- `/health` - Comprehensive health status

### Metrics

Prometheus metrics are exposed at `/metrics`:

- `stock_operations_total` - Total stock operations
- `stock_level_current` - Current stock levels
- `stock_processing_duration_seconds` - Processing duration
- `low_stock_alerts_total` - Low stock alerts generated

### Logging

Structured logging using Serilog with correlation IDs for request tracing.

## 🔒 Security

### Authentication & Authorization

- JWT-based authentication
- Role-based access control
- API key validation for AI services

### Data Protection

- Input validation and sanitization
- SQL injection prevention through parameterized queries
- Sensitive data encryption at rest

## 🚨 Troubleshooting

### Common Issues

#### Database Connection Issues
```bash
# Check database connectivity
docker exec -it stock-postgres pg_isready -U postgres

# Check connection string
echo $ConnectionStrings__DefaultConnection
```

#### Event Bus Issues
```bash
# Check RabbitMQ status
docker exec -it stock-rabbitmq rabbitmq-diagnostics status

# Check queue status
docker exec -it stock-rabbitmq rabbitmqctl list_queues
```

#### Performance Issues
```bash
# Check API response times
curl -w "@curl-format.txt" -o /dev/null -s "https://stock-api.tosserp.com/api/items"

# Check database performance
docker exec -it stock-postgres psql -U postgres -d stock_db -c "SELECT * FROM pg_stat_activity;"
```

### Logs

```bash
# View API logs
docker logs stock-api

# View processor logs
docker logs stock-processor

# View database logs
docker logs stock-postgres
```

## 📈 Performance Tuning

### Database Optimization

- Use appropriate indexes on frequently queried columns
- Implement connection pooling
- Regular database maintenance and vacuuming

### Caching Strategy

- Redis caching for frequently accessed data
- In-memory caching for application-level data
- Cache invalidation on data changes

### Background Processing

- Configure appropriate batch sizes for stock processing
- Use multiple processor instances for high-volume scenarios
- Implement retry policies with exponential backoff

## 🔄 Deployment

### CI/CD Pipeline

The Stock module includes automated deployment pipelines:

1. **Build**: Compile and test the application
2. **Package**: Create Docker images
3. **Deploy**: Deploy to target environment
4. **Verify**: Run health checks and smoke tests

### Environment Promotion

- **Development**: Local development and testing
- **Staging**: Pre-production validation
- **Production**: Live production environment

### Rollback Strategy

- Maintain previous deployment versions
- Automated rollback on health check failures
- Database migration rollback procedures

## 📚 Additional Resources

- [Domain-Driven Design Guide](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/core/introduction)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

## 🤝 Contributing

1. Follow the established coding standards
2. Write comprehensive tests for new features
3. Update documentation for API changes
4. Follow the Git flow branching strategy

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../../LICENSE) file for details.
