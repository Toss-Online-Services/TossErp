@echo off
echo ========================================
echo TOSS Development Setup Verification
echo ========================================
echo.

echo Checking Project Structure...
if exist "src\" (
    echo ✓ Source directory exists
) else (
    echo ✗ Source directory missing
)

if exist "src\clients\web\" (
    echo ✓ Web client directory exists
) else (
    echo ✗ Web client directory missing
)

if exist "src\clients\mobile\" (
    echo ✓ Mobile client directory exists
) else (
    echo ✗ Mobile client directory missing
)

if exist "src\Services\" (
    echo ✓ Services directory exists
) else (
    echo ✗ Services directory missing
)

echo.
echo Checking Configuration Files...
if exist ".cursor\rules\toss-erp.mdc" (
    echo ✓ TOSS ERP rules exist
) else (
    echo ✗ TOSS ERP rules missing
)

if exist ".cursor\mcp.json" (
    echo ✓ MCP configuration exists
) else (
    echo ✗ MCP configuration missing
)

if exist "prd.txt" (
    echo ✓ PRD specification exists
) else (
    echo ✗ PRD specification missing
)

echo.
echo Checking MCP Server Installation...
echo.

echo Core MCP Servers:
call npx -y @modelcontextprotocol/server-filesystem --version >nul 2>&1 && echo ✓ Filesystem server || echo ✗ Filesystem server
call npx -y @modelcontextprotocol/server-git --version >nul 2>&1 && echo ✓ Git server || echo ✗ Git server
call npx -y @modelcontextprotocol/server-web-search --version >nul 2>&1 && echo ✓ Web search server || echo ✗ Web search server
call npx -y @modelcontextprotocol/server-brave-search --version >nul 2>&1 && echo ✓ Brave search server || echo ✗ Brave search server
call npx -y @modelcontextprotocol/server-sqlite --version >nul 2>&1 && echo ✓ SQLite server || echo ✗ SQLite server
call npx -y @modelcontextprotocol/server-postgres --version >nul 2>&1 && echo ✓ PostgreSQL server || echo ✗ PostgreSQL server
call npx -y @modelcontextprotocol/server-docker --version >nul 2>&1 && echo ✓ Docker server || echo ✗ Docker server
call npx -y @modelcontextprotocol/server-kubernetes --version >nul 2>&1 && echo ✓ Kubernetes server || echo ✗ Kubernetes server
call npx -y @modelcontextprotocol/server-http --version >nul 2>&1 && echo ✓ HTTP server || echo ✗ HTTP server
call npx -y @modelcontextprotocol/server-puppeteer --version >nul 2>&1 && echo ✓ Puppeteer server || echo ✗ Puppeteer server

echo.
echo TOSS-Specific MCP Servers:
call npx -y @modelcontextprotocol/server-redis --version >nul 2>&1 && echo ✓ Redis server || echo ✗ Redis server
call npx -y @modelcontextprotocol/server-rabbitmq --version >nul 2>&1 && echo ✓ RabbitMQ server || echo ✗ RabbitMQ server
call npx -y @modelcontextprotocol/server-elasticsearch --version >nul 2>&1 && echo ✓ Elasticsearch server || echo ✗ Elasticsearch server
call npx -y @modelcontextprotocol/server-prometheus --version >nul 2>&1 && echo ✓ Prometheus server || echo ✗ Prometheus server
call npx -y @modelcontextprotocol/server-grafana --version >nul 2>&1 && echo ✓ Grafana server || echo ✗ Grafana server
call npx -y @modelcontextprotocol/server-terraform --version >nul 2>&1 && echo ✓ Terraform server || echo ✗ Terraform server
call npx -y @modelcontextprotocol/server-aws --version >nul 2>&1 && echo ✓ AWS server || echo ✗ AWS server
call npx -y @modelcontextprotocol/server-azure --version >nul 2>&1 && echo ✓ Azure server || echo ✗ Azure server
call npx -y @modelcontextprotocol/server-gcp --version >nul 2>&1 && echo ✓ GCP server || echo ✗ GCP server

echo.
echo Checking Task Master AI...
call npx -y task-master-ai --version >nul 2>&1 && echo ✓ Task Master AI || echo ✗ Task Master AI

echo.
echo Checking Development Tools...
call dotnet --version >nul 2>&1 && echo ✓ .NET SDK || echo ✗ .NET SDK
call flutter --version >nul 2>&1 && echo ✓ Flutter SDK || echo ✗ Flutter SDK
call node --version >nul 2>&1 && echo ✓ Node.js || echo ✗ Node.js
call npm --version >nul 2>&1 && echo ✓ npm || echo ✗ npm

echo.
echo Checking Docker...
call docker --version >nul 2>&1 && echo ✓ Docker || echo ✗ Docker

echo.
echo ========================================
echo Verification Complete!
echo ========================================
echo.
echo If any items show ✗, please:
echo 1. Run scripts\install-toss-mcp-servers.bat
echo 2. Install missing development tools
echo 3. Configure API keys in .cursor/mcp.json
echo.
pause
