# Web Gateway

## Overview
The Web Gateway serves as the primary entry point for web client applications, providing routing, authentication, and cross-cutting concerns for the TossErp microservices architecture.

## Architecture
- **API Gateway Pattern**: Centralized routing and request handling
- **Backend for Frontend (BFF)**: Optimized for web client needs
- **Circuit Breaker**: Resilience patterns for downstream services
- **Rate Limiting**: Protection against abuse
- **Authentication**: Centralized auth handling

## Features
- **Service Discovery**: Dynamic routing to microservices
- **Load Balancing**: Distribute requests across service instances
- **Authentication & Authorization**: JWT token validation and role-based access
- **Request/Response Transformation**: Adapt data formats for clients
- **Caching**: Response caching for improved performance
- **Logging & Monitoring**: Centralized request logging
- **CORS**: Cross-origin resource sharing configuration
- **Health Checks**: Gateway health monitoring

## Configuration
```json
{
  "ReverseProxy": {
    "Routes": {
      "stock-service": {
        "ClusterId": "stock-cluster",
        "Match": {
          "Path": "/api/stock/{**catch-all}"
        }
      },
      "user-service": {
        "ClusterId": "user-cluster",
        "Match": {
          "Path": "/api/users/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "stock-cluster": {
        "Destinations": {
          "stock-service": {
            "Address": "http://stock-service:80"
          }
        }
      }
    }
  }
}
```

## Development
```bash
# Run locally
dotnet run

# Run with Docker
docker-compose up

# Health check
curl http://localhost:8080/health
```

## Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Staging/Production
- `ASPNETCORE_URLS`: Gateway listening URLs
- `JWT_SECRET`: JWT signing secret
- `SERVICE_DISCOVERY_URL`: Service discovery endpoint

## Dependencies
- ASP.NET Core 8.0
- YARP (Yet Another Reverse Proxy)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Polly (Resilience patterns)
- Serilog
