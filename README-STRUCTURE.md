# TOSS ERP - Monorepo Structure

This document outlines the systematic and organized approach to the TOSS ERP monorepo structure, following best practices for .NET microservices and modern web applications.

## 🏗️ Repository Structure

```
TossErp/
├── 📁 .github/                    # GitHub workflows and templates
├── 📁 .vscode/                    # VS Code settings
├── 📁 docs/                       # Documentation
├── 📁 scripts/                    # Build and deployment scripts
├── 📁 tools/                      # Development tools
├── 📁 configs/                    # Configuration files
├── 📁 docker/                     # Docker orchestration files
├── 📁 infra/                      # Infrastructure as code
├── 📁 k8s/                        # Kubernetes manifests
├── 📁 monitoring/                 # Monitoring configuration
├── 📁 tests/                      # Test projects
├── 📁 src/                        # Source code
│   ├── 📁 AppHost/                # Single Aspire AppHost project
│   ├── 📁 Services/               # Backend microservices
│   │   ├── 📁 Identity/           # Identity & Authentication
│   │   ├── 📁 Stock/              # Stock Management
│   │   ├── 📁 CRM/                # Customer Relationship Management
│   │   ├── 📁 Collaboration/      # Group-Buy & Collaboration
│   │   └── 📁 Notifications/      # Notification Service
│   ├── 📁 Gateway/                # API Gateway
│   ├── 📁 Shared/                 # Shared libraries
│   │   ├── 📁 Common/             # Common utilities
│   │   ├── 📁 Contracts/          # Shared contracts
│   │   └── 📁 Infrastructure/     # Shared infrastructure
│   └── 📁 Clients/                # Frontend applications
│       ├── 📁 Web/                # Nuxt 4 web application
│       └── 📁 Mobile/             # Flutter mobile application
├── 📄 docker-compose.yml          # Main docker-compose entry point
├── 📄 Directory.Packages.props    # Centralized package versions
├── 📄 Directory.Build.props       # Centralized build properties
├── 📄 global.json                 # .NET version
├── 📄 TossErp.sln                # Main solution file
└── 📄 README.md                   # Main documentation
```

## 🔧 Key Principles

### 1. **Domain-Driven Organization**
- Services are organized by business domain (Identity, Stock, CRM, etc.)
- Each service follows Clean Architecture with Domain, Application, Infrastructure layers
- Clear boundaries between services with well-defined contracts

### 2. **Single Responsibility**
- One AppHost project to rule them all
- Each service has a single, well-defined purpose
- Shared libraries contain only truly common functionality

### 3. **Consistent Naming**
- Use PascalCase for project names
- Use kebab-case for directories
- Prefix shared libraries with appropriate identifiers

### 4. **Dependency Management**
- Centralized package versions in `Directory.Packages.props`
- Project references for inter-service dependencies
- No circular dependencies between services

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker & Docker Compose
- Node.js 18+ (for frontend development)
- PowerShell (for Windows scripts)

### Development Environment

1. **Clone and Setup**
   ```bash
   git clone <repository-url>
   cd TossErp
   ```

2. **Build Backend**
   ```powershell
   # Windows
   .\scripts\build.ps1 -Restore -Test
   
   # Or manually
   dotnet restore TossErp.sln
   dotnet build TossErp.sln
   dotnet test TossErp.sln
   ```

3. **Start Infrastructure**
   ```powershell
   # Windows
   .\scripts\docker.ps1 infra
   
   # Or manually
   docker-compose -f docker/docker-compose.yml up postgres redis rabbitmq
   ```

4. **Start Development Environment**
   ```powershell
   # Windows
   .\scripts\docker.ps1 dev
   
   # Or manually
   docker-compose -f docker/docker-compose.yml -f docker/docker-compose.dev.yml up
   ```

5. **Frontend Development**
   ```bash
   cd src/clients/web
   npm install
   npm run dev
   ```

## 🐳 Docker Management

### Available Commands

```powershell
# Start all services
.\scripts\docker.ps1 up

# Start development environment (with monitoring)
.\scripts\docker.ps1 dev

# Start only infrastructure
.\scripts\docker.ps1 infra

# Start only backend services
.\scripts\docker.ps1 services

# Start only frontend
.\scripts\docker.ps1 frontend

# View logs
.\scripts\docker.ps1 logs

# Stop all services
.\scripts\docker.ps1 down

# Clean Docker resources
.\scripts\docker.ps1 clean
```

