# 🚀 TOSS ERP III - Complete Deployment Guide

**Comprehensive deployment instructions for all application services**

---

## 📋 PRE-DEPLOYMENT CHECKLIST

### System Requirements

**Backend:**
- .NET 9 SDK or later
- PostgreSQL 16
- Redis 7
- 4GB RAM minimum
- 20GB disk space

**Web:**
- Node.js 20+ and npm
- 2GB RAM minimum

**Mobile:**
- Flutter 3.x SDK
- Android Studio (for Android)
- Xcode (for iOS, macOS only)

**Optional:**
- Docker Desktop (for containerized deployment)
- Kubernetes cluster (for production scaling)

---

## 🎯 DEPLOYMENT OPTIONS

### Option 1: Docker Compose (Recommended for Quick Start)

#### Step 1: Start All Services
```bash
cd backend
docker-compose up -d --build
```

This starts:
- PostgreSQL (port 5432)
- Redis (port 6379)
- Backend API (port 5000)
- Web Admin (port 3000)

#### Step 2: Run Database Migrations
```bash
dotnet ef database update --project src/TossErp.Infrastructure/TossErp.Infrastructure.csproj --startup-project src/TossErp.API/TossErp.API.csproj
```

#### Step 3: Access Applications
- **Backend API:** http://localhost:5000
- **API Documentation (Swagger):** http://localhost:5000
- **Web Admin:** http://localhost:3000
- **Health Check:** http://localhost:5000/health

---

### Option 2: Manual Deployment (Development)

#### Backend API

**Step 1: Install PostgreSQL**
```bash
# Windows: Download from postgresql.org
# Or use Docker:
docker run -d --name tosserp-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tosserp -p 5432:5432 postgres:16
```

**Step 2: Install Redis**
```bash
# Windows: Download from redis.io
# Or use Docker:
docker run -d --name tosserp-redis -p 6379:6379 redis:7-alpine
```

**Step 3: Configure Connection Strings**
Edit `backend/src/TossErp.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tosserp;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  }
}
```

**Step 4: Run Migrations**
```bash
cd backend
dotnet ef database update --project src/TossErp.Infrastructure/TossErp.Infrastructure.csproj --startup-project src/TossErp.API/TossErp.API.csproj
```

**Step 5: Start Backend API**
```bash
dotnet run --project src/TossErp.API/TossErp.API.csproj --urls "http://localhost:5000"
```

**Backend is now running at:** http://localhost:5000

---

#### Web Admin

**Step 1: Install Dependencies**
```bash
cd toss-web
npm install
```

**Step 2: Configure API URL**
Create `.env` file:
```bash
API_BASE_URL=http://localhost:5000
NUXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

**Step 3: Start Development Server**
```bash
npm run dev
```

**Web Admin is now running at:** http://localhost:3000

---

#### Mobile App

**Step 1: Install Dependencies**
```bash
cd toss-mobile
flutter pub get
```

**Step 2: Configure API Endpoint**
Edit `lib/core/constants/api_constants.dart`:
```dart
class ApiConstants {
  static const String baseUrl = 'http://localhost:5000/api';
  // For Android Emulator: 'http://10.0.2.2:5000/api'
  // For iOS Simulator: 'http://localhost:5000/api'
  // For Physical Device: 'http://YOUR_IP_ADDRESS:5000/api'
}
```

**Step 3: Run on Device/Emulator**
```bash
# Android
flutter run

# iOS (macOS only)
flutter run

# Web
flutter run -d chrome

# Build for Production
flutter build apk --release  # Android
flutter build ios --release  # iOS
flutter build web --release  # Web
```

---

### Option 3: Production Deployment (Kubernetes)

#### Prerequisites
- Kubernetes cluster (Azure AKS, AWS EKS, or Google GKE)
- kubectl configured
- Helm 3+ installed

#### Step 1: Create Kubernetes Namespace
```bash
kubectl create namespace tosserp
```

#### Step 2: Create Secrets
```bash
kubectl create secret generic tosserp-secrets \
  --from-literal=db-password=<STRONG_PASSWORD> \
  --from-literal=jwt-secret=<STRONG_JWT_SECRET> \
  --from-literal=openai-api-key=<YOUR_OPENAI_KEY> \
  -n tosserp
```

#### Step 3: Deploy PostgreSQL
```bash
kubectl apply -f k8s/postgresql.yaml -n tosserp
```

#### Step 4: Deploy Redis
```bash
kubectl apply -f k8s/redis.yaml -n tosserp
```

#### Step 5: Deploy Backend API
```bash
kubectl apply -f k8s/api-deployment.yaml -n tosserp
kubectl apply -f k8s/api-service.yaml -n tosserp
```

#### Step 6: Deploy Web Admin
```bash
kubectl apply -f k8s/web-deployment.yaml -n tosserp
kubectl apply -f k8s/web-service.yaml -n tosserp
```

#### Step 7: Run Migrations
```bash
# Get API pod name
kubectl get pods -n tosserp

