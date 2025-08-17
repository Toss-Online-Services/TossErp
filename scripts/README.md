# TOSS ERP - Scripts Directory

This directory contains scripts for launching and managing the TOSS ERP system using Docker Compose and .NET Aspire.

## 🚀 **Launch Scripts**

### **Full Stack Launch**
- **`launch-full-stack.ps1`** - PowerShell script to launch complete TOSS ERP system
- **`launch-full-stack.bat`** - Windows batch file alternative
- **`launch-docker.ps1`** - PowerShell script for Docker Compose management
- **`launch-docker.bat`** - Windows batch file alternative

### **Legacy Scripts**
- **`launch-all-apps.ps1`** - Original PowerShell script (legacy)
- **`launch-all-apps.bat`** - Original batch file (legacy)

## 🎯 **What Each Script Does**

### **Full Stack Scripts**
Launch the complete TOSS ERP system including:
- **Backend Services**: API Gateway, Stock API, PostgreSQL, Redis, RabbitMQ
- **Client Applications**: Mobile (Flutter), Web (Nuxt.js with admin functionality)
- **Infrastructure**: Nginx reverse proxy, unified access portal

### **Docker Management Scripts**
Provide Docker Compose management with options for:
- Full stack deployment
- Backend-only deployment
- Infrastructure-only deployment
- Service monitoring and logs

## 🌐 **Access Points After Launch**

| Service | URL | Description |
|---------|-----|-------------|
| **Main Portal** | http://localhost/ | Unified access through Nginx |
| **Mobile Client** | http://localhost:3000/ | Flutter web app |
| **Web Client** | http://localhost:3001/ | Nuxt.js app (includes admin) |
| **API Gateway** | http://localhost:8080/ | Backend API endpoints |
| **RabbitMQ** | http://localhost:15672/ | Message broker management |

## 🔧 **Usage Examples**

### **PowerShell**
```powershell
# Launch full stack
.\launch-full-stack.ps1

# Launch with options
.\launch-full-stack.ps1 -OpenBrowser -ShowLogs
```

### **Batch (Windows)**
```batch
# Launch full stack
scripts\launch-full-stack.bat

# Launch with options
scripts\launch-full-stack.bat --open-browser
```

### **Manual Docker Compose**
```bash
# Full stack
docker-compose -f docker-compose.full-stack.yml up -d --build

# Backend only
docker-compose -f docker-compose.dev.yml up -d --build

# Infrastructure only
docker-compose -f docker-compose.infrastructure.yml up -d
```

## 📱 **Client Applications**

### **Mobile Client (Flutter)**
- **Port**: 3000
- **Purpose**: Field operations, inventory management
- **Features**: Offline support, barcode scanning, real-time sync

### **Web Client (Nuxt.js)**
- **Port**: 3001
- **Purpose**: Web-based management and reporting
- **Features**: **Comprehensive admin functionality**, responsive design, analytics

> **Note**: The web client includes all admin functionality, eliminating the need for a separate admin client.

## 🛠️ **Prerequisites**

1. **Docker Desktop** installed and running
2. **Docker Compose** v2.38.2+ (included with Docker Desktop)
3. **PowerShell 5.1+** or **Command Prompt** (Windows)

## 🔍 **Troubleshooting**

### **Common Issues**
- **Port conflicts** - Ensure required ports are available
- **Docker not running** - Start Docker Desktop
- **Build failures** - Check Docker logs and source files

### **Debug Commands**
```bash
# Check service status
docker-compose -f docker-compose.full-stack.yml ps

# View logs
docker-compose -f docker-compose.full-stack.yml logs -f

# Check specific service
docker-compose -f docker-compose.full-stack.yml logs <service-name>
```

## 📚 **Related Files**

- **`docker-compose.full-stack.yml`** - Complete system configuration
- **`docker-compose.dev.yml`** - Backend services only
- **`docker-compose.infrastructure.yml`** - Infrastructure services only
- **`README-DOCKER-ASPIRE.md`** - Comprehensive setup documentation

---

**🎉 Use these scripts to quickly launch your TOSS ERP development environment!**
