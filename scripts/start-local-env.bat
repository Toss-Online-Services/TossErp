@echo off
echo 🚀 Starting TOSS ERP Complete Local Development Environment
echo =============================================================

echo.
echo 🔍 Checking prerequisites...

rem Check if Docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker Desktop.
    echo.
    pause
    exit /b 1
)
echo ✅ Docker is running

rem Check if Node.js is available
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Node.js is not installed or not in PATH.
    echo Please install Node.js from https://nodejs.org/
    echo.
    pause
    exit /b 1
)
echo ✅ Node.js is available

echo.
echo 🐳 Starting infrastructure services...
docker-compose -f docker-compose.local.yml up -d postgres redis rabbitmq

echo.
echo ⏱️  Waiting for infrastructure to be ready...
timeout /t 15 >nul

echo.
echo 🔧 Starting additional development tools...
docker-compose -f docker-compose.local.yml up -d pgadmin redis-commander mailhog seq jaeger minio

echo.
echo ⏱️  Waiting for dev tools to be ready...
timeout /t 10 >nul

echo.
echo 🔧 Starting backend services...
cd docker
docker-compose up -d identity-api stock-api crm-api gateway

echo.
echo ⏱️  Waiting for backend services to be ready...
timeout /t 10 >nul

echo.
echo 🌐 Starting web client development server...
cd ..\src\clients\web

rem Check if node_modules exists
if not exist "node_modules" (
    echo 📦 Installing web client dependencies...
    npm install
    if %errorlevel% neq 0 (
        echo ❌ Failed to install dependencies
        pause
        exit /b 1
    )
)

echo 🚀 Starting Nuxt development server...
start "TOSS ERP Web Client" cmd /k "npm run dev"

echo.
echo ✅ TOSS ERP Development Environment Started!
echo.
echo 📍 Access Points:
echo.
echo   🌐 Application:
echo      💻 Web Client:        http://localhost:3001
echo      🔗 API Gateway:       http://localhost:8080
echo      📊 API Documentation: http://localhost:8080/swagger
echo.
echo   🛠️  Development Tools:
echo      🗄️  Database Admin:    http://localhost:5050 (admin@tosserp.com/admin123)
echo      📦 Redis Commander:   http://localhost:8081 (admin/admin123)
echo      📧 Email Testing:     http://localhost:8025
echo      📊 Distributed Tracing: http://localhost:16686
echo      📝 Structured Logging: http://localhost:5341
echo      📁 File Storage UI:   http://localhost:9001 (minioadmin/minioadmin123)
echo      🐰 RabbitMQ Admin:    http://localhost:15672 (guest/guest)
echo.
echo   📡 Direct Services:
echo      🗄️  PostgreSQL:       localhost:5432 (postgres/postgres123)
echo      📦 Redis:             localhost:6379
echo      🐰 RabbitMQ:          localhost:5672
echo      📁 MinIO S3:          localhost:9000
echo      📧 SMTP Server:       localhost:1025
echo.
echo 💡 Tips:
echo    - Use Ctrl+C in the web client window to stop the frontend
echo    - All services will continue running in the background
echo    - Check service logs: docker-compose logs [service-name]
echo    - View all running containers: docker ps
echo.
echo 🛑 To stop all services: scripts\stop-local-env.bat
echo.
echo 🎉 Happy coding with TOSS ERP!
echo.
pause
