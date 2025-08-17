@echo off
echo ========================================
echo    TOSS ERP - Docker Compose Launch
echo ========================================
echo.

REM Set title for the console window
title TOSS ERP - Docker Compose

REM Check if Docker is running
echo Checking Docker status...
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker Desktop first.
    echo Download from: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
) else (
    echo ✅ Docker is running
)

REM Check if Docker Compose is available
echo Checking Docker Compose...
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose not found. Please install Docker Compose.
    pause
    exit /b 1
) else (
    echo ✅ Docker Compose is available
)

echo.
echo All prerequisites are satisfied!
echo.

REM Parse command line arguments
set "MODE=dev"
set "SKIP_BUILD="
set "OPEN_BROWSER="

:parse_args
if "%1"=="" goto :end_parse
if "%1"=="-FullStack" set "MODE=full"
if "%1"=="-DevOnly" set "MODE=dev"
if "%1"=="-SkipBuild" set "SKIP_BUILD=--no-build"
if "%1"=="-OpenBrowser" set "OPEN_BROWSER=true"
shift
goto :parse_args
:end_parse

REM Determine which compose file to use
if "%MODE%"=="full" (
    set "COMPOSE_FILE=docker-compose.local.yml"
    echo Using Full Stack mode with file: %COMPOSE_FILE%
) else (
    set "COMPOSE_FILE=docker-compose.dev.yml"
    echo Using Development mode with file: %COMPOSE_FILE%
)

echo.

REM Check if compose file exists
if not exist "%COMPOSE_FILE%" (
    echo ❌ Docker Compose file not found: %COMPOSE_FILE%
    pause
    exit /b 1
)

REM Start services
echo Starting Docker Compose services...
echo.

if defined SKIP_BUILD (
    echo Starting services (skipping build)...
    docker-compose -f %COMPOSE_FILE% up -d
) else (
    echo Building and starting services...
    docker-compose -f %COMPOSE_FILE% up -d --build
)

if %errorlevel% neq 0 (
    echo ❌ Failed to start services. Check the error messages above.
    pause
    exit /b 1
)

echo.
echo ✅ Services started successfully!
echo.

REM Wait for services to be ready
echo Waiting for services to be ready...
timeout /t 10 /nobreak >nul

REM Show service URLs
echo.
echo ========================================
echo    Service URLs
echo ========================================
echo.

if "%MODE%"=="full" (
    echo 🌐 Main Application:
    echo   http://localhost (Nginx Reverse Proxy)
    echo.
    echo 📱 Client Applications:
    echo   Mobile Client:  http://localhost/mobile/
    echo   Web Client:     http://localhost/web/
    echo   Admin Client:   http://localhost/admin/
    echo.
    echo 🔌 Backend Services:
    echo   API Gateway:    http://localhost/api/
    echo   Stock API:      http://localhost/stock/
    echo.
    echo 📊 Monitoring:
    echo   Prometheus:     http://localhost/monitoring/prometheus/
    echo   Grafana:        http://localhost/monitoring/grafana/
    echo   RabbitMQ:       http://localhost/rabbitmq/
) else (
    echo 🔌 Backend Services:
    echo   API Gateway:    http://localhost:8080
    echo   Stock API:      http://localhost:5001
    echo.
    echo 🗄️  Infrastructure:
    echo   PostgreSQL:     localhost:5432
    echo   Redis:          localhost:6379
    echo   RabbitMQ:       http://localhost:15672
)

echo.
echo 🔑 Default Credentials:
echo   PostgreSQL: postgres/postgres123
echo   RabbitMQ:   guest/guest
if "%MODE%"=="full" (
    echo   Monitoring: admin/admin123
)

echo.
echo ========================================
echo    Management Commands
echo ========================================
echo.
echo 📊 View logs:
echo   docker-compose -f %COMPOSE_FILE% logs -f
echo.
echo 🔄 Restart services:
echo   docker-compose -f %COMPOSE_FILE% restart
echo.
echo 🛑 Stop services:
echo   docker-compose -f %COMPOSE_FILE% down
echo.
echo 🧹 Clean up (removes volumes):
echo   docker-compose -f %COMPOSE_FILE% down -v

REM Open browser if requested
if defined OPEN_BROWSER (
    echo.
    echo Opening services in browser...
    if "%MODE%"=="full" (
        start http://localhost
        start http://localhost/api/health
    ) else (
        start http://localhost:8080/health
        start http://localhost:5001/health
        start http://localhost:15672
    )
) else (
    echo.
    echo Press any key to open services in browser...
    pause >nul
    echo Opening services in browser...
    if "%MODE%"=="full" (
        start http://localhost
        start http://localhost/api/health
    ) else (
        start http://localhost:8080/health
        start http://localhost:5001/health
        start http://localhost:15672
    )
)

echo.
echo 🎉 All services are now running!
echo.
echo To stop all services:
echo   docker-compose -f %COMPOSE_FILE% down
echo.
echo Happy coding! 🚀
pause