# Run migrations
kubectl exec -it <api-pod-name> -n tosserp -- dotnet ef database update
```

---

## 🔧 CONFIGURATION

### Environment Variables

#### Backend (.env or appsettings.json)
```bash
# Database
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=tosserp;Username=postgres;Password=<PASSWORD>
ConnectionStrings__Redis=localhost:6379

# JWT
Jwt__Secret=<GENERATE_STRONG_SECRET_MIN_32_CHARS>
Jwt__Issuer=https://yourdomain.com
Jwt__Audience=https://yourdomain.com
Jwt__ExpiryMinutes=60

# AI Copilot (Optional)
AI__ApiKey=<YOUR_OPENAI_API_KEY>
AI__Endpoint=https://api.openai.com/v1/chat/completions

# Email (Optional)
Email__SmtpHost=smtp.gmail.com
Email__SmtpPort=587
Email__SmtpUser=<YOUR_EMAIL>
Email__SmtpPassword=<YOUR_APP_PASSWORD>

# CORS
Cors__Origins=http://localhost:3000,https://yourdomain.com
```

#### Web Admin (.env)
```bash
API_BASE_URL=http://localhost:5000
NUXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

#### Mobile (api_constants.dart)
```dart
static const String baseUrl = 'http://YOUR_SERVER_IP:5000/api';
```

---

## 🗄️ DATABASE SETUP

### Initialize Database

**Option 1: Automatic (via EF Migrations)**
```bash
cd backend
dotnet ef database update
```

**Option 2: Manual SQL**
```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE tosserp;

# Run migrations from generated SQL
\i migrations.sql
```

### Seed Initial Data (Optional)

Create default admin user:
```sql
-- Connect to tosserp database
\c tosserp

-- Create admin role
INSERT INTO "Roles" ("Name", "Description", "IsSystem", "CreatedAt")
VALUES ('Admin', 'System Administrator', true, NOW());

-- Create admin user (password: Admin@123456)
INSERT INTO "Users" ("Username", "Email", "PasswordHash", "FirstName", "LastName", "IsActive", "CreatedAt")
VALUES ('admin', 'admin@tosserp.com', '$2a$12$KIXxLv7qH5V9Z3Y8rN4Sd.WFpQMZqwWKQF9iP8xN9hJ0kTLMm3Y2u', 'System', 'Administrator', true, NOW());

-- Link user to admin role
INSERT INTO "UserRoles" ("UserId", "RoleId", "AssignedAt")
VALUES (1, 1, NOW());
```

---

## 🧪 VERIFY DEPLOYMENT

### Backend API Health Check
```bash
curl http://localhost:5000/health
# Expected: {"status": "Healthy"}

curl http://localhost:5000
# Should return Swagger UI HTML
```

### Web Admin Access
```bash
# Open in browser
http://localhost:3000

# Should show login page
```

### Test API Endpoints
```bash
# Register new user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test@123456",
    "firstName": "Test",
    "lastName": "User"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123456"
  }'
```

---

## 📱 MOBILE APP DEPLOYMENT

### Android Deployment

**Step 1: Build APK**
```bash
cd toss-mobile
flutter build apk --release
```

**Step 2: Locate APK**
```
build/app/outputs/flutter-apk/app-release.apk
```

**Step 3: Distribute**
- Upload to Google Play Store
- Or distribute directly to users
- Or use Firebase App Distribution

**Step 4: Configure Firebase (Optional)**
```bash
# For push notifications and analytics
flutter pub add firebase_core firebase_messaging
flutterfire configure
```

### iOS Deployment (macOS only)

**Step 1: Build IPA**
```bash
flutter build ios --release
```

**Step 2: Archive in Xcode**
- Open `ios/Runner.xcworkspace` in Xcode
- Product → Archive
- Distribute to App Store or AdHoc

**Step 3: Upload to App Store**
- Use Xcode Organizer
- Or use Transporter app

### Web Deployment

**Step 1: Build Web Version**
```bash
flutter build web --release
```

**Step 2: Deploy to Hosting**
```bash
# Build output is in: build/web/

# Deploy to Firebase Hosting
firebase deploy

# Or deploy to any static hosting (Netlify, Vercel, etc.)
```

---

## 🔍 MONITORING & MAINTENANCE

### Logs

**Backend API Logs:**
```bash
# Docker
docker logs tosserp-api -f

# Manual
# Logs are in console output
```

**Web Admin Logs:**
```bash
# Docker
docker logs tosserp-web -f

# Manual
# Check browser console for errors
```

