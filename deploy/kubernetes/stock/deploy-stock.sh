#!/bin/bash

# Stock Module Kubernetes Deployment Script
# This script deploys the complete Stock module to Kubernetes

set -e

# Configuration
NAMESPACE="tosserp"
STOCK_DIR="infra/k8s/stock"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if kubectl is available
check_kubectl() {
    if ! command -v kubectl &> /dev/null; then
        print_error "kubectl is not installed or not in PATH"
        exit 1
    fi
}

# Function to check if namespace exists
check_namespace() {
    if ! kubectl get namespace $NAMESPACE &> /dev/null; then
        print_warning "Namespace $NAMESPACE does not exist. Creating..."
        kubectl create namespace $NAMESPACE
    fi
}

# Function to deploy secrets
deploy_secrets() {
    print_status "Deploying Stock module secrets..."
    kubectl apply -f $STOCK_DIR/stock-secrets.yaml -n $NAMESPACE
}

# Function to deploy database
deploy_database() {
    print_status "Deploying Stock database..."
    kubectl apply -f $STOCK_DIR/stock-database-deployment.yaml -n $NAMESPACE
    
    # Wait for database to be ready
    print_status "Waiting for database to be ready..."
    kubectl wait --for=condition=ready pod -l app=stock-postgres -n $NAMESPACE --timeout=300s
}

# Function to deploy monitoring
deploy_monitoring() {
    print_status "Deploying Stock monitoring configuration..."
    kubectl apply -f $STOCK_DIR/stock-monitoring.yaml -n $NAMESPACE
}

# Function to deploy Stock.Processor
deploy_processor() {
    print_status "Deploying Stock.Processor..."
    kubectl apply -f $STOCK_DIR/stock-processor-deployment.yaml -n $NAMESPACE
    
    # Wait for processor to be ready
    print_status "Waiting for Stock.Processor to be ready..."
    kubectl wait --for=condition=ready pod -l app=stock-processor -n $NAMESPACE --timeout=300s
}

# Function to deploy Stock.API
deploy_api() {
    print_status "Deploying Stock.API..."
    kubectl apply -f $STOCK_DIR/stock-api-deployment.yaml -n $NAMESPACE
    
    # Wait for API to be ready
    print_status "Waiting for Stock.API to be ready..."
    kubectl wait --for=condition=ready pod -l app=stock-api -n $NAMESPACE --timeout=300s
}

# Function to verify deployment
verify_deployment() {
    print_status "Verifying deployment..."
    
    # Check all pods are running
    kubectl get pods -n $NAMESPACE -l component=stock
    
    # Check services
    kubectl get services -n $NAMESPACE -l component=stock
    
    # Check ingress
    kubectl get ingress -n $NAMESPACE -l component=stock
    
    print_status "Deployment verification complete!"
}

# Function to show deployment status
show_status() {
    print_status "Current Stock module deployment status:"
    echo ""
    kubectl get all -n $NAMESPACE -l component=stock
    echo ""
    kubectl get ingress -n $NAMESPACE -l component=stock
}

# Main deployment function
main() {
    print_status "Starting Stock module deployment..."
    
    check_kubectl
    check_namespace
    
    deploy_secrets
    deploy_database
    deploy_monitoring
    deploy_processor
    deploy_api
    
    verify_deployment
    show_status
    
    print_status "Stock module deployment completed successfully!"
}

# Handle command line arguments
case "${1:-deploy}" in
    "deploy")
        main
        ;;
    "status")
        show_status
        ;;
    "delete")
        print_status "Deleting Stock module..."
        kubectl delete -f $STOCK_DIR/ -n $NAMESPACE
        print_status "Stock module deleted!"
        ;;
    "restart")
        print_status "Restarting Stock module..."
        kubectl rollout restart deployment/stock-api -n $NAMESPACE
        kubectl rollout restart deployment/stock-processor -n $NAMESPACE
        print_status "Stock module restarted!"
        ;;
    *)
        echo "Usage: $0 {deploy|status|delete|restart}"
        echo "  deploy   - Deploy the Stock module (default)"
        echo "  status   - Show deployment status"
        echo "  delete   - Delete the Stock module"
        echo "  restart  - Restart the Stock module"
        exit 1
        ;;
esac
