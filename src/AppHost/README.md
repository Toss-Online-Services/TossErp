# TossERP AppHost - Modernized Implementation

This AppHost has been updated to follow the .NET Aspire best practices from the eShop reference architecture.

## Key Improvements

### Infrastructure
- **PostgreSQL**: Added with pgvector support for AI/vector operations
- **RabbitMQ**: Message broker with management plugin and data persistence
- **Redis**: Cache with data persistence
- **Forwarded Headers**: Automatic configuration for production deployments

### Service Architecture
- **Identity API**: Core authentication service with external endpoints
- **Business Services**: CRM, HR, Sales, Stock, Financial, Logistics, AI services
- **Support Services**: Assets, Accounts, Projects, Collaboration, Setup
- **Background Processors**: Stock processor for async operations
- **Gateway**: Central entry point with service references

### Best Practices Implemented

1. **Dependency Management**: Services wait for dependencies using `.WaitFor()`
2. **Environment Variables**: Proper Identity URL configuration across services
3. **Database Segregation**: Separate databases for different domains
4. **Port Management**: Consistent port allocation strategy
5. **Launch Profiles**: HTTP/HTTPS configuration based on environment
6. **AI Integration**: Optional OpenAI and Ollama support
7. **Service References**: Proper cross-service communication setup

### Service Dependencies

```
Infrastructure (PostgreSQL, RabbitMQ, Redis)
    ↓
Identity API
    ↓
Business Services (CRM, HR, Sales, etc.)
    ↓
Gateway (orchestrates all services)
```

### Configuration

The AppHost uses:
- Environment variables for service discovery
- Connection strings for database access
- Service references for inter-service communication
- Launch profiles for development/production modes

### Running the Application

```bash
cd src/AppHost
dotnet run
```

This will start all services in the proper dependency order with health checks and monitoring.

## Service Ports

| Service | Port | Description |
|---------|------|-------------|
| Identity API | 5000 | Authentication & Authorization |
| Setup API | 5010 | System Configuration |
| Assets API | 5020 | Asset Management |
| Accounts API | 5030 | Account Management |
| Projects API | 5040 | Project Management |
| Sales API | 5050 | Sales Operations |
| CRM API | 5060 | Customer Relationship |
| Collaboration API | 5070 | Team Collaboration |
| Financial API | 5080 | Financial Services |
| HR API | 5090 | Human Resources |
| AI API | 5100 | AI Services |
| Logistics API | 5110 | Supply Chain |
| Gateway | 8081 | API Gateway |

## Development Notes

- The `TOSSERP_USE_HTTP_ENDPOINTS=1` environment variable forces HTTP for testing
- Services are configured with proper health checks and readiness probes
- AI integration is optional and can be enabled by setting flags in Program.cs
- All services use the same database connection pattern with separate databases per domain
