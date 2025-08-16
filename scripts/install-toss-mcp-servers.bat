@echo off
echo ========================================
echo TOSS MCP Servers Installation Script
echo ========================================
echo.

echo Installing Core MCP Servers...
call npm install -g @modelcontextprotocol/server-filesystem
call npm install -g @modelcontextprotocol/server-git
call npm install -g @modelcontextprotocol/server-web-search
call npm install -g @modelcontextprotocol/server-brave-search
call npm install -g @modelcontextprotocol/server-sqlite
call npm install -g @modelcontextprotocol/server-postgres
call npm install -g @modelcontextprotocol/server-docker
call npm install -g @modelcontextprotocol/server-kubernetes
call npm install -g @modelcontextprotocol/server-http
call npm install -g @modelcontextprotocol/server-puppeteer

echo.
echo Installing TOSS-Specific MCP Servers...
call npm install -g @modelcontextprotocol/server-redis
call npm install -g @modelcontextprotocol/server-rabbitmq
call npm install -g @modelcontextprotocol/server-elasticsearch
call npm install -g @modelcontextprotocol/server-prometheus
call npm install -g @modelcontextprotocol/server-grafana
call npm install -g @modelcontextprotocol/server-terraform
call npm install -g @modelcontextprotocol/server-aws
call npm install -g @modelcontextprotocol/server-azure
call npm install -g @modelcontextprotocol/server-gcp

echo.
echo Installing Task Master AI...
call npm install -g task-master-ai

echo.
echo ========================================
echo Installation Complete!
echo ========================================
echo.
echo Next steps:
echo 1. Configure API keys in .cursor/mcp.json
echo 2. Run scripts\verify-toss-setup.bat to verify installation
echo 3. Start developing with TOSS!
echo.
pause
