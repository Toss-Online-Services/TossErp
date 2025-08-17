# TOSS ERP - Docker Compose & .NET Aspire Setup

This document explains how to run TOSS ERP locally using **Docker Compose** and **.NET Aspire** - the modern, professional way to develop microservices applications.

## 🚀 **Why Docker Compose & Aspire?**

### **Docker Compose Benefits:**
- **Production-like environment** - Same containers, same networking, same dependencies
- **Consistent across team** - Everyone gets the exact same setup
- **Easy dependency management** - PostgreSQL, Redis, RabbitMQ all managed automatically
- **Portable** - Works on Windows, macOS, and Linux
- **Scalable** - Easy to add more services or scale existing ones

### **.NET Aspire Benefits:**
- **Native .NET orchestration** - Built specifically for .NET microservices
- **Intelligent service discovery** - Automatic configuration and connection management
- **Built-in monitoring** - Health checks, metrics, and observability
- **Development productivity** - Hot reload, debugging, and local development tools

## 🏗️ **Architecture Overview**

TOSS ERP follows a **microservices architecture** with:

- **API Gateway** - Single entry point for all API requests
- **Stock Service** - Core business logic for inventory management
- **Client Applications** - User interfaces for different user types
- **Infrastructure** - Database, cache, and message broker

### **Client Applications:**
- **Mobile Client** (Flutter) - Mobile-first interface for field operations
- **Web Client** (Nuxt.js) - **Comprehensive web interface including admin functionality**

> **Note:** The web client includes all admin functionality, eliminating the need for a separate admin client.

## 🐳 **Quick Start with Docker Compose**

### **Option 1: Full Stack (Recommended)**
Launch everything including client applications:

```bash
# PowerShell
.\scripts\launch-full-stack.ps1

# Batch
scripts\launch-full-stack.bat

# Manual
docker-compose -f docker-compose.full-stack.yml up -d --build
```

### **Option 2: Backend Only**
Launch just the core services:

```bash
docker-compose -f docker-compose.dev.yml up -d --build
```

### **Option 3: Infrastructure Only**
Launch just the database and message broker:

```bash
docker-compose -f docker-compose.infrastructure.yml up -d
```

## 🌐 **Access Points**

Once launched, access your applications at:

| Service | URL | Description |
|---------|-----|-------------|
| **Main Portal** | http://localhost/ | Unified access through Nginx |
| **Mobile Client** | http://localhost:3000/ | Flutter web app |
| **Web Client** | http://localhost:3001/ | Nuxt.js app (includes admin) |
| **API Gateway** | http://localhost:8080/ | Backend API endpoints |
| **RabbitMQ** | http://localhost:15672/ | Message broker management |
| **PostgreSQL** | localhost:5432 | Database |
| **Redis** | localhost:6379 | Cache |

## 🔧 **Management Commands**

```bash
# View service status
docker-compose -f docker-compose.full-stack.yml ps

# View logs
docker-compose -f docker-compose.full-stack.yml logs -f

# Stop services
docker-compose -f docker-compose.full-stack.yml down

# Restart services
docker-compose -f docker-compose.full-stack.yml restart
```

## 🚀 **.NET Aspire Alternative**

For native .NET development experience:

```bash
# Navigate to Aspire host
cd TossErp.AppHost

# Launch with Aspire
dotnet run
```

This provides:
- **Hot reload** for all services
- **Integrated debugging**
- **Service discovery** and configuration
- **Built-in monitoring** and health checks

## 📁 **Project Structure**

```
TossErp/
├── src/
│   ├── Gateway/                 # API Gateway service
│   ├── Services/
│   │   └── Stock/              # Stock management service
│   └── clients/
│       ├── mobile/             # Flutter mobile client
│       └── web/                # Nuxt.js web client (includes admin)
├── docker-compose.*.yml        # Docker Compose configurations
├── scripts/                    # Launch scripts
└── TossErp.AppHost/           # .NET Aspire orchestration
```

## 🔍 **Troubleshooting**

### **Common Issues:**

1. **Port conflicts** - Ensure ports 80, 3000, 3001, 8080, 15672, 5432, 6379 are available
2. **Docker not running** - Start Docker Desktop
3. **Build failures** - Check Docker logs and ensure all source files are present

### **Debug Commands:**

```bash
# Check Docker status
docker info

# View container logs
docker logs <container-name>

# Check service health
docker-compose -f docker-compose.full-stack.yml ps
```

## 🎯 **Development Workflow**

1. **Start infrastructure** - Database, cache, message broker
2. **Launch backend** - API Gateway and business services
3. **Start clients** - Web and mobile applications
4. **Access via Nginx** - Unified portal at http://localhost/

## 📚 **Additional Resources**

- **Docker Compose**: https://docs.docker.com/compose/
- **.NET Aspire**: https://learn.microsoft.com/en-us/dotnet/aspire/
- **Nuxt.js**: https://nuxt.com/
- **Flutter**: https://flutter.dev/

---

**🎉 You're now ready to develop with a professional, production-like environment!**
