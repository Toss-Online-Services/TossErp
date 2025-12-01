#!/bin/bash

# TOSS ERP-III Frontend Deployment Script
# This script builds and prepares the Nuxt application for deployment

set -e

echo "🚀 Starting TOSS ERP-III Frontend Deployment..."

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if we're in the right directory
if [ ! -f "nuxt.config.ts" ]; then
  echo -e "${RED}Error: nuxt.config.ts not found. Please run this script from the toss-web-app directory.${NC}"
  exit 1
fi

# Step 1: Install dependencies
echo -e "${YELLOW}📦 Installing dependencies...${NC}"
pnpm install --frozen-lockfile

# Step 2: Run type checking
echo -e "${YELLOW}🔍 Running type check...${NC}"
pnpm run typecheck || {
  echo -e "${RED}Type check failed. Please fix errors before deploying.${NC}"
  exit 1
}

# Step 3: Run linter
echo -e "${YELLOW}🧹 Running linter...${NC}"
pnpm run lint || {
  echo -e "${YELLOW}Linter found issues. Continuing anyway...${NC}"
}

# Step 4: Run tests
echo -e "${YELLOW}🧪 Running tests...${NC}"
pnpm run test || {
  echo -e "${YELLOW}Some tests failed. Continuing anyway...${NC}"
}

# Step 5: Build for production
echo -e "${YELLOW}🏗️  Building for production...${NC}"
pnpm run build

# Step 6: Verify build output
if [ ! -d "dist/toss-web-app" ]; then
  echo -e "${RED}Error: Build output not found. Build may have failed.${NC}"
  exit 1
fi

echo -e "${GREEN}✅ Build completed successfully!${NC}"
echo -e "${GREEN}📁 Build output: dist/toss-web-app${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Review the build output in dist/toss-web-app"
echo "2. Deploy to your hosting platform (Vercel, Netlify, etc.)"
echo "3. Ensure environment variables are set:"
echo "   - NUXT_PUBLIC_API_BASE (backend API URL)"
echo "   - Other required environment variables"

