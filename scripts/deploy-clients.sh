#!/bin/bash

# Client Deployment Script
# Deploys all client applications with proper configuration

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
ENVIRONMENT=${1:-development}
GATEWAY_URL=${2:-http://localhost:8080}
BUILD_DIR="dist"
DEPLOY_DIR="deploy/clients"

echo -e "${BLUE}🚀 Deploying Client Applications${NC}"
echo "=========================================="
echo "Environment: $ENVIRONMENT"
echo "Gateway URL: $GATEWAY_URL"
echo ""

# Function to create environment file
create_env_file() {
    local client=$1
    local template_file="src/clients/$client/env.template"
    local env_file="src/clients/$client/.env"
    
    if [[ -f "$template_file" ]]; then
        echo -e "${BLUE}Creating environment file for $client...${NC}"
        
        # Copy template and replace placeholders
        cp "$template_file" "$env_file"
        
        # Update with actual values
        if [[ "$ENVIRONMENT" == "production" ]]; then
            # Production configuration
            sed -i "s|http://localhost:8080|$GATEWAY_URL|g" "$env_file"
            sed -i "s|NODE_ENV=development|NODE_ENV=production|g" "$env_file"
            sed -i "s|DEBUG=true|DEBUG=false|g" "$env_file"
        else
            # Development configuration
            sed -i "s|http://localhost:8080|$GATEWAY_URL|g" "$env_file"
        fi
        
        echo -e "  ${GREEN}✅ Environment file created: $env_file${NC}"
    else
        echo -e "  ${YELLOW}⚠️  Template file not found: $template_file${NC}"
    fi
}

# Function to deploy mobile client
deploy_mobile() {
    echo -e "${BLUE}📱 Deploying Mobile Client${NC}"
    
    cd src/clients/mobile
    
    # Check if Flutter is available
    if ! command -v flutter &> /dev/null; then
        echo -e "  ${RED}❌ Flutter not found. Please install Flutter first.${NC}"
        cd ../../..
        return 1
    fi
    
    # Create environment file
    create_env_file "mobile"
    
    # Clean and get dependencies
    echo "  Cleaning previous builds..."
    flutter clean
    
    echo "  Getting dependencies..."
    flutter pub get
    
    # Build for different platforms
    echo "  Building for web..."
    flutter build web --release
    
    echo "  Building for Android..."
    flutter build apk --release
    
    echo "  Building for iOS..."
    flutter build ios --release --no-codesign
    
    # Copy builds to deploy directory
    mkdir -p ../../../$DEPLOY_DIR/mobile
    cp -r build/web/* ../../../$DEPLOY_DIR/mobile/web/
    cp build/app/outputs/flutter-apk/app-release.apk ../../../$DEPLOY_DIR/mobile/android/
    
    echo -e "  ${GREEN}✅ Mobile client deployed${NC}"
    cd ../../..
}

# Function to deploy web client
deploy_web() {
    echo -e "${BLUE}🌐 Deploying Web Client${NC}"
    
    cd src/clients/web
    
    # Check if Node.js is available
    if ! command -v node &> /dev/null; then
        echo -e "  ${RED}❌ Node.js not found. Please install Node.js first.${NC}"
        cd ../../..
        return 1
    fi
    
    # Create environment file
    create_env_file "web"
    
    # Install dependencies
    echo "  Installing dependencies..."
    npm ci --only=production
    
    # Build for production
    echo "  Building for production..."
    npm run build
    
    # Copy build to deploy directory
    mkdir -p ../../../$DEPLOY_DIR/web
    cp -r .output/* ../../../$DEPLOY_DIR/web/
    
    echo -e "  ${GREEN}✅ Web client deployed${NC}"
    cd ../../..
}

# Function to deploy admin client
deploy_admin() {
    echo -e "${BLUE}⚙️  Deploying Admin Client${NC}"
    
    cd src/clients/admin
    
    # Check if Node.js is available
    if ! command -v node &> /dev/null; then
        echo -e "  ${RED}❌ Node.js not found. Please install Node.js first.${NC}"
        cd ../../..
        return 1
    fi
    
    # Create environment file
    create_env_file "admin"
    
    # Install dependencies
    echo "  Installing dependencies..."
    npm ci --only=production
    
    # Build for production
    echo "  Building for production..."
    npm run build
    
    # Copy build to deploy directory
    mkdir -p ../../../$DEPLOY_DIR/admin
    cp -r build/* ../../../$DEPLOY_DIR/admin/
    
    echo -e "  ${GREEN}✅ Admin client deployed${NC}"
    cd ../../..
}

# Function to create deployment configuration
create_deployment_config() {
    echo -e "${BLUE}📋 Creating Deployment Configuration${NC}"
    
    mkdir -p $DEPLOY_DIR
    
    # Create nginx configuration for web clients
    cat > $DEPLOY_DIR/nginx.conf << EOF
server {
    listen 80;
    server_name localhost;
    
    # Web Client
    location / {
        root /usr/share/nginx/html/web;
        try_files \$uri \$uri/ /index.html;
    }
    
    # Admin Client
    location /admin {
        alias /usr/share/nginx/html/admin;
        try_files \$uri \$uri/ /index.html;
    }
    
    # API Gateway proxy
    location /api {
        proxy_pass $GATEWAY_URL;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
    
    # Health check
    location /health {
        proxy_pass $GATEWAY_URL/health;
    }
}
EOF

    # Create Docker Compose for clients
    cat > $DEPLOY_DIR/docker-compose.yml << EOF
version: '3.8'

services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/conf.d/default.conf
      - ./web:/usr/share/nginx/html/web
      - ./admin:/usr/share/nginx/html/admin
    depends_on:
      - gateway
    networks:
      - toss-network

  gateway:
    image: tosserp/gateway:latest
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
    networks:
      - toss-network

networks:
  toss-network:
    external: true
EOF

    # Create deployment script
    cat > $DEPLOY_DIR/deploy.sh << 'EOF'
#!/bin/bash

# Deploy clients using Docker Compose
echo "🚀 Deploying clients..."

# Create network if it doesn't exist
docker network create toss-network 2>/dev/null || true

# Deploy services
docker-compose up -d

echo "✅ Clients deployed successfully!"
echo "🌐 Web Client: http://localhost"
echo "⚙️  Admin Client: http://localhost/admin"
echo "🔌 API Gateway: http://localhost:8080"
EOF

    chmod +x $DEPLOY_DIR/deploy.sh
    
    echo -e "  ${GREEN}✅ Deployment configuration created${NC}"
}

# Function to create health check script
create_health_check() {
    echo -e "${BLUE}🏥 Creating Health Check Script${NC}"
    
    cat > $DEPLOY_DIR/health-check.sh << 'EOF'
#!/bin/bash

# Health check for deployed clients

echo "🔍 Checking client health..."

# Check nginx
if curl -s http://localhost/health > /dev/null; then
    echo "✅ Nginx is running"
else
    echo "❌ Nginx is not responding"
fi

# Check web client
if curl -s http://localhost > /dev/null; then
    echo "✅ Web client is accessible"
else
    echo "❌ Web client is not accessible"
fi

# Check admin client
if curl -s http://localhost/admin > /dev/null; then
    echo "✅ Admin client is accessible"
else
    echo "❌ Admin client is not accessible"
fi

# Check API gateway
if curl -s http://localhost:8080/health > /dev/null; then
    echo "✅ API Gateway is healthy"
else
    echo "❌ API Gateway is not responding"
fi

echo "🏁 Health check complete!"
EOF

    chmod +x $DEPLOY_DIR/health-check.sh
    
    echo -e "  ${GREEN}✅ Health check script created${NC}"
}

# Main deployment process
main() {
    echo -e "${BLUE}Starting client deployment...${NC}"
    echo ""
    
    # Create deployment directory
    mkdir -p $DEPLOY_DIR
    
    # Deploy each client
    deploy_mobile
    deploy_web
    deploy_admin
    
    # Create deployment configuration
    create_deployment_config
    create_health_check
    
    echo ""
    echo -e "${GREEN}🎉 All clients deployed successfully!${NC}"
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo "1. Review deployment configuration in $DEPLOY_DIR"
    echo "2. Start the API Gateway and Stock API services"
    echo "3. Run: cd $DEPLOY_DIR && ./deploy.sh"
    echo "4. Check health: ./health-check.sh"
    echo ""
    echo -e "${BLUE}Deployment Summary:${NC}"
    echo "- Mobile: $DEPLOY_DIR/mobile/"
    echo "- Web: $DEPLOY_DIR/web/"
    echo "- Admin: $DEPLOY_DIR/admin/"
    echo "- Config: $DEPLOY_DIR/nginx.conf"
    echo "- Docker: $DEPLOY_DIR/docker-compose.yml"
}

# Run main function
main "$@"
