# TOSS ERP III - Quick Start Guide

## 🚀 Get Up and Running in 10 Minutes

This guide will help you get the TOSS ERP system running on your local machine quickly.

## Prerequisites

Before you begin, ensure you have:

- [ ] Docker Desktop installed and running
- [ ] Node.js 18+ installed
- [ ] .NET 9 SDK installed (or .NET 10 preview)
- [ ] Flutter SDK 3.x installed (for mobile app)
- [ ] Git installed

## Step 1: Clone and Navigate

```bash
# If you haven't cloned yet
git clone <repository-url>
cd TossErp
```

## Step 2: Start Backend Services (1 minute)

```bash
cd backend

# Start PostgreSQL + Redis + API in Docker
docker-compose up -d

# Wait 30 seconds for services to start, then check status
docker-compose ps
```

**Expected Output:**
- `tosserp-postgres` - running on port 5432
- `tosserp-redis` - running on port 6379  
- `tosserp-api` - running on port 5000

**Verify API is Running:**
Open browser to: http://localhost:5000
You should see the Swagger UI documentation.

## Step 3: Start Web Admin (2 minutes)

```bash
cd ../toss-web

# Install dependencies (first time only)
npm install

# Start development server
npm run dev
```

**Expected Output:**
```
✔ Vite server built in 543ms
✔ Nuxt Nitro server built in 236 ms

  ➜ Local:   http://localhost:3000/
```

**Verify Web Admin:**
Open browser to: http://localhost:3000
You should see the login page.

## Step 4: Start Mobile App (Optional - 3 minutes)

```bash
cd ../toss-mobile

# Install dependencies (first time only)
flutter pub get

# Run on connected device or emulator
flutter run
```

## Step 5: Explore the System

### Backend API (http://localhost:5000)

**Test the API:**
1. Open Swagger UI
2. Explore available endpoints
3. Try the health check: `GET /health`

**Sample API Calls (using Swagger):**

1. **Create a Product:**
   - Endpoint: `POST /api/products`
   - Try the "Try it out" button
   - Fill in the sample data
   - Execute

2. **Get Sales Summary:**
   - Endpoint: `GET /api/sales/summary`
   - No auth required for testing in development
   - See today's sales

### Web Admin (http://localhost:3000)

**Default Login (once auth is seeded):**
- Email: admin@tosserp.com
- Password: admin123

**Explore:**
1. Dashboard - View KPIs and charts
2. Sales → POS Dashboard - See POS metrics
3. Inventory → Dashboard - Check stock levels
4. Finance → Dashboard - View financials
5. HR → Dashboard - Employee overview

### Mobile App

**Explore:**
1. POS Screen - Add products to cart
2. Process transactions
3. View transaction history
4. Manage customers

## 🔧 Troubleshooting

### Backend Won't Start

**Problem:** Docker compose fails
```bash
# Check Docker is running
docker ps

# View logs
docker-compose logs tosserp-api

# Rebuild if needed
docker-compose down
docker-compose up --build
```

**Problem:** Migration errors
```bash
# Manually apply migrations
cd backend/src/TossErp.API
dotnet ef database update --project ../TossErp.Infrastructure
```

### Web Admin Issues

**Problem:** Connection refused to API
- Check API is running: http://localhost:5000
- Check `.env` or `nuxt.config.ts` has correct `API_BASE_URL`
- Check browser console for errors

**Problem:** Build errors
```bash
# Clear cache and reinstall
rm -rf node_modules .nuxt
npm install
```

### Mobile App Issues

**Problem:** Build errors
```bash
# Clean and rebuild
flutter clean
flutter pub get
flutter run
```

**Problem:** Firebase not configured
- Run: `flutter pub run flutter_config`
- Or check `firebase.json` exists

## 📝 Seed Data (Development)

To populate the system with sample data:

### Option 1: Through API

Use Swagger UI to create:
1. Warehouses (`POST /api/inventory/warehouses`)
2. Products (`POST /api/products`)
3. Customers (`POST /api/customers`)
4. Employees (`POST /api/hr/employees`)

