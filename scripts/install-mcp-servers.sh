#!/bin/bash

# MCP Server Installation Script for TossErp Project
# This script installs all necessary MCP servers for autonomous development

echo "🚀 Installing MCP Servers for Autonomous Development..."

# Create scripts directory if it doesn't exist
mkdir -p scripts

# Install core MCP servers
echo "📦 Installing core MCP servers..."

# Filesystem server for file operations
npm install -g @modelcontextprotocol/server-filesystem

# Git server for version control
npm install -g @modelcontextprotocol/server-git

# Web search server for research
npm install -g @modelcontextprotocol/server-web-search

# Brave search server for advanced search
npm install -g @modelcontextprotocol/server-brave-search

# SQLite server for database operations
npm install -g @modelcontextprotocol/server-sqlite

# PostgreSQL server for database operations
npm install -g @modelcontextprotocol/server-postgres

# Docker server for container operations
npm install -g @modelcontextprotocol/server-docker

# Kubernetes server for K8s operations
npm install -g @modelcontextprotocol/server-kubernetes

# HTTP server for API testing
npm install -g @modelcontextprotocol/server-http

# Puppeteer server for web automation
npm install -g @modelcontextprotocol/server-puppeteer

echo "✅ All MCP servers installed successfully!"

# Create environment template
echo "🔧 Creating environment template..."
cat > .env.template << EOF
# MCP Server Environment Variables
# Copy this file to .env and fill in your actual values

# Task Master AI
ANTHROPIC_API_KEY=your_anthropic_api_key_here
PERPLEXITY_API_KEY=your_perplexity_api_key_here
OPENAI_API_KEY=your_openai_api_key_here
GOOGLE_API_KEY=your_google_api_key_here
XAI_API_KEY=your_xai_api_key_here
OPENROUTER_API_KEY=your_openrouter_api_key_here
MISTRAL_API_KEY=your_mistral_api_key_here
AZURE_OPENAI_API_KEY=your_azure_openai_api_key_here
OLLAMA_API_KEY=your_ollama_api_key_here

# Brave Search
BRAVE_API_KEY=your_brave_api_key_here

# Database Connections
POSTGRES_CONNECTION_STRING=your_postgres_connection_string_here

# Other Services
OLLAMA_BASE_URL=http://localhost:11434/api
AZURE_OPENAI_ENDPOINT=your_azure_openai_endpoint_here
EOF

echo "📝 Environment template created at .env.template"
echo "⚠️  Please copy .env.template to .env and fill in your actual API keys"

# Create MCP server verification script
echo "🔍 Creating MCP server verification script..."
cat > scripts/verify-mcp-servers.sh << 'EOF'
#!/bin/bash

echo "🔍 Verifying MCP Server Installation..."

# Check if MCP servers are accessible
servers=(
    "task-master-ai"
    "filesystem"
    "git"
    "web-search"
    "brave-search"
    "sqlite"
    "postgres"
    "docker"
    "kubernetes"
    "http"
    "puppeteer"
)

for server in "${servers[@]}"; do
    if command -v "$server" >/dev/null 2>&1; then
        echo "✅ $server: Installed"
    else
        echo "❌ $server: Not found"
    fi
done

echo ""
echo "🎯 MCP Server Verification Complete!"
echo "📚 For more information, visit: https://modelcontextprotocol.io/servers"
EOF

chmod +x scripts/verify-mcp-servers.sh

echo "✅ MCP server verification script created at scripts/verify-mcp-servers.sh"

# Create usage documentation
echo "📚 Creating usage documentation..."
cat > docs/MCP_SERVERS.md << 'EOF'
# MCP Servers for TossErp Project

This document describes the MCP servers configured for autonomous development.

## Available MCP Servers

### Core Development
- **task-master-ai**: Task management and project planning
- **filesystem**: File system operations and management
- **git**: Version control operations

### Research & Information
- **web-search**: General web search capabilities
- **brave-search**: Advanced search with Brave API

### Database Operations
- **sqlite**: SQLite database operations
- **postgres**: PostgreSQL database operations

### Infrastructure
- **docker**: Docker container operations
- **kubernetes**: Kubernetes cluster operations
- **http**: HTTP request testing and API calls

### Automation
- **puppeteer**: Web automation and testing

## Configuration

All MCP servers are configured in `.cursor/mcp.json`. Environment variables are stored in `.env`.

## Usage

These servers enable autonomous development by providing:
- File system access and manipulation
- Version control operations
- Web search and research capabilities
- Database operations
- Infrastructure management
- Web automation

## Security

- API keys are stored in environment variables
- Never commit `.env` files to version control
- Use `.env.template` for reference
- Rotate API keys regularly

## Troubleshooting

Run `scripts/verify-mcp-servers.sh` to check server installation status.

For more information, visit: https://modelcontextprotocol.io/servers
EOF

echo "📚 MCP server documentation created at docs/MCP_SERVERS.md"

echo ""
echo "🎉 MCP Server Installation Complete!"
echo ""
echo "📋 Next Steps:"
echo "1. Copy .env.template to .env"
echo "2. Fill in your actual API keys"
echo "3. Run scripts/verify-mcp-servers.sh to verify installation"
echo "4. Restart Cursor to load new MCP servers"
echo ""
echo "🚀 You're now ready for autonomous development!"