### Health Monitoring

**Set up health check endpoints:**
- Backend: http://localhost:5000/health
- PostgreSQL: Check via backend health endpoint
- Redis: Check via backend health endpoint

**Recommended Monitoring Tools:**
- Application Insights (Azure)
- Grafana + Prometheus
- ELK Stack (Elasticsearch, Logstash, Kibana)
- Sentry (for error tracking)

### Database Backups

**Automated PostgreSQL Backups:**
```bash
# Create backup script
#!/bin/bash
BACKUP_DIR="/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
docker exec tosserp-postgres pg_dump -U postgres tosserp > $BACKUP_DIR/tosserp_$TIMESTAMP.sql

# Schedule with cron (daily at 2 AM)
0 2 * * * /path/to/backup-script.sh
```

**Manual Backup:**
```bash
pg_dump -U postgres -h localhost tosserp > backup_$(date +%Y%m%d).sql
```

**Restore from Backup:**
```bash
psql -U postgres -h localhost tosserp < backup_20251007.sql
```

---

## 🔐 SECURITY HARDENING (Production)

### 1. Update Secrets
```bash
# Generate strong JWT secret (32+ characters)
openssl rand -base64 32

# Update in appsettings.json or environment variables
```

### 2. Configure HTTPS
```bash
# Install SSL certificate
# Update ASPNETCORE_URLS to https://+:443

# For Let's Encrypt:
certbot certonly --standalone -d yourdomain.com
```

### 3. Firewall Rules
```bash
# Allow only necessary ports
ufw allow 80/tcp
ufw allow 443/tcp
ufw allow 22/tcp
ufw enable
```

### 4. Database Security
```bash
# Change PostgreSQL password
ALTER USER postgres WITH PASSWORD '<STRONG_PASSWORD>';

# Restrict network access in pg_hba.conf
# host    all    all    10.0.0.0/8    md5
```

### 5. Enable Rate Limiting
Already implemented in code (100 requests/minute per client)

---

## 📊 DEPLOYMENT VERIFICATION

### Checklist

- [ ] Backend API responds at http://localhost:5000
- [ ] Swagger UI accessible at http://localhost:5000
- [ ] Health endpoint returns "Healthy"
- [ ] PostgreSQL connection successful
- [ ] Redis connection successful
- [ ] Web admin loads at http://localhost:3000
- [ ] Login page displays correctly
- [ ] Can register new user
- [ ] Can login successfully
- [ ] Dashboards load data
- [ ] API endpoints respond correctly
- [ ] Mobile app connects to API
- [ ] Offline mode works (mobile)
- [ ] Service Worker registered (web)

---

## 🆘 TROUBLESHOOTING

### Backend Won't Start

**Issue:** "Could not load file or assembly"
```bash
# Solution: Clean and rebuild
dotnet clean
dotnet build
dotnet run
```

**Issue:** "Database connection failed"
```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Test connection
psql -U postgres -h localhost -d tosserp

# Verify connection string in appsettings.json
```

**Issue:** "Redis connection failed"
```bash
# Check Redis is running
docker ps | grep redis

# Test connection
redis-cli ping
# Should return: PONG
```

### Web Won't Start

**Issue:** "Module not found"
```bash
# Reinstall dependencies
rm -rf node_modules package-lock.json
npm install
```

**Issue:** "API connection refused"
```bash
# Verify backend is running
curl http://localhost:5000/health

# Check CORS settings in backend
```

### Mobile Connection Issues

**Issue:** "Network error" on Android Emulator
```dart
// Use 10.0.2.2 instead of localhost
static const String baseUrl = 'http://10.0.2.2:5000/api';
```

**Issue:** "Connection refused" on Physical Device
```bash
# Make sure device and computer are on same network
# Use computer's IP address instead of localhost
# Example: http://192.168.1.100:5000/api
```

---

## 🚀 PRODUCTION DEPLOYMENT STEPS

### 1. Prepare Infrastructure

**Cloud Provider Setup (Choose One):**

**Azure:**
```bash
# Create resource group
az group create --name tosserp-rg --location southafricawest

# Create PostgreSQL
az postgres flexible-server create \
  --name tosserp-db \
  --resource-group tosserp-rg \
  --location southafricawest \
  --admin-user tossadmin \
  --admin-password <STRONG_PASSWORD> \
  --sku-name Standard_B1ms

# Create Redis
az redis create \
  --name tosserp-redis \
  --resource-group tosserp-rg \
  --location southafricawest \
  --sku Basic \
  --vm-size c0

# Create App Service
az appservice plan create \
  --name tosserp-plan \
  --resource-group tosserp-rg \
  --sku B1 \
  --is-linux

az webapp create \
  --name tosserp-api \
  --resource-group tosserp-rg \
  --plan tosserp-plan \
  --runtime "DOTNET|9.0"
```

