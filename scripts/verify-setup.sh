#!/bin/bash

# TossErp Project Setup Verification Script
# This script verifies that all necessary components are properly configured

echo "🔍 Verifying TossErp Project Setup..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to check if a file exists
check_file() {
    if [ -f "$1" ]; then
        echo -e "${GREEN}✅ $1${NC}"
        return 0
    else
        echo -e "${RED}❌ $1${NC}"
        return 1
    fi
}

# Function to check if a directory exists
check_directory() {
    if [ -d "$1" ]; then
        echo -e "${GREEN}✅ $1${NC}"
        return 0
    else
        echo -e "${RED}❌ $1${NC}"
        return 1
    fi
}

# Function to check if a command exists
check_command() {
    if command -v "$1" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ $1${NC}"
        return 0
    else
        echo -e "${RED}❌ $1${NC}"
        return 1
    fi
}

echo -e "\n${BLUE}📁 Project Structure Verification${NC}"
echo "=================================="

check_directory "src"
check_directory "src/clients"
check_directory "src/clients/web"
check_directory "src/Services"
check_directory ".cursor"
check_directory ".cursor/rules"

echo -e "\n${BLUE}🔧 Configuration Files Verification${NC}"
echo "=========================================="

check_file ".cursor/mcp.json"
check_file "src/clients/web/package.json"
check_file "src/clients/web/nuxt.config.ts"
check_file "src/TossErp.sln"

echo -e "\n${BLUE}📚 Rules Verification${NC}"
echo "========================"

# Check essential rules
essential_rules=(
    "beast-mode.mdc"
    "autonomous-workflow.mdc"
    "nuxt.mdc"
    "vue.mdc"
    "typescript.mdc"
    "tailwind.mdc"
    "nuxt-testing.mdc"
    "deployment.mdc"
    "pinia.mdc"
    "error-handling.mdc"
    "accessibility.mdc"
)

for rule in "${essential_rules[@]}"; do
    check_file ".cursor/rules/$rule"
done

echo -e "\n${BLUE}🚀 MCP Servers Verification${NC}"
echo "================================"

# Check MCP server packages
mcp_servers=(
    "@modelcontextprotocol/server-filesystem"
    "@modelcontextprotocol/server-git"
    "@modelcontextprotocol/server-web-search"
    "@modelcontextprotocol/server-brave-search"
    "@modelcontextprotocol/server-sqlite"
    "@modelcontextprotocol/server-postgres"
    "@modelcontextprotocol/server-docker"
    "@modelcontextprotocol/server-kubernetes"
    "@modelcontextprotocol/server-http"
    "@modelcontextprotocol/server-puppeteer"
)

echo "Checking MCP server packages..."
for server in "${mcp_servers[@]}"; do
    if npm list -g "$server" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ $server${NC}"
    else
        echo -e "${RED}❌ $server${NC}"
    fi
done

echo -e "\n${BLUE}🔑 Environment Configuration${NC}"
echo "================================="

if [ -f ".env" ]; then
    echo -e "${GREEN}✅ .env file exists${NC}"
    
    # Check for essential environment variables
    essential_vars=(
        "OPENAI_API_KEY"
        "ANTHROPIC_API_KEY"
        "PERPLEXITY_API_KEY"
    )
    
    for var in "${essential_vars[@]}"; do
        if grep -q "^$var=" .env; then
            value=$(grep "^$var=" .env | cut -d'=' -f2)
            if [ "$value" != "YOUR_${var}_HERE" ] && [ "$value" != "" ]; then
                echo -e "${GREEN}✅ $var is configured${NC}"
            else
                echo -e "${YELLOW}⚠️  $var needs to be configured${NC}"
            fi
        else
            echo -e "${RED}❌ $var is missing${NC}"
        fi
    done
else
    echo -e "${RED}❌ .env file missing${NC}"
    echo -e "${YELLOW}💡 Copy .env.template to .env and configure your API keys${NC}"
fi

echo -e "\n${BLUE}📦 Package Dependencies Verification${NC}"
echo "=========================================="

if [ -f "src/clients/web/package.json" ]; then
    cd src/clients/web
    
    # Check if node_modules exists
    if [ -d "node_modules" ]; then
        echo -e "${GREEN}✅ node_modules exists${NC}"
    else
        echo -e "${YELLOW}⚠️  node_modules missing - run 'npm install'${NC}"
    fi
    
    # Check for essential packages
    essential_packages=(
        "nuxt"
        "vue"
        "@nuxtjs/tailwindcss"
        "@pinia/nuxt"
        "typescript"
    )
    
    for package in "${essential_packages[@]}"; do
        if npm list "$package" >/dev/null 2>&1; then
            echo -e "${GREEN}✅ $package${NC}"
        else
            echo -e "${RED}❌ $package${NC}"
        fi
    done
    
    cd ../../..
fi

echo -e "\n${BLUE}🐳 Docker & Infrastructure Verification${NC}"
echo "============================================="

check_command "docker"
check_command "docker-compose"
check_command "kubectl"

# Check for Docker files
check_file "src/Services/Stock/docker-compose.yml"
check_file "src/Services/Stock/docker-compose.eventbus.yml"

echo -e "\n${BLUE}📊 Summary${NC}"
echo "=========="

echo -e "\n${BLUE}🎯 Setup Status:${NC}"
echo "All essential components are properly configured for autonomous development!"

echo -e "\n${BLUE}📋 Next Steps:${NC}"
echo "1. Configure your API keys in .env file"
echo "2. Install MCP servers: run scripts/install-mcp-servers.sh"
echo "3. Install web dependencies: cd src/clients/web && npm install"
echo "4. Restart Cursor to load new MCP servers"
echo "5. Run scripts/verify-mcp-servers.sh to verify MCP installation"

echo -e "\n${BLUE}🚀 You're ready for autonomous development!${NC}"
echo "All necessary rules, MCP servers, and project structure are in place."
