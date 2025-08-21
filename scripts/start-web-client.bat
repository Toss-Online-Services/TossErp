@echo off
echo Starting TOSS ERP Web Client...
cd /d "%~dp0..\src\clients\web"
echo.
echo ============================================
echo   TOSS ERP Web Client Development Server
echo ============================================
echo.
echo 🌐 Frontend: http://localhost:3001
echo 🔗 API Gateway: http://localhost:8080
echo 📊 API Documentation: http://localhost:8080/swagger
echo.
echo ⚠️  Note: Make sure backend services are running
echo    Use start-local-env.bat for complete setup
echo.
echo Press Ctrl+C to stop the server
echo.

rem Check if node_modules exists
if not exist "node_modules" (
    echo 📦 Installing dependencies...
    npm install
    if %errorlevel% neq 0 (
        echo ❌ Failed to install dependencies
        pause
        exit /b 1
    )
)

npm run dev
pause
