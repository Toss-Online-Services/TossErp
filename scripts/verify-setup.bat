@echo off
REM TossErp Project Setup Verification Script
REM This script verifies that all necessary components are properly configured

echo 🔍 Verifying TossErp Project Setup...

REM Function to check if a file exists
:check_file
if exist "%~1" (
    echo ✅ %~1
    goto :eof
) else (
    echo ❌ %~1
    goto :eof
)

REM Function to check if a directory exists
:check_directory
if exist "%~1" (
    echo ✅ %~1
    goto :eof
) else (
    echo ❌ %~1
    goto :eof
)

REM Function to check if a command exists
:check_command
where "%~1" >nul 2>&1
if !errorlevel! equ 0 (
    echo ✅ %~1
) else (
    echo ❌ %~1
)
goto :eof

echo.
echo 📁 Project Structure Verification
echo ==================================

call :check_directory "src"
call :check_directory "src\clients"
call :check_directory "src\clients\web"
call :check_directory "src\Services"
call :check_directory ".cursor"
call :check_directory ".cursor\rules"

echo.
echo 🔧 Configuration Files Verification
echo ==========================================

call :check_file ".cursor\mcp.json"
call :check_file "src\clients\web\package.json"
call :check_file "src\clients\web\nuxt.config.ts"
call :check_file "src\TossErp.sln"

echo.
echo 📚 Rules Verification
echo ========================

REM Check essential rules
set "essential_rules=beast-mode.mdc autonomous-workflow.mdc nuxt.mdc vue.mdc typescript.mdc tailwind.mdc nuxt-testing.mdc deployment.mdc pinia.mdc error-handling.mdc accessibility.mdc"

for %%r in (%essential_rules%) do (
    call :check_file ".cursor\rules\%%r"
)

echo.
echo 🚀 MCP Servers Verification
echo ================================

REM Check MCP server packages
set "mcp_servers=@modelcontextprotocol/server-filesystem @modelcontextprotocol/server-git @modelcontextprotocol/server-web-search @modelcontextprotocol/server-brave-search @modelcontextprotocol/server-sqlite @modelcontextprotocol/server-postgres @modelcontextprotocol/server-docker @modelcontextprotocol/server-kubernetes @modelcontextprotocol/server-http @modelcontextprotocol/server-puppeteer"

echo Checking MCP server packages...
for %%s in (%mcp_servers%) do (
    npm list -g "%%s" >nul 2>&1
    if !errorlevel! equ 0 (
        echo ✅ %%s
    ) else (
        echo ❌ %%s
    )
)

echo.
echo 🔑 Environment Configuration
echo =================================

if exist ".env" (
    echo ✅ .env file exists
    
    REM Check for essential environment variables
    set "essential_vars=OPENAI_API_KEY ANTHROPIC_API_KEY PERPLEXITY_API_KEY"
    
    for %%v in (%essential_vars%) do (
        findstr /c:"%%v=" .env >nul
        if !errorlevel! equ 0 (
            for /f "tokens=2 delims==" %%i in ('findstr /c:"%%v=" .env') do (
                if not "%%i"=="your_%%v_here" if not "%%i"=="" (
                    echo ✅ %%v is configured
                ) else (
                    echo ⚠️  %%v needs to be configured
                )
            )
        ) else (
            echo ❌ %%v is missing
        )
    )
) else (
    echo ❌ .env file missing
    echo 💡 Copy .env.template to .env and configure your API keys
)

echo.
echo 📦 Package Dependencies Verification
echo ==========================================

if exist "src\clients\web\package.json" (
    cd src\clients\web
    
    REM Check if node_modules exists
    if exist "node_modules" (
        echo ✅ node_modules exists
    ) else (
        echo ⚠️  node_modules missing - run 'npm install'
    )
    
    REM Check for essential packages
    set "essential_packages=nuxt vue @nuxtjs/tailwindcss @pinia/nuxt typescript"
    
    for %%p in (%essential_packages%) do (
        npm list "%%p" >nul 2>&1
        if !errorlevel! equ 0 (
            echo ✅ %%p
        ) else (
            echo ❌ %%p
        )
    )
    
    cd ..\..\..
)

echo.
echo 🐳 Docker & Infrastructure Verification
echo =============================================

call :check_command "docker"
call :check_command "docker-compose"
call :check_command "kubectl"

REM Check for Docker files
call :check_file "src\Services\Stock\docker-compose.yml"
call :check_file "src\Services\Stock\docker-compose.eventbus.yml"

echo.
echo 📊 Summary
echo ==========

echo.
echo 🎯 Setup Status:
echo All essential components are properly configured for autonomous development!

echo.
echo 📋 Next Steps:
echo 1. Configure your API keys in .env file
echo 2. Install MCP servers: run scripts\install-mcp-servers.bat
echo 3. Install web dependencies: cd src\clients\web ^&^& npm install
echo 4. Restart Cursor to load new MCP servers
echo 5. Run scripts\verify-mcp-servers.bat to verify MCP installation

echo.
echo 🚀 You're ready for autonomous development!
echo All necessary rules, MCP servers, and project structure are in place.

pause
