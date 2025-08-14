# TossErp - Enterprise Resource Planning System

## Overview
TossErp is a modern, microservices-based Enterprise Resource Planning (ERP) system built with .NET 8, Vue.js, and Kubernetes. The system provides comprehensive business management capabilities including inventory management, order processing, user management, and AI-powered automation.

## Architecture

### Microservices Architecture
- **API Gateway**: Centralized routing and authentication
- **Stock Service**: Inventory and catalog management
- **User Service**: User management and authentication
- **Order Service**: Order processing and fulfillment
- **Payment Service**: Payment processing and financial management
- **AI Service**: LangChain-powered automation and insights

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core
- **Frontend**: Vue.js 3, Nuxt.js 3, TypeScript, Tailwind CSS
- **Database**: SQL Server, Redis
- **Message Broker**: RabbitMQ
- **Containerization**: Docker, Kubernetes
- **Monitoring**: Prometheus, Grafana, ELK Stack
- **AI Integration**: LangChain.NET

## Project Structure

```
TossErp/
├── clients/                    # Frontend applications
│   ├── web-app/               # Vue.js/Nuxt.js web application
│   ├── mobile-app/            # Mobile application
│   └── admin-panel/           # Admin dashboard
├── services/                   # Microservices
│   ├── stock-service/         # Inventory management service
│   ├── user-service/          # User management service
│   ├── order-service/         # Order processing service
│   ├── payment-service/       # Payment processing service
│   └── app-host/              # Application host (Aspire)
├── gateways/                   # API Gateways
│   ├── web-gateway/           # Web client gateway
│   └── mobile-gateway/        # Mobile client gateway
├── shared/                     # Shared libraries and utilities
│   ├── service-defaults/      # Common service configurations
│   ├── event-bus/             # Event-driven communication
│   ├── common-libraries/      # Shared utilities
│   ├── proto-definitions/     # gRPC protocol definitions
│   └── config-templates/      # Configuration templates
├── deploy/                     # Deployment configurations
│   ├── kubernetes/            # Kubernetes manifests
│   ├── docker/                # Docker configurations
│   └── terraform/             # Infrastructure as Code
├── docs/                       # Documentation
│   ├── architecture/          # Architecture diagrams
│   ├── api/                   # API documentation
│   └── guides/                # Development guides
├── tests/                      # Integration and E2E tests
└── scripts/                    # Build and deployment scripts
```

## Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- Kubernetes cluster (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/tosserp.git
   cd tosserp
   ```

2. **Start the application host**
   ```bash
   cd services/app-host
   dotnet run
   ```

3. **Start the web application**
   ```bash
   cd clients/web-app
   npm install
   npm run dev
   ```

4. **Access the application**
   - Web App: http://localhost:3000
   - API Gateway: http://localhost:8080
   - Stock Service: http://localhost:5001

### Docker Development

1. **Start all services with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Access services**
   - Web App: http://localhost:3000
   - API Gateway: http://localhost:8080
   - Stock Service: http://localhost:5001

## Features

### Core Features
- **Inventory Management**: Complete stock tracking and management
- **Order Processing**: End-to-end order lifecycle management
- **User Management**: Role-based access control and authentication
- **Payment Processing**: Secure payment handling
- **Reporting**: Comprehensive business analytics and reporting

### AI-Powered Features
- **Natural Language Queries**: Ask questions about your data in plain English
- **Automated Operations**: AI-driven stock reordering and optimization
- **Predictive Analytics**: Demand forecasting and trend analysis
- **Smart Recommendations**: Product recommendations and insights

### Technical Features
- **Microservices Architecture**: Scalable and maintainable design
- **Event-Driven Communication**: Loose coupling between services
- **API-First Design**: Comprehensive REST APIs with OpenAPI documentation
- **Real-time Updates**: WebSocket integration for live data
- **Comprehensive Testing**: Unit, integration, and E2E tests
- **Monitoring & Observability**: Full-stack monitoring and logging

## Development

### Adding a New Service

1. **Create service structure**
   ```bash
   mkdir -p services/new-service/src
   cd services/new-service
   ```

2. **Add to application host**
   ```bash
   # In services/app-host/Program.cs
   var newService = builder.AddProject("new-service", "../new-service/src/NewService.API");
   ```

3. **Configure gateway routing**
   ```bash
   # In gateways/web-gateway/appsettings.json
   {
     "ReverseProxy": {
       "Routes": {
         "new-service": {
           "ClusterId": "new-service-cluster",
           "Match": { "Path": "/api/new-service/{**catch-all}" }
         }
       }
     }
   }
   ```

### Testing

```bash
# Run all tests
dotnet test

# Run specific service tests
dotnet test services/stock-service/tests/

# Run E2E tests
npm run test:e2e
```

### Code Quality

```bash
# Format code
dotnet format

# Run linting
npm run lint

# Run type checking
npm run type-check
```

## Deployment

### Kubernetes Deployment

1. **Deploy to development**
   ```bash
   kubectl apply -k deploy/kubernetes/overlays/development
   ```

2. **Deploy to production**
```bash
   kubectl apply -k deploy/kubernetes/overlays/production
```

### Docker Deployment

```bash
# Build and push images
docker-compose -f docker-compose.prod.yml build
docker-compose -f docker-compose.prod.yml push

# Deploy to production
docker-compose -f docker-compose.prod.yml up -d
```

## Monitoring

### Health Checks
- **Service Health**: `/health` endpoint on each service
- **Database Health**: Connection and query performance monitoring
- **External Dependencies**: Third-party service health monitoring

### Metrics
- **Application Metrics**: Request rates, response times, error rates
- **Business Metrics**: Orders, revenue, inventory levels
- **Infrastructure Metrics**: CPU, memory, disk usage

### Logging
- **Structured Logging**: JSON-formatted logs with correlation IDs
- **Centralized Logging**: ELK stack for log aggregation and analysis
- **Log Levels**: Debug, Info, Warning, Error with appropriate filtering

## Contributing

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Make your changes**: Follow the coding standards and add tests
4. **Commit your changes**: `git commit -m 'Add amazing feature'`
5. **Push to the branch**: `git push origin feature/amazing-feature`
6. **Open a Pull Request**: Provide detailed description and screenshots

### Development Standards
- **Code Style**: Follow .NET and Vue.js coding conventions
- **Testing**: Maintain >80% code coverage
- **Documentation**: Update documentation for all changes
- **Security**: Follow security best practices
- **Performance**: Optimize for performance and scalability

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/your-org/tosserp/issues)
- **Discussions**: [GitHub Discussions](https://github.com/your-org/tosserp/discussions)
- **Email**: support@tosserp.com

## Roadmap

### Phase 1: Core Services ✅
- [x] Stock Service
- [x] User Service
- [x] API Gateway
- [x] Web Application

### Phase 2: Advanced Features 🚧
- [ ] Order Service
- [ ] Payment Service
- [ ] AI Integration
- [ ] Mobile Application

### Phase 3: Enterprise Features 📋
- [ ] Multi-tenancy
- [ ] Advanced Analytics
- [ ] Workflow Engine
- [ ] Integration Hub

### Phase 4: Scale & Optimize 📋
- [ ] Performance Optimization
- [ ] Advanced Monitoring
- [ ] Auto-scaling
- [ ] Disaster Recovery 