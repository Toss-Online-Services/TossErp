# TOSS ERP - Makefile for Cross-Platform Development
# This provides Unix/Linux/macOS compatibility alongside PowerShell scripts

.PHONY: help build clean restore test publish docker-up docker-down docker-dev docker-prod docker-infra docker-services docker-frontend docker-logs docker-clean

# Default target
help:
	@echo "🚀 TOSS ERP - Available Commands"
	@echo ""
	@echo "Backend Development:"
	@echo "  build     - Build the entire solution"
	@echo "  clean     - Clean the solution"
	@echo "  restore   - Restore NuGet packages"
	@echo "  test      - Run all tests"
	@echo "  publish   - Publish the solution"
	@echo ""
	@echo "Docker Management:"
	@echo "  docker-up      - Start all services"
	@echo "  docker-down    - Stop all services"
	@echo "  docker-dev     - Start development environment"
	@echo "  docker-prod    - Start production environment"
	@echo "  docker-infra   - Start infrastructure only"
	@echo "  docker-services- Start backend services only"
	@echo "  docker-frontend- Start frontend only"
	@echo "  docker-logs    - Show service logs"
	@echo "  docker-clean   - Clean Docker resources"
	@echo ""
	@echo "Quick Start:"
	@echo "  quick-start    - Complete development setup"
	@echo "  infra-only     - Start infrastructure for development"

# Backend Development
build:
	@echo "🔨 Building TOSS ERP solution..."
	dotnet build TossErp.sln -c Debug

clean:
	@echo "🧹 Cleaning TOSS ERP solution..."
	dotnet clean TossErp.sln

restore:
	@echo "📦 Restoring NuGet packages..."
	dotnet restore TossErp.sln

test:
	@echo "🧪 Running tests..."
	dotnet test TossErp.sln --no-build

publish:
	@echo "📤 Publishing TOSS ERP solution..."
	dotnet publish TossErp.sln -c Release -o ./publish

# Docker Management
docker-up:
	@echo "🐳 Starting all services..."
	docker-compose -f docker/docker-compose.yml up -d

docker-down:
	@echo "🛑 Stopping all services..."
	docker-compose -f docker/docker-compose.yml down

docker-dev:
	@echo "🚀 Starting development environment..."
	docker-compose -f docker/docker-compose.yml -f docker/docker-compose.dev.yml up -d

docker-prod:
	@echo "🏭 Starting production environment..."
	docker-compose -f docker/docker-compose.yml up -d

docker-infra:
	@echo "🏗️ Starting infrastructure services..."
	docker-compose -f docker/docker-compose.yml up -d postgres redis rabbitmq

docker-services:
	@echo "⚙️ Starting backend services..."
	docker-compose -f docker/docker-compose.yml up -d identity-api stock-api crm-api gateway

docker-frontend:
	@echo "🌐 Starting frontend services..."
	docker-compose -f docker/docker-compose.yml up -d web-client nginx

docker-logs:
	@echo "📋 Showing service logs..."
	docker-compose -f docker/docker-compose.yml logs -f

docker-clean:
	@echo "🧹 Cleaning Docker resources..."
	docker system prune -f
	docker volume prune -f
	docker network prune -f

# Quick Start Commands
quick-start: restore build test docker-infra docker-services docker-frontend
	@echo "🎉 TOSS ERP development environment is ready!"
	@echo "📱 Web Client: http://localhost:3000"
	@echo "🌐 API Gateway: http://localhost:8080"
	@echo "🗄️ PostgreSQL: localhost:5432"
	@echo "🔴 Redis: localhost:6379"
	@echo "🐰 RabbitMQ: http://localhost:15672"

infra-only: docker-infra
	@echo "🏗️ Infrastructure services are running!"
	@echo "🗄️ PostgreSQL: localhost:5432"
	@echo "🔴 Redis: localhost:6379"
	@echo "🐰 RabbitMQ: http://localhost:15672"

# Development Workflow
dev-setup: restore build docker-infra
	@echo "🔧 Development setup complete!"
	@echo "Run 'make docker-dev' to start full development environment"

# Production Deployment
prod-deploy: clean restore build test publish
	@echo "🚀 Production deployment ready!"
	@echo "Run 'make docker-prod' to start production environment"

# Monitoring
monitoring: docker-dev
	@echo "📊 Development monitoring started!"
	@echo "📈 Prometheus: http://localhost:9090"
	@echo "📊 Grafana: http://localhost:3001 (admin/admin123)"

# Utility Commands
status:
	@echo "📊 Checking service status..."
	docker-compose -f docker/docker-compose.yml ps

logs-all:
	@echo "📋 Showing all service logs..."
	docker-compose -f docker/docker-compose.yml logs --tail=100

restart:
	@echo "🔄 Restarting all services..."
	docker-compose -f docker/docker-compose.yml restart

# Frontend Development
frontend-dev:
	@echo "🌐 Starting frontend development..."
	cd src/clients/web && npm run dev

frontend-install:
	@echo "📦 Installing frontend dependencies..."
	cd src/clients/web && npm install

frontend-build:
	@echo "🔨 Building frontend..."
	cd src/clients/web && npm run build

# Database Management
db-reset:
	@echo "🗄️ Resetting database..."
	docker-compose -f docker/docker-compose.yml down -v
	docker-compose -f docker/docker-compose.yml up -d postgres
	@echo "⏳ Waiting for database to be ready..."
	@sleep 10
	@echo "✅ Database reset complete!"

# Health Checks
health:
	@echo "🏥 Checking service health..."
	@curl -f http://localhost:8080/health || echo "❌ Gateway not healthy"
	@curl -f http://localhost:5001/health || echo "❌ Identity not healthy"
	@curl -f http://localhost:5002/health || echo "❌ Stock not healthy"
	@curl -f http://localhost:5003/health || echo "❌ CRM not healthy"
	@curl -f http://localhost:3000 || echo "❌ Web client not healthy"