### Option 2: SQL Script (Coming Soon)

```bash
# Run seed script
psql -U postgres -d tosserp -f backend/scripts/seed.sql
```

## 🎯 What to Try First

### 1. Create Your First Sale (Web Admin)

1. Go to http://localhost:3000/sales/pos
2. Add products to cart
3. Select customer
4. Process payment
5. Generate receipt

### 2. Monitor Stock Levels

1. Go to http://localhost:3000/inventory/dashboard
2. View current stock
3. Check low stock alerts
4. Create stock adjustment

### 3. View Financial Reports

1. Go to http://localhost:3000/finance/dashboard
2. View balance sheet
3. Check P&L statement
4. Review account balances

### 4. Manage Employees

1. Go to http://localhost:3000/hr/dashboard
2. Add new employee
3. Record attendance
4. Create leave request

## 🔐 Security Note

**Development Mode:**
- Authentication is configured but may allow test access
- CORS is set to "AllowAll" in development
- HTTPS redirection is disabled locally

**Before Production:**
- Change JWT secret key
- Update database passwords
- Configure production CORS origins
- Enable HTTPS
- Set up proper monitoring
- Review all security settings

## 📊 Monitoring

### Health Checks
Visit: http://localhost:5000/health

Should return:
```json
{
  "status": "Healthy",
  "checks": {
    "PostgreSQL": "Healthy",
    "Redis": "Healthy"
  }
}
```

### Logs

**Backend:**
```bash
# View API logs
docker-compose logs -f tosserp-api

# View PostgreSQL logs
docker-compose logs -f postgres
```

**Web:**
- Check browser console
- Check terminal where `npm run dev` is running

## 🎨 Customization

### Update Company Information

Edit `backend/src/TossErp.API/appsettings.json`:
```json
{
  "CompanySettings": {
    "Name": "Your Business Name",
    "TaxId": "Your Tax Number",
    "Address": "Your Address"
  }
}
```

### Change Theme (Web)

Edit `toss-web/tailwind.config.js`:
```javascript
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: '#your-color',
      }
    }
  }
}
```

## 📖 Next Steps

After getting familiar with the system:

1. **Review Documentation**
   - [Backend README](backend/README.md)
   - [Web README](toss-web/README.md)
   - [Implementation Summary](IMPLEMENTATION_SUMMARY.md)
   - [Project Roadmap](PROJECT_ROADMAP.md)

2. **Explore the Code**
   - Review domain entities to understand business logic
   - Check API controllers to see available endpoints
   - Examine web components to understand UI patterns
   - Look at mobile screens to see Flutter implementation

3. **Plan Your Customizations**
   - What additional modules do you need?
   - What reports are most important?
   - What integrations are required?
   - What workflows need automation?

4. **Start Development**
   - Follow the implementation guides
   - Create new features following existing patterns
   - Write tests for new code
   - Document your work

## 🆘 Getting Help

**Common Issues:**

1. **Port Already in Use**
   ```bash
   # Change ports in docker-compose.yml
   ports:
     - "5001:8080"  # Change 5000 to 5001
   ```

2. **Database Connection Fails**
   - Check PostgreSQL is running: `docker ps`
   - Verify connection string in `appsettings.json`
   - Check PostgreSQL logs: `docker-compose logs postgres`

3. **Migration Issues**
   ```bash
   # Drop and recreate database
   docker-compose down -v
   docker-compose up -d
   dotnet ef database update --project src/TossErp.Infrastructure --startup-project src/TossErp.API
   ```

**For More Help:**
- Check the troubleshooting section in individual READMEs
- Review GitHub issues
- Contact the development team

---

🎉 **Congratulations!** You now have a fully functional ERP III system running locally.

**Current Status:**  Foundation modules complete ✅  
**Next Phase:** Extended ERP modules (Manufacturing, Supply Chain, Projects, etc.)

Happy coding! 🚀