**AWS:**
```bash
# Create RDS PostgreSQL
aws rds create-db-instance \
  --db-instance-identifier tosserp-db \
  --db-instance-class db.t3.micro \
  --engine postgres \
  --master-username tossadmin \
  --master-user-password <STRONG_PASSWORD> \
  --allocated-storage 20

# Create ElastiCache Redis
aws elasticache create-cache-cluster \
  --cache-cluster-id tosserp-redis \
  --engine redis \
  --cache-node-type cache.t3.micro \
  --num-cache-nodes 1
```

### 2. Build for Production

**Backend:**
```bash
cd backend
dotnet publish src/TossErp.API/TossErp.API.csproj -c Release -o ./publish
```

**Web:**
```bash
cd toss-web
npm run build
```

**Mobile:**
```bash
cd toss-mobile
flutter build apk --release
flutter build ios --release  # macOS only
flutter build web --release
```

### 3. Deploy

**Backend to Azure App Service:**
```bash
az webapp deployment source config-zip \
  --resource-group tosserp-rg \
  --name tosserp-api \
  --src publish.zip
```

**Web to Static Hosting:**
```bash
# Netlify
netlify deploy --prod --dir=.output/public

# Vercel
vercel --prod

# Azure Static Web Apps
az staticwebapp create \
  --name tosserp-web \
  --resource-group tosserp-rg \
  --source .output/public
```

### 4. Run Production Migrations
```bash
# SSH into production server or use Azure Cloud Shell
dotnet ef database update --connection "<PRODUCTION_CONNECTION_STRING>"
```

### 5. Configure DNS
```bash
# Point your domain to deployment
# A Record: yourdomain.com → <SERVER_IP>
# CNAME: api.yourdomain.com → tosserp-api.azurewebsites.net
# CNAME: app.yourdomain.com → tosserp-web.netlify.app
```

---

## 📈 POST-DEPLOYMENT

### 1. Create Admin User
Login to database and run seed script (see Database Setup section)

### 2. Initial Configuration
- Create warehouses
- Add initial products
- Set up user roles
- Configure tax rates
- Set business information

### 3. User Onboarding
- Train first users
- Create user accounts
- Assign roles and permissions
- Provide documentation
- Schedule support sessions

### 4. Monitoring Setup
```bash
# Install monitoring agent
# Configure alerts
# Set up dashboards
# Enable error tracking
```

---

## 🎯 QUICK START SUMMARY

### Fastest Way to Get Running (Development)

```bash
# 1. Start databases (Docker)
docker run -d --name postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tosserp -p 5432:5432 postgres:16
docker run -d --name redis -p 6379:6379 redis:7-alpine

# 2. Start backend
cd backend
dotnet ef database update
dotnet run --project src/TossErp.API/TossErp.API.csproj --urls "http://localhost:5000"

# 3. Start web (new terminal)
cd toss-web
npm install
npm run dev

# 4. Access applications
# API: http://localhost:5000
# Web: http://localhost:3000
```

### Production Deployment with Docker Compose

```bash
cd backend
docker-compose up -d --build
dotnet ef database update

# All services now running:
# - PostgreSQL: localhost:5432
# - Redis: localhost:6379
# - Backend API: http://localhost:5000
# - Web Admin: http://localhost:3000
```

---

## 📞 SUPPORT

### Common Issues

**Port Already in Use:**
```bash
# Find process using port 5000
netstat -ano | findstr :5000
# Kill process
taskkill /PID <PID> /F
```

**Database Connection Failed:**
```bash
# Verify PostgreSQL is running
docker ps | grep postgres
# Or: pg_isctl status

# Test connection
psql -U postgres -h localhost -d tosserp
```

**Redis Connection Failed:**
```bash
# Verify Redis is running
docker ps | grep redis
# Or: redis-cli ping
```

---

## ✅ DEPLOYMENT COMPLETE

Once all services are running:

1. **Access Swagger API Docs:** http://localhost:5000
2. **Access Web Admin:** http://localhost:3000
3. **Login Credentials:** Use registered account or admin user
4. **Mobile App:** Install APK on Android or IPA on iOS
5. **Test Workflows:** Create sale, check inventory, view reports

---

**STATUS:** 🚀 **READY FOR PRODUCTION USE**

**Next Steps:**
1. Configure production environment variables
2. Set up SSL certificates
3. Deploy to cloud platform
4. Onboard first users
5. Monitor and iterate

---

**For Questions:** Refer to PROJECT_COMPLETE_FINAL_REPORT.md or COMPREHENSIVE_IMPLEMENTATION_SUMMARY.md

**Prepared By:** AI Development Team  
**Date:** October 7, 2025  
**Version:** 1.0.0

