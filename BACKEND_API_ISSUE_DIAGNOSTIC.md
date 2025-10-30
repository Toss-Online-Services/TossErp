# Backend API Connection Issue - Diagnostic Report

**Date:** October 30, 2025  
**Issue:** POS page data not loading - Backend API not responding

---

## 🔍 Problem Summary

The frontend POS page (`http://localhost:3001/sales/pos`) is not loading data because the backend API (`https://localhost:5001`) is not responding to requests, despite the process running and listening on the correct port.

## 📊 Current System Status

### ✅ Working Components:
- **Frontend:** Running on `http://localhost:3001`
- **Database:** PostgreSQL containers running in Docker
  - Container: `postgres-hcynswxt` on port `61324`
  - Container: `postgres-zgadgzsk` on port `58361`
  - Container: `toss-postgres` on port `5432`
- **Docker:** Docker Desktop running (v28.5.1)
- **Backend Process:** `Toss.Web` (PID varies) is running

### ❌ Failing Components:
- **Backend API:** Not responding to HTTP/HTTPS requests
  - Process: `Toss.Web.exe` running
  - Port: Listening on `5001` (HTTPS)
  - Health endpoint: `https://localhost:5001/health` - **Connection closed**
  - API endpoint: `https://localhost:5001/api/Sales/daily-summary` - **Connection closed**

## 🔧 Diagnostic Details

### Port Status:
```
LocalPort: 5001
State: Listen
OwningProcess: Toss.Web (various PIDs)
```

### Error Messages:
- **PowerShell:** "The underlying connection was closed: An unexpected error occurred on a send."
- **HTTP:** Connection timeout or immediate close
- **HTTPS:** TLS handshake fails or connection drops

### API Configuration:
**Frontend (`nuxt.config.ts`):**
```typescript
apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5001'
```

**Backend (`launchSettings.json`):**
```json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```

## 🎯 Root Cause Analysis

The issue is likely one of these:

### 1. **SSL/TLS Certificate Issue** (Most Likely)
- The backend is configured for HTTPS but the certificate is not trusted
- PowerShell `Invoke-RestMethod` cannot establish secure connection
- Frontend browser can bypass certificate warnings, but API calls fail

### 2. **Backend Startup Failure**
- The process binds to port 5001 but then crashes/hangs during initialization
- Database connection might be failing silently
- Missing configuration or environment variables

### 3. **Database Migration Required**
- Entity Framework migrations not applied
- Backend waiting for database schema to exist
- Connection string misconfiguration

### 4. **.NET Aspire Orchestration Issue**
- AppHost started but didn't properly configure the Web service
- Service discovery not working
- Resource dependencies not resolved

## 💡 Recommended Solutions

### Solution 1: Trust the Development Certificate

```powershell
# Run in PowerShell as Administrator
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Then restart the backend:
```powershell
cd backend/Toss/src/Web
dotnet run
```

### Solution 2: Run Database Migrations

```powershell
cd backend/Toss/src/Web
dotnet ef database update
```

If Entity Framework tools are not installed:
```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update
```

### Solution 3: Check Backend Logs

Open the PowerShell window where the backend is running and look for:
- **Red error messages**
- **Stack traces**
- **Database connection errors**
- **Missing configuration warnings**

### Solution 4: Use HTTP Instead of HTTPS

**Temporary workaround** for testing:

1. Edit `backend/Toss/src/Web/Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5001"
```

2. Update `toss-web/.env`:
```
NUXT_PUBLIC_API_BASE=http://localhost:5001
```

3. Restart both frontend and backend

### Solution 5: Run via .NET Aspire AppHost

The proper way to run the backend:

```powershell
cd backend/Toss/src/AppHost
dotnet run
```

This will:
- Start PostgreSQL container automatically
- Configure service discovery
- Set up proper networking
- Open Aspire Dashboard at `http://localhost:5000`

## 📝 Changes Already Made

### 1. POS Page API Integration
**File:** `toss-web/pages/sales/pos/index.vue`

**Changed:**
- Removed hardcoded stats: `18496, 48, 285, 2500`
- Added API call: `salesAPI.getDailySummary(shopId)`
- Implemented dynamic data loading

**Status:** ✅ Code changes complete, waiting for backend to work

### 2. Frontend Configuration
**File:** `toss-web/.env` (created)

**Content:**
```
NUXT_PUBLIC_API_BASE=https://localhost:5001
```

**Status:** ✅ Created

## 🧪 Testing Commands

### Test Backend Health:
```powershell
# With SSL bypass
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-RestMethod -Uri "https://localhost:5001/health"
```

### Test API Endpoint:
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/daily-summary?shopId=1"
```

### Check Running Processes:
```powershell
Get-Process | Where-Object { $_.ProcessName -like "*Toss*" }
```

### Check Listening Ports:
```powershell
Get-NetTCPConnection | Where-Object { $_.LocalPort -eq 5001 -and $_.State -eq "Listen" }
```

### View Docker Containers:
```powershell
docker ps
```

## 📋 Action Items for User

### Immediate Actions:
1. ✅ Check the PowerShell window where backend is running
2. ✅ Look for error messages or exceptions
3. ✅ Try Solution 1 (Trust development certificate)
4. ✅ Try Solution 2 (Run database migrations)
5. ✅ If still failing, try Solution 4 (HTTP workaround)

### Alternative:
- Copy any error messages from the backend window
- Share them for further diagnosis
- We can troubleshoot the specific error

## 🎯 Expected Outcome

Once the backend is working:
1. Open `http://localhost:3001/sales/pos`
2. The POS statistics should show:
   - Today's Sales (from API, not hardcoded)
   - Today's Transactions (from API)
   - Average Sale (calculated from API data)
   - Cash Float (from API)
3. All values dynamically loaded from `https://localhost:5001/api/Sales/daily-summary?shopId=1`

## 📚 Related Documentation

- `POS_API_INTEGRATION.md` - Details of code changes made
- `TOSS_COMPLETE_FLOW.md` - Complete business flow
- `BROWSER_TESTING_GUIDE.md` - Manual testing steps

---

**Status:** 🔴 BLOCKED - Waiting for backend API to be accessible  
**Next Step:** Trust SSL certificate or check backend error logs  
**Code Changes:** ✅ Complete (POS page ready for API data)
