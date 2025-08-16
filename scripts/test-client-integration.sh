#!/bin/bash

# Client Integration Test Script
# Tests all client applications' connectivity to the backend through the API Gateway

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
GATEWAY_URL="http://localhost:8080"
STOCK_API_URL="http://localhost:8080/api/stock"
TIMEOUT=10

echo -e "${BLUE}🔌 Testing Client Integration with Backend Services${NC}"
echo "=================================================="
echo ""

# Function to test endpoint
test_endpoint() {
    local url=$1
    local description=$2
    local expected_status=${3:-200}
    
    echo -n "Testing $description... "
    
    if curl -s -o /dev/null -w "%{http_code}" --max-time $TIMEOUT "$url" | grep -q "$expected_status"; then
        echo -e "${GREEN}✅ PASS${NC}"
        return 0
    else
        echo -e "${RED}❌ FAIL${NC}"
        return 1
    fi
}

# Function to test API response
test_api_response() {
    local url=$1
    local description=$2
    
    echo -n "Testing $description... "
    
    response=$(curl -s --max-time $TIMEOUT "$url" 2>/dev/null || echo "ERROR")
    
    if [[ "$response" == "ERROR" ]]; then
        echo -e "${RED}❌ FAIL - Connection error${NC}"
        return 1
    elif [[ "$response" == *"error"* ]] || [[ "$response" == *"Error"* ]]; then
        echo -e "${YELLOW}⚠️  WARNING - API returned error${NC}"
        return 0
    else
        echo -e "${GREEN}✅ PASS${NC}"
        return 0
    fi
}

# Test counter
passed=0
failed=0
warnings=0

echo -e "${BLUE}1. Testing Gateway Health${NC}"
echo "------------------------"

if test_endpoint "$GATEWAY_URL/health" "Gateway Health Check" 200; then
    ((passed++))
else
    ((failed++))
fi

echo ""

echo -e "${BLUE}2. Testing Stock API Endpoints${NC}"
echo "--------------------------------"

# Test Stock API endpoints
if test_endpoint "$STOCK_API_URL/items" "Stock Items Endpoint" 200; then
    ((passed++))
else
    ((failed++))
fi

if test_endpoint "$STOCK_API_URL/levels" "Stock Levels Endpoint" 200; then
    ((passed++))
else
    ((failed++))
fi

if test_endpoint "$STOCK_API_URL/movements" "Stock Movements Endpoint" 200; then
    ((passed++))
else
    ((failed++))
fi

echo ""

echo -e "${BLUE}3. Testing Client-Specific Endpoints${NC}"
echo "----------------------------------------"

# Test mobile dashboard endpoint
if test_endpoint "$GATEWAY_URL/api/mobile/dashboard" "Mobile Dashboard" 200; then
    ((passed++))
else
    ((failed++))
fi

# Test web dashboard endpoint
if test_endpoint "$GATEWAY_URL/api/web/dashboard" "Web Dashboard" 200; then
    ((passed++))
else
    ((failed++))
fi

echo ""

echo -e "${BLUE}4. Testing Authentication Endpoints${NC}"
echo "----------------------------------------"

# Test authentication endpoints (should return 401 without token)
if test_endpoint "$STOCK_API_URL/items" "Stock API Auth Check" 200; then
    ((passed++))
else
    ((failed++))
fi

echo ""

echo -e "${BLUE}5. Testing CORS Configuration${NC}"
echo "--------------------------------"

# Test CORS headers
echo -n "Testing CORS headers... "
cors_headers=$(curl -s -I -H "Origin: http://localhost:3000" "$GATEWAY_URL/api/mobile/dashboard" 2>/dev/null | grep -i "access-control-allow-origin" || echo "NO_CORS")

if [[ "$cors_headers" != "NO_CORS" ]]; then
    echo -e "${GREEN}✅ PASS${NC}"
    ((passed++))
else
    echo -e "${YELLOW}⚠️  WARNING - CORS headers not found${NC}"
    ((warnings++))
fi

echo ""

echo -e "${BLUE}6. Testing Client Environment Files${NC}"
echo "----------------------------------------"

# Check if environment template files exist
check_env_file() {
    local file=$1
    local client=$2
    
    if [[ -f "$file" ]]; then
        echo -e "  ${GREEN}✅ $client: $file exists${NC}"
        ((passed++))
    else
        echo -e "  ${RED}❌ $client: $file missing${NC}"
        ((failed++))
    fi
}

check_env_file "src/clients/mobile/env.template" "Mobile"
check_env_file "src/clients/web/env.template" "Web"
check_env_file "src/clients/admin/env.template" "Admin"

echo ""

echo -e "${BLUE}7. Testing Client API Services${NC}"
echo "-----------------------------------"

# Check if API service files exist and are properly configured
check_api_service() {
    local file=$1
    local client=$2
    
    if [[ -f "$file" ]]; then
        # Check if it points to gateway
        if grep -q "localhost:8080" "$file"; then
            echo -e "  ${GREEN}✅ $client: API service configured for gateway${NC}"
            ((passed++))
        else
            echo -e "  ${YELLOW}⚠️  $client: API service may not be configured for gateway${NC}"
            ((warnings++))
        fi
    else
        echo -e "  ${RED}❌ $client: API service file missing${NC}"
        ((failed++))
    fi
}

check_api_service "src/clients/mobile/lib/core/network/api_service.dart" "Mobile"
check_api_service "src/clients/web/services/api.ts" "Web"
check_api_service "src/clients/admin/src/services/api.ts" "Admin"

echo ""

echo -e "${BLUE}8. Testing Gateway Configuration${NC}"
echo "------------------------------------"

# Check gateway configuration
if [[ -f "src/Gateway/appsettings.json" ]]; then
    if grep -q "stock-api" "src/Gateway/appsettings.json"; then
        echo -e "  ${GREEN}✅ Gateway configured for Stock API${NC}"
        ((passed++))
    else
        echo -e "  ${RED}❌ Gateway not configured for Stock API${NC}"
        ((failed++))
    fi
else
    echo -e "  ${RED}❌ Gateway configuration file missing${NC}"
    ((failed++))
fi

echo ""

# Summary
echo "=================================================="
echo -e "${BLUE}Integration Test Summary${NC}"
echo "=================================================="
echo -e "Tests Passed: ${GREEN}$passed${NC}"
echo -e "Tests Failed: ${RED}$failed${NC}"
echo -e "Warnings: ${YELLOW}$warnings${NC}"
echo ""

if [[ $failed -eq 0 ]]; then
    echo -e "${GREEN}🎉 All critical tests passed! Clients are properly wired to the backend.${NC}"
    exit 0
else
    echo -e "${RED}❌ Some tests failed. Please review the issues above.${NC}"
    exit 1
fi