### Docker Compose Files

- **`docker/docker-compose.yml`** - Main services configuration
- **`docker/docker-compose.dev.yml`** - Development overrides (monitoring, hot reload)
- **`docker-compose.yml`** - Root entry point with usage instructions

## 🏗️ Service Architecture

### Identity Service
- **Port**: 5001
- **Purpose**: User management, authentication, authorization
- **Features**: JWT tokens, RBAC, tenant isolation, POPIA compliance

### Stock Service
- **Port**: 5002
- **Purpose**: Inventory management, stock tracking
- **Features**: Stock levels, warehouses, stock movements

### CRM Service
- **Port**: 5003
- **Purpose**: Customer relationship management
- **Features**: Customer profiles, interactions, loyalty programs

### API Gateway
- **Port**: 8080
- **Purpose**: Centralized API routing and aggregation
- **Features**: Load balancing, authentication, rate limiting

### Web Client
- **Port**: 3000
- **Purpose**: Nuxt 4 web application
- **Features**: Modern UI, responsive design, real-time updates

## 📁 Directory Organization

### `src/Services/`
Each service follows this structure:
```
ServiceName/
├── ServiceName.Domain/           # Domain entities, business logic
├── ServiceName.Application/      # Use cases, DTOs, MediatR handlers
├── ServiceName.Infrastructure/  # Data access, external services
└── ServiceName.API/             # Web API controllers
```

### `src/Shared/`
Common functionality shared across services:
```
Shared/
├── Common/                       # Utilities, extensions, helpers
├── Contracts/                    # Shared interfaces, DTOs
└── Infrastructure/               # Common infrastructure components
```

### `src/Clients/`
Frontend applications:
```
Clients/
├── Web/                         # Nuxt 4 web application
│   ├── app/                     # App router (Nuxt 4)
│   ├── components/              # Vue components
│   ├── composables/             # Vue composables
│   ├── layouts/                 # Page layouts
│   ├── pages/                   # File-based routing
│   └── stores/                  # State management
└── Mobile/                      # Flutter mobile application
```

## 🔄 Development Workflow

### 1. **Feature Development**
- Create feature branch from main
- Implement changes in appropriate service
- Add/update tests
- Update documentation

### 2. **Testing**
- Unit tests for domain logic
- Integration tests for services
- E2E tests for critical user journeys

### 3. **Code Review**
- Pull request with clear description
- Automated tests must pass
- Code coverage requirements met

### 4. **Deployment**
- Automated CI/CD pipeline
- Staging environment validation
- Production deployment with rollback capability

## 📊 Monitoring & Observability

### Development Monitoring
- **Prometheus**: Metrics collection (port 9090)
- **Grafana**: Dashboards and visualization (port 3001)
- **Health Checks**: Built into all services

### Production Monitoring
- **Application Insights**: .NET application monitoring
- **Log Aggregation**: Centralized logging
- **Performance Metrics**: Response times, throughput, error rates

## 🛠️ Tools & Scripts

### Build Scripts
- **`scripts/build.ps1`**: Complete solution build with options
- **`scripts/docker.ps1`**: Docker environment management

### Configuration
- **`Directory.Packages.props`**: Centralized NuGet package versions
- **`Directory.Build.props`**: Common build properties
- **`global.json`**: .NET SDK version

## 🚨 Troubleshooting

### Common Issues

1. **Port Conflicts**
   - Check if ports are already in use
   - Use `netstat -an | findstr :<port>` (Windows) or `lsof -i :<port>` (Linux/Mac)

2. **Docker Issues**
   - Clean Docker resources: `.\scripts\docker.ps1 clean`
   - Rebuild containers: `.\scripts\docker.ps1 build`

3. **Build Failures**
   - Clean solution: `.\scripts\build.ps1 -Clean`
   - Restore packages: `.\scripts\build.ps1 -Restore`

4. **Service Dependencies**
   - Ensure infrastructure services are running first
   - Check service health endpoints

## 📚 Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Nuxt 4 Documentation](https://nuxt.com/docs)
- [Docker Documentation](https://docs.docker.com/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 🤝 Contributing

1. Follow the established structure and patterns
2. Add tests for new functionality
3. Update documentation as needed
4. Use conventional commit messages
5. Ensure all checks pass before merging

---

**Last Updated**: August 2025
**Version**: 1.0.0
