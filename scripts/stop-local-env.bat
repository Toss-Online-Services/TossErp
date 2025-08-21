@echo off
echo 🛑 Stopping TOSS ERP Local Development Environment
echo ===================================================

echo.
echo 🔌 Stopping web client processes...
taskkill /f /im node.exe 2>nul
echo ✅ Web client processes stopped

echo.
echo 🐳 Stopping backend services...
cd docker
docker-compose down
echo ✅ Backend services stopped

echo.
echo 🐳 Stopping infrastructure and development tools...
cd ..
docker-compose -f docker-compose.local.yml down
echo ✅ Infrastructure services stopped

echo.
echo 🧹 Cleaning up (optional - keeps data volumes)...
echo    To remove all data: docker-compose -f docker-compose.local.yml down -v
echo    To remove images: docker system prune

echo.
echo ✅ All TOSS ERP services stopped!
echo.
echo 📊 Service Status:
docker ps --filter "name=tosserp" --format "table {{.Names}}\t{{.Status}}" 2>nul
if %errorlevel% neq 0 (
    echo    No TOSS ERP containers running
)

echo.
echo 💡 To restart: scripts\start-local-env.bat
echo.
pause
