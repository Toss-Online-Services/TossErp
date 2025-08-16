@echo off
echo ========================================
echo    TOSS ERP - Launch All Applications
echo ========================================
echo.

REM Set title for the console window
title TOSS ERP - All Applications

REM Check if required tools are installed
echo Checking prerequisites...
echo.

REM Check for .NET
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ .NET SDK not found. Please install .NET 8.0 SDK first.
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
) else (
    echo ✅ .NET SDK found
)

REM Check for Node.js
where node >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ Node.js not found. Please install Node.js first.
    echo Download from: https://nodejs.org/
    pause
    exit /b 1
) else (
    echo ✅ Node.js found
)

REM Check for Flutter
where flutter >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ Flutter not found. Please install Flutter first.
    echo Download from: https://flutter.dev/docs/get-started/install
    pause
    exit /b 1
) else (
    echo ✅ Flutter found
)

echo.
echo All prerequisites are satisfied!
echo.
echo Starting applications...
echo.

REM Create environment files if they don't exist
echo Creating environment files...
if not exist "src\clients\mobile\.env" (
    copy "src\clients\mobile\env.template" "src\clients\mobile\.env" >nul
    echo ✅ Mobile environment file created
) else (
    echo ✅ Mobile environment file already exists
)

if not exist "src\clients\web\.env" (
    copy "src\clients\web\env.template" "src\clients\web\.env" >nul
    echo ✅ Web environment file created
) else (
    echo ✅ Web environment file already exists
)

if not exist "src\clients\admin\.env" (
    copy "src\clients\admin\env.template" "src\clients\admin\.env" >nul
    echo ✅ Admin environment file created
) else (
    echo ✅ Admin environment file already exists
)

echo.

REM Start Gateway (in a new console window)
echo 🚀 Starting API Gateway...
start "TOSS ERP - Gateway" cmd /k "cd /d %~dp0src\Gateway && echo Starting Gateway... && dotnet run"

REM Wait a moment for Gateway to start
timeout /t 5 /nobreak >nul

REM Start Stock API (in a new console window)
echo 🚀 Starting Stock API...
start "TOSS ERP - Stock API" cmd /k "cd /d %~dp0src\Services\Stock\Stock.API && echo Starting Stock API... && dotnet run"

REM Wait a moment for Stock API to start
timeout /t 5 /nobreak >nul

REM Start Mobile Client (in a new console window)
echo 🚀 Starting Mobile Client...
start "TOSS ERP - Mobile Client" cmd /k "cd /d %~dp0src\clients\mobile && echo Starting Mobile Client... && flutter run -d web-server --web-port 5000"

REM Wait a moment for Mobile client to start
timeout /t 5 /nobreak >nul

REM Start Web Client (in a new console window)
echo 🚀 Starting Web Client...
start "TOSS ERP - Web Client" cmd /k "cd /d %~dp0src\clients\web && echo Starting Web Client... && npm run dev"

REM Wait a moment for Web client to start
timeout /t 5 /nobreak >nul

REM Start Admin Client (in a new console window)
echo 🚀 Starting Admin Client...
start "TOSS ERP - Admin Client" cmd /k "cd /d %~dp0src\clients\admin && echo Starting Admin Client... && npm start"

echo.
echo ========================================
echo    All Applications Started!
echo ========================================
echo.
echo 🌐 Applications will be available at:
echo    Gateway:        http://localhost:8080
echo    Stock API:      http://localhost:5001
echo    Mobile Client:  http://localhost:5000
echo    Web Client:     http://localhost:5173
echo    Admin Client:   http://localhost:3000
echo.
echo 📱 Mobile Client (Flutter Web) will open in your browser
echo 🌐 Web Client (Nuxt) will open in your browser  
echo ⚙️  Admin Client (React) will open in your browser
echo.
echo 🔌 Gateway and Stock API are running in background
echo.
echo Press any key to open all applications in your browser...
pause >nul

REM Open all applications in browser
echo Opening applications in browser...
start http://localhost:8080/health
start http://localhost:5000
start http://localhost:5173
start http://localhost:3000

echo.
echo 🎉 All applications are now running!
echo.
echo To stop all applications:
echo 1. Close each console window
echo 2. Or press Ctrl+C in each window
echo.
echo Happy coding! 🚀
pause
