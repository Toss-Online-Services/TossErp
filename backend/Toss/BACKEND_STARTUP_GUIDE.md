# TOSS Backend Startup Guide

This guide explains how to start the TOSS backend with automatic process cleanup.

---

## 🚀 Quick Start

### Option 1: Start with Aspire Dashboard (Recommended)

```powershell
cd backend/Toss/src/AppHost
.\start-backend.ps1
```

**Features:**
- ✅ Automatic process cleanup (kills existing backend)
- ✅ Starts Aspire Dashboard for monitoring
- ✅ PostgreSQL container management
- ✅ Distributed tracing and logging
- ✅ Service orchestration

**URLs:**
- Backend API: `http://localhost:5000` or `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger/index.html`
- Aspire Dashboard: `https://localhost:17078`

---

### Option 2: Start Web API Only

```powershell
cd backend/Toss/src/Web
.\start-web.ps1
```

**Features:**
- ✅ Automatic process cleanup (kills existing Web API)
- ✅ Direct Web API startup
- ✅ Faster startup (no Aspire overhead)
- ⚠️  Manual PostgreSQL setup required

**URLs:**
- Backend API: `http://localhost:5000` or `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger/index.html`

---

## 📋 Prerequisites

### 1. PostgreSQL Database

**Option A: Using Docker (Recommended)**
```powershell
docker run --name toss-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=TossErp -p 5432:5432 -d postgres:15
```

**Option B: Local PostgreSQL Installation**
- Install PostgreSQL 15+
- Create database: `TossErp`
- Update connection string in `appsettings.json`

### 2. .NET 9 SDK
- Ensure .NET 9 SDK is installed
- Verify: `dotnet --version`

### 3. Build the Solution
```powershell
cd backend/Toss
dotnet build
```

---

## 🔧 How the Scripts Work

### Automatic Process Cleanup

Both scripts automatically:
1. **Detect running backend processes** by:
   - Process name (`dotnet`)
   - Process path (contains `Web` or `AppHost`)
   - Port usage (5000, 5001, 15010, 17078)

2. **Terminate existing processes** gracefully:
   - Display found processes with PIDs
   - Force-kill all detected processes
   - Wait 2 seconds for cleanup

3. **Start fresh instance**:
   - Navigate to correct directory
   - Run `dotnet run` with appropriate flags
   - Display startup information

### Why This Matters

**Problem:** Running multiple backend instances causes:
- ❌ Port conflicts (Address already in use)
- ❌ Database connection pool exhaustion
- ❌ Confusing logs from multiple instances
- ❌ Wasted resources

**Solution:** Scripts ensure only ONE backend instance runs at a time.

---

## 🎯 Common Scenarios

### Scenario 1: "Port 5000 is already in use"

**Before (Manual):**
```powershell
# Find process using port 5000
netstat -ano | findstr :5000
# Kill manually
taskkill /PID <process_id> /F
# Start backend
dotnet run
```

**After (Automatic):**
```powershell
.\start-backend.ps1  # Done! Script handles everything
```

---

### Scenario 2: Multiple Developers on Same Machine

**Problem:** Developer A starts backend, forgets. Developer B starts backend → conflict.

**Solution:** Scripts automatically clean up previous instances.

---

### Scenario 3: Debugging Sessions

**Problem:** VS Code/Rider leaves orphaned processes after debugging.

**Solution:** Run script before each debug session.

---

## 📊 Script Output Example

```
🚀 TOSS Backend Startup Script
================================

🔍 Checking for existing backend processes...
⚠️  Found 2 existing backend process(es):
   • PID 12345: dotnet
   • PID 12346: dotnet

🔪 Terminating existing processes...
   ✅ Killed PID 12345
   ✅ Killed PID 12346

📂 Working Directory: C:\...\backend\Toss\src\AppHost

🚀 Starting TOSS Backend (AppHost)...
================================

Backend will be available at:
  • HTTP:  http://localhost:5000
  • HTTPS: https://localhost:5001
  • Swagger: http://localhost:5000/swagger/index.html
  • Aspire Dashboard: https://localhost:17078

Press Ctrl+C to stop the backend
================================

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## 🛠️ Troubleshooting

### Issue: "Access Denied" when killing processes

**Cause:** Processes owned by another user or running with elevated privileges.

**Solution:**
```powershell
# Run script as Administrator
Start-Process powershell -ArgumentList "-File .\start-backend.ps1" -Verb RunAs
```

---

### Issue: PostgreSQL not running

**Symptoms:**
```
❌ Npgsql.NpgsqlException: Connection refused
```

**Solution:**
```powershell
# Check if PostgreSQL is running
docker ps | findstr postgres

# If not running, start it
docker start toss-postgres

# Or create new container
docker run --name toss-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=TossErp -p 5432:5432 -d postgres:15
```

---

### Issue: Script doesn't find processes

**Cause:** Non-standard process names or ports.

**Solution:** Edit script and add custom detection logic:
```powershell
# In start-backend.ps1, add custom process detection
$customProcesses = Get-Process -Name "MyCustomProcessName" -ErrorAction SilentlyContinue
```

---

## 🎨 Integration with IDEs

### Visual Studio Code

Create `.vscode/tasks.json`:
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Start TOSS Backend",
      "type": "shell",
      "command": "powershell",
      "args": [
        "-ExecutionPolicy", "Bypass",
        "-File", "${workspaceFolder}/backend/Toss/src/AppHost/start-backend.ps1"
      ],
      "group": "build",
      "presentation": {
        "reveal": "always",
        "panel": "new"
      }
    }
  ]
}
```

Then run: `Ctrl+Shift+P` → `Tasks: Run Task` → `Start TOSS Backend`

---

### JetBrains Rider

1. Go to `Run` → `Edit Configurations`
2. Add `Shell Script` configuration
3. Set script path: `backend/Toss/src/AppHost/start-backend.ps1`
4. Set interpreter: `powershell.exe`

---

## 📝 Advanced Usage

### Custom Ports

Edit the script to check additional ports:
```powershell
$portsToCheck = @(5000, 5001, 8080, 8443)  # Add your custom ports
```

### Logging

Redirect output to log file:
```powershell
.\start-backend.ps1 | Tee-Object -FilePath "backend-startup.log"
```

### Scheduled Restart

Create scheduled task to restart backend daily:
```powershell
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
  -Argument "-File C:\path\to\start-backend.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 2am
Register-ScheduledTask -TaskName "TOSS Backend Restart" `
  -Action $action -Trigger $trigger
```

---

## 🔐 Security Notes

**⚠️ Important:**
- Scripts kill processes by PID - ensure you trust the script
- Running as Administrator grants full system access
- Review script contents before execution
- Add scripts to version control with proper review

---

## ✅ Summary

**When to use `start-backend.ps1` (AppHost):**
- ✅ Full development environment
- ✅ Need Aspire Dashboard monitoring
- ✅ Working with microservices
- ✅ Team collaboration

**When to use `start-web.ps1` (Web API only):**
- ✅ Quick testing
- ✅ Minimal resource usage
- ✅ Direct API development
- ✅ CI/CD pipelines

---

## 📞 Support

**Issues with scripts?**
1. Check PowerShell execution policy: `Get-ExecutionPolicy`
2. Set if needed: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
3. Verify .NET installation: `dotnet --version`
4. Check PostgreSQL: `docker ps` or `psql --version`

**Still having problems?**
- Review logs in console output
- Check `backend/Toss/logs/` directory
- Verify ports are not blocked by firewall
- Ensure sufficient system resources (RAM, disk space)

