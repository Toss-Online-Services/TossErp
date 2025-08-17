@echo off
echo ===============================================
echo     TOSS ERP - Full Stack Launch
echo ===============================================
echo.

REM Set title for the console window
title TOSS ERP - Full Stack Launch

REM Check if Docker is running
echo 🔍 Checking Docker status...
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker Desktop first.
    echo    Download from: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
) else (
    echo ✅ Docker is running
)

REM Check if Docker Compose is available
echo 🔍 Checking Docker Compose...
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose not found. Please install Docker Compose.
    pause
    exit /b 1
) else (
    echo ✅ Docker Compose is available
)

echo.
echo 🚀 Starting TOSS ERP Full Stack...

REM Stop any existing containers
echo 🛑 Stopping existing containers...
docker-compose -f docker-compose.full-stack.yml down >nul 2>&1

REM Launch the full stack
if "%1"=="--skip-build" (
    echo 📦 Launching services (skipping build)...
    docker-compose -f docker-compose.full-stack.yml up -d
) else (
    echo 🔨 Building and launching services...
    docker-compose -f docker-compose.full-stack.yml up -d --build
)

if %errorlevel% equ 0 (
    echo.
    echo 🎉 TOSS ERP Full Stack launched successfully!
    echo.
    
    REM Wait for services to be ready
    echo ⏳ Waiting for services to be ready...
    timeout /t 30 /nobreak >nul
    
    REM Show service status
    echo 📊 Service Status:
    docker-compose -f docker-compose.full-stack.yml ps
    
    echo.
    echo 🌐 Access Points:
    echo    Main Portal:     http://localhost/
    echo    Mobile Client:   http://localhost:3000/
    echo    Web Client:      http://localhost:3001/
    echo    API Gateway:     http://localhost:8080/
    echo    RabbitMQ:        http://localhost:15672/
    echo    PostgreSQL:      localhost:5432
    echo    Redis:           localhost:6379
    
    echo.
    echo 📱 Web Client includes comprehensive admin functionality
    
    REM Open browser if requested
    if "%1"=="--open-browser" (
        echo 🌐 Opening main portal in browser...
        start http://localhost
    )
    
    echo.
    echo 💡 Use 'docker-compose -f docker-compose.full-stack.yml logs -f' to view logs
    echo 💡 Use 'docker-compose -f docker-compose.full-stack.yml down' to stop services
    
) else (
    echo ❌ Failed to launch TOSS ERP Full Stack
    echo Check the logs above for errors
    pause
    exit /b 1
)

echo.
echo 🎉 TOSS ERP Full Stack is ready!
pause
