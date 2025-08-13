# Shared Libraries

## Overview
The Shared Libraries contain common code, utilities, and configurations that are used across multiple microservices and clients in the TossErp system.

## Structure
```
shared/
├── service-defaults/     # Common service configurations
├── event-bus/           # Event-driven communication
├── common-libraries/    # Shared utilities and helpers
├── proto-definitions/   # gRPC protocol definitions
├── config-templates/    # Configuration templates
└── README.md           # This file
```

## Components

### Service Defaults
Common configurations and middleware for all microservices:
- **Health Checks**: Standard health check implementations
- **Logging**: Structured logging with Serilog
- **Authentication**: JWT authentication middleware
- **CORS**: Cross-origin resource sharing configuration
- **Error Handling**: Global exception handling
- **Metrics**: OpenTelemetry instrumentation

### Event Bus
Event-driven communication infrastructure:
- **Domain Events**: Base classes for domain events
- **Event Handlers**: Common event handling patterns
- **Event Publishing**: Event publishing abstractions
- **Event Subscriptions**: Event subscription management
- **Event Serialization**: Event serialization utilities

### Common Libraries
Reusable utilities and helpers:
- **Extensions**: Common extension methods
- **Validators**: Shared validation logic
- **Mappers**: Object mapping utilities
- **Caching**: Caching abstractions and implementations
- **HttpClient**: HTTP client factories and policies

### Protocol Definitions
gRPC and API definitions:
- **Proto Files**: Protocol Buffer definitions
- **API Contracts**: REST API contracts
- **DTOs**: Shared data transfer objects
- **Enums**: Common enumerations
- **Constants**: Shared constants

### Configuration Templates
Configuration templates for different environments:
- **Development**: Local development configurations
- **Staging**: Staging environment configurations
- **Production**: Production environment configurations
- **Docker**: Container-specific configurations
- **Kubernetes**: Kubernetes deployment configurations

## Usage

### Adding to a Microservice
```csharp
// Add service defaults
builder.AddServiceDefaults();

// Add event bus
builder.Services.AddEventBus();

// Add common libraries
builder.Services.AddCommonLibraries();
```

### Using Shared Components
```csharp
// Use common extensions
using TossErp.Shared.Extensions;

// Use event bus
using TossErp.Shared.EventBus;

// Use common validators
using TossErp.Shared.Validators;
```

## Development
```bash
# Build all shared libraries
dotnet build

# Run tests
dotnet test

# Pack for distribution
dotnet pack
```

## Versioning
- **Semantic Versioning**: Follows semantic versioning (MAJOR.MINOR.PATCH)
- **Breaking Changes**: Documented in CHANGELOG.md
- **Compatibility**: Maintain backward compatibility when possible
- **Deprecation**: Clear deprecation notices for breaking changes

## Dependencies
- .NET 8.0
- ASP.NET Core
- MediatR
- Serilog
- AutoMapper
- FluentValidation
- Polly
- OpenTelemetry

## Contributing
1. Follow the established patterns and conventions
2. Add comprehensive tests for new functionality
3. Update documentation for any changes
4. Ensure backward compatibility
5. Update version numbers appropriately
