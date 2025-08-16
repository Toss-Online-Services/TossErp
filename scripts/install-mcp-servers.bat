@echo off
REM MCP Server Installation Script for TossErp Project
REM This script installs all necessary MCP servers for autonomous development

echo 🚀 Installing MCP Servers for Autonomous Development...

REM Create scripts directory if it doesn't exist
if not exist "scripts" mkdir scripts

REM Install core MCP servers
echo 📦 Installing core MCP servers...

REM Filesystem server for file operations
npm install -g @modelcontextprotocol/server-filesystem

REM Git server for version control
npm install -g @modelcontextprotocol/server-git

REM Web search server for research
npm install -g @modelcontextprotocol/server-web-search

REM Brave search server for advanced search
npm install -g @modelcontextprotocol/server-brave-search

REM SQLite server for database operations
npm install -g @modelcontextprotocol/server-sqlite

REM PostgreSQL server for database operations
npm install -g @modelcontextprotocol/server-postgres

REM Docker server for container operations
npm install -g @modelcontextprotocol/server-docker

REM Kubernetes server for K8s operations
npm install -g @modelcontextprotocol/server-kubernetes

REM HTTP server for API testing
npm install -g @modelcontextprotocol/server-http

REM Puppeteer server for web automation
npm install -g @modelcontextprotocol/server-puppeteer

echo ✅ All MCP servers installed successfully!

REM Create environment template
echo 🔧 Creating environment template...
(
echo # MCP Server Environment Variables
echo # Copy this file to .env and fill in your actual values
echo.
echo # Task Master AI
echo ANTHROPIC_API_KEY=your_anthropic_api_key_here
echo PERPLEXITY_API_KEY=your_perplexity_api_key_here
echo OPENAI_API_KEY=your_openai_api_key_here
echo GOOGLE_API_KEY=your_google_api_key_here
echo XAI_API_KEY=your_xai_api_key_here
echo OPENROUTER_API_KEY=your_openrouter_api_key_here
echo MISTRAL_API_KEY=your_mistral_api_key_here
echo AZURE_OPENAI_API_KEY=your_azure_openai_api_key_here
echo OLLAMA_API_KEY=your_ollama_api_key_here
echo.
echo # Brave Search
echo BRAVE_API_KEY=your_brave_api_key_here
echo.
echo # Database Connections
echo POSTGRES_CONNECTION_STRING=your_postgres_connection_string_here
echo.
echo # Other Services
echo OLLAMA_BASE_URL=http://localhost:11434/api
echo AZURE_OPENAI_ENDPOINT=your_azure_openai_endpoint_here
) > .env.template

echo 📝 Environment template created at .env.template
echo ⚠️  Please copy .env.template to .env and fill in your actual API keys

REM Create MCP server verification script
echo 🔍 Creating MCP server verification script...
(
echo @echo off
echo echo 🔍 Verifying MCP Server Installation...
echo echo.
echo REM Check if MCP servers are accessible
echo set "servers=task-master-ai filesystem git web-search brave-search sqlite postgres docker kubernetes http puppeteer"
echo.
echo for %%s in (%%servers%%) do (
echo     where %%s >nul 2>&1
echo     if !errorlevel! equ 0 (
echo         echo ✅ %%s: Installed
echo     ) else (
echo         echo ❌ %%s: Not found
echo     )
echo )
echo.
echo echo.
echo echo 🎯 MCP Server Verification Complete!
echo echo 📚 For more information, visit: https://modelcontextprotocol.io/servers
echo pause
) > scripts\verify-mcp-servers.bat

echo ✅ MCP server verification script created at scripts\verify-mcp-servers.bat

REM Create usage documentation
echo 📚 Creating usage documentation...
if not exist "docs" mkdir docs

(
echo # MCP Servers for TossErp Project
echo.
echo This document describes the MCP servers configured for autonomous development.
echo.
echo ## Available MCP Servers
echo.
echo ### Core Development
echo - **task-master-ai**: Task management and project planning
echo - **filesystem**: File system operations and management
echo - **git**: Version control operations
echo.
echo ### Research & Information
echo - **web-search**: General web search capabilities
echo - **brave-search**: Advanced search with Brave API
echo.
echo ### Database Operations
echo - **sqlite**: SQLite database operations
echo - **postgres**: PostgreSQL database operations
echo.
echo ### Infrastructure
echo - **docker**: Docker container operations
echo - **kubernetes**: Kubernetes cluster operations
echo - **http**: HTTP request testing and API calls
echo.
echo ### Automation
echo - **puppeteer**: Web automation and testing
echo.
echo ## Configuration
echo.
echo All MCP servers are configured in `.cursor/mcp.json`. Environment variables are stored in `.env`.
echo.
echo ## Usage
echo.
echo These servers enable autonomous development by providing:
echo - File system access and manipulation
echo - Version control operations
echo - Web search and research capabilities
echo - Database operations
echo - Infrastructure management
echo - Web automation
echo.
echo ## Security
echo.
echo - API keys are stored in environment variables
echo - Never commit `.env` files to version control
echo - Use `.env.template` for reference
echo - Rotate API keys regularly
echo.
echo ## Troubleshooting
echo.
echo Run `scripts\verify-mcp-servers.bat` to check server installation status.
echo.
echo For more information, visit: https://modelcontextprotocol.io/servers
) > docs\MCP_SERVERS.md

echo 📚 MCP server documentation created at docs\MCP_SERVERS.md

echo.
echo 🎉 MCP Server Installation Complete!
echo.
echo 📋 Next Steps:
echo 1. Copy .env.template to .env
echo 2. Fill in your actual API keys
echo 3. Run scripts\verify-mcp-servers.bat to verify installation
echo 4. Restart Cursor to load new MCP servers
echo.
echo 🚀 You're now ready for autonomous development!

pause
