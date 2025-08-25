#!/bin/bash

# TOSS ERP III - Development Environment Startup Script
# This script sets up and starts the complete development environment

set -e

echo "🚀 Starting TOSS ERP III Development Environment..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    echo "   Download from: https://www.docker.com/get-started"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Create .env file if it doesn't exist
if [ ! -f .env ]; then
    echo "📝 Creating .env file..."
    cat > .env << EOF
# TOSS ERP III Development Environment Variables

# OpenAI API Key (required for AI features)
OPENAI_API_KEY=your-openai-api-key-here

# JWT Secrets (auto-generated for development)
JWT_SECRET=$(openssl rand -base64 32)
REFRESH_TOKEN_SECRET=$(openssl rand -base64 32)

# Email Configuration (using MailHog for development)
EMAIL_FROM=noreply@toss-erp.com
EMAIL_SMTP_HOST=mailhog
EMAIL_SMTP_PORT=1025

# Database Configuration
POSTGRES_USER=postgres
POSTGRES_PASSWORD=password
POSTGRES_DB=toss_erp

# Redis Configuration
REDIS_URL=redis://redis:6379

# Node Environment
NODE_ENV=development
EOF
    echo "✅ Created .env file with default values"
    echo "⚠️  Please update OPENAI_API_KEY in .env file for AI features to work"
fi

# Create necessary directories
echo "📁 Creating necessary directories..."
mkdir -p logs
mkdir -p data/postgres
mkdir -p data/redis

# Pull latest images
echo "📦 Pulling Docker images..."
docker-compose -f docker-compose.dev.yml pull

# Build services
echo "🔨 Building services..."
docker-compose -f docker-compose.dev.yml build

# Start the environment
echo "🚀 Starting development environment..."
docker-compose -f docker-compose.dev.yml up -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 30

# Check service health
echo "🔍 Checking service health..."

services=(
    "postgres:5432"
    "redis:6379"
    "auth-service:3001"
    "api-gateway:3000"
    "frontend:3100"
)

for service in "${services[@]}"; do
    name=$(echo $service | cut -d: -f1)
    port=$(echo $service | cut -d: -f2)
    
    if curl -f -s http://localhost:$port/health > /dev/null 2>&1 || nc -z localhost $port 2>/dev/null; then
        echo "✅ $name is healthy"
    else
        echo "⚠️  $name might not be ready yet"
    fi
done

echo ""
echo "🎉 TOSS ERP III Development Environment is starting up!"
echo ""
echo "📍 Service URLs:"
echo "   🌐 Frontend (Nuxt):     http://localhost:3100"
echo "   🔌 API Gateway:         http://localhost:3000"
echo "   🔐 Auth Service:        http://localhost:3001"
echo "   📊 CRM Service:         http://localhost:3002"
echo "   📦 Inventory Service:   http://localhost:3003"
echo "   💰 Accounting Service:  http://localhost:3004"
echo "   💼 Sales Service:       http://localhost:3005"
echo "   🤖 AI Copilot Service:  http://localhost:3010"
echo ""
echo "🛠️  Management Tools:"
echo "   📧 MailHog (Email):     http://localhost:8025"
echo "   🗄️  Adminer (Database): http://localhost:8080"
echo ""
echo "📚 API Documentation:"
echo "   Auth API:               http://localhost:3001/docs"
echo "   CRM API:                http://localhost:3002/docs"
echo "   Inventory API:          http://localhost:3003/docs"
echo ""
echo "💡 Useful Commands:"
echo "   📋 View logs:           docker-compose -f docker-compose.dev.yml logs -f [service-name]"
echo "   🔄 Restart service:     docker-compose -f docker-compose.dev.yml restart [service-name]"
echo "   🛑 Stop all:            docker-compose -f docker-compose.dev.yml down"
echo "   🗑️  Clean up:           docker-compose -f docker-compose.dev.yml down -v"
echo ""
echo "⚠️  Notes:"
echo "   - Update OPENAI_API_KEY in .env for AI features"
echo "   - Services may take a few minutes to be fully ready"
echo "   - Check logs if any service fails to start"
echo ""
echo "Happy coding! 🚀"
