# Stock Module Deployment Guide

This guide provides step-by-step instructions for deploying the Stock module to various environments.

## 🎯 Deployment Overview

The Stock module can be deployed using multiple strategies:

1. **Local Development**: Docker Compose for development and testing
2. **Production Docker Compose**: Containerized deployment for small to medium scale
3. **Kubernetes**: Production-grade orchestration for large scale deployments
4. **CI/CD Pipeline**: Automated deployment with GitHub Actions

## 🚀 Local Development Deployment

### Prerequisites

- Docker Desktop
- Docker Compose
- .NET 8 SDK (for local development)

### Quick Start

1. **Navigate to the Stock module directory:**
   ```bash
   cd src/Services/Stock
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **Verify services are running:**
   ```bash
   docker-compose ps
   ```

4. **Access services:**
   - **Stock API**: http://localhost:5001
   - **Stock Processor**: http://localhost:5002
   - **PostgreSQL**: localhost:5432
   - **RabbitMQ Management**: http://localhost:15672
   - **Redis**: localhost:6379
   - **Prometheus**: http://localhost:9090
   - **Grafana**: http://localhost:3000

### Development Workflow

1. **Run the API locally:**
   ```bash
   cd Stock.API
   dotnet run
   ```

2. **Run the Processor locally:**
   ```bash
   cd Stock.Processor
   dotnet run
   ```

3. **Run tests:**
   ```bash
   dotnet test
   ```

## 🐳 Production Docker Compose Deployment

### Environment Configuration

1. **Create production environment file:**
   ```bash
   cp .env.prod.example .env.prod
   ```

2. **Edit environment variables:**
   ```bash
   # Database
   POSTGRES_PASSWORD=your_secure_password_here
   
   # Event Bus
   RABBITMQ_USER=stock_user
   RABBITMQ_PASSWORD=your_secure_rabbitmq_password
   
   # Redis
   REDIS_PASSWORD=your_secure_redis_password
   
   # OpenAI
   OPENAI_API_KEY=your_openai_api_key_here
   
   # Grafana
   GRAFANA_USER=admin
   GRAFANA_PASSWORD=your_secure_grafana_password
   ```

### Deployment Steps

1. **Build and deploy:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --build
   ```

2. **Verify deployment:**
   ```bash
   docker-compose -f docker-compose.prod.yml ps
   ```

3. **Check logs:**
   ```bash
   docker-compose -f docker-compose.prod.yml logs -f stock-api
   ```

### Scaling

1. **Scale API instances:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --scale stock-api=3
   ```

2. **Scale Processor instances:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --scale stock-processor=2
   ```

## ☸️ Kubernetes Deployment

### Prerequisites

- Kubernetes cluster (1.24+)
- kubectl configured
- Helm 3.x (optional)
- cert-manager (for SSL certificates)
- nginx-ingress controller

### Cluster Setup

1. **Create namespace:**
   ```bash
   kubectl create namespace tosserp
   ```

2. **Set context:**
   ```bash
   kubectl config set-context --current --namespace=tosserp
   ```

### Deployment Steps

1. **Navigate to deployment directory:**
   ```bash
   cd deploy/kubernetes/stock
   ```

2. **Deploy the complete module:**
   ```bash
   ./deploy-stock.sh deploy
   ```

3. **Check deployment status:**
   ```bash
   ./deploy-stock.sh status
   ```

### Manual Deployment

If you prefer manual deployment:

1. **Deploy secrets:**
   ```bash
   kubectl apply -f stock-secrets.yaml
   ```

2. **Deploy database:**
   ```bash
   kubectl apply -f stock-database-deployment.yaml
   ```

3. **Deploy monitoring:**
   ```bash
   kubectl apply -f stock-monitoring.yaml
   ```

4. **Deploy processor:**
   ```bash
   kubectl apply -f stock-processor-deployment.yaml
   ```

5. **Deploy API:**
   ```bash
   kubectl apply -f stock-api-deployment.yaml
   ```

### Verification

1. **Check all resources:**
   ```bash
   kubectl get all -n tosserp -l component=stock
   ```

2. **Check ingress:**
   ```bash
   kubectl get ingress -n tosserp -l component=stock
   ```

3. **Test health endpoints:**
   ```bash
   # Port forward to access services
   kubectl port-forward -n tosserp svc/stock-api-service 8080:80
   
   # Test health check
   curl http://localhost:8080/health
   ```

## 🔄 CI/CD Pipeline Deployment

### GitHub Actions Setup

1. **Repository secrets required:**
   - `KUBECONFIG`: Base64 encoded kubeconfig
   - `DOCKER_USERNAME`: Docker registry username
   - `DOCKER_PASSWORD`: Docker registry password
   - `OPENAI_API_KEY`: OpenAI API key

2. **Workflow triggers:**
   - Push to `main` branch: Deploy to staging
   - Push to `production` branch: Deploy to production
   - Pull request: Run tests and security scans

### Pipeline Stages

1. **Build & Test:**
   ```yaml
   - name: Build and Test
     run: |
       dotnet build --no-restore
       dotnet test --no-build --verbosity normal
   ```

2. **Security Scan:**
   ```yaml
   - name: Security Scan
     run: |
       trivy fs --exit-code 1 --severity HIGH,CRITICAL .
   ```

3. **Build Docker Images:**
   ```yaml
   - name: Build and Push Images
     run: |
       docker build -t ${{ secrets.DOCKER_USERNAME }}/stock-api:${{ github.sha }} .
       docker push ${{ secrets.DOCKER_USERNAME }}/stock-api:${{ github.sha }}
   ```

4. **Deploy to Kubernetes:**
   ```yaml
   - name: Deploy to Kubernetes
     run: |
       echo "${{ secrets.KUBECONFIG }}" | base64 -d > kubeconfig
       export KUBECONFIG=kubeconfig
       ./deploy-stock.sh deploy
   ```

## 🔧 Configuration Management

### Environment-Specific Configs

1. **Development:**
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft": "Information"
       }
     },
     "Stock": {
       "Processor": {
         "LowStockAlert": {
           "CheckIntervalMinutes": 5
         }
       }
     }
   }
   ```

2. **Production:**
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft": "Warning"
       }
     },
     "Stock": {
       "Processor": {
         "LowStockAlert": {
           "CheckIntervalMinutes": 15
         }
       }
     }
   }
   ```

### Secrets Management

1. **Kubernetes Secrets:**
   ```bash
   # Create secret from file
   kubectl create secret generic stock-db-secret \
     --from-file=username=./db-username.txt \
     --from-file=password=./db-password.txt
   
   # Create secret from literal
   kubectl create secret generic openai-secret \
     --from-literal=api-key=your-api-key-here
   ```

2. **Docker Compose Secrets:**
   ```yaml
   secrets:
     db_password:
       file: ./secrets/db_password.txt
     openai_key:
       file: ./secrets/openai_key.txt
   ```

## 📊 Monitoring & Observability

### Health Checks

1. **API Health Endpoints:**
   - `/health/live`: Liveness probe
   - `/health/ready`: Readiness probe
   - `/health`: Comprehensive health status

2. **Database Health:**
   ```bash
   kubectl exec -n tosserp -it stock-postgres -- pg_isready -U postgres
   ```

3. **Event Bus Health:**
   ```bash
   kubectl exec -n tosserp -it stock-rabbitmq -- rabbitmq-diagnostics status
   ```

### Metrics & Logging

1. **Prometheus Metrics:**
   - Endpoint: `/metrics`
   - Scrape interval: 15s
   - Key metrics: stock operations, response times, error rates

2. **Structured Logging:**
   - Format: JSON with correlation IDs
   - Output: Console and file
   - Log levels: Debug, Information, Warning, Error

3. **Distributed Tracing:**
   - Correlation IDs propagated across services
   - Request tracing for debugging
   - Performance monitoring

## 🔒 Security Configuration

### Authentication & Authorization

1. **JWT Configuration:**
   ```json
   {
     "JwtSettings": {
       "SecretKey": "your-secret-key-here",
       "Issuer": "tosserp",
       "Audience": "stock-api",
       "ExpirationMinutes": 60
     }
   }
   ```

2. **Role-Based Access Control:**
   ```csharp
   [Authorize(Roles = "StockManager")]
   [HttpPost("api/stock-operations/issue")]
   public async Task<IActionResult> IssueStock(IssueStockCommand command)
   ```

### Network Security

1. **Ingress Security:**
   ```yaml
   annotations:
     nginx.ingress.kubernetes.io/ssl-redirect: "true"
     nginx.ingress.kubernetes.io/force-ssl-redirect: "true"
     cert-manager.io/cluster-issuer: "letsencrypt-prod"
   ```

2. **Pod Security:**
   ```yaml
   securityContext:
     runAsNonRoot: true
     runAsUser: 1000
     capabilities:
       drop:
       - ALL
   ```

## 🚨 Troubleshooting

### Common Issues

1. **Database Connection Failures:**
   ```bash
   # Check connection string
   kubectl get secret stock-db-secret -o yaml
   
   # Test connectivity
   kubectl exec -n tosserp -it stock-postgres -- pg_isready -U postgres
   ```

2. **Event Bus Issues:**
   ```bash
   # Check RabbitMQ status
   kubectl exec -n tosserp -it stock-rabbitmq -- rabbitmq-diagnostics status
   
   # Check queues
   kubectl exec -n tosserp -it stock-rabbitmq -- rabbitmqctl list_queues
   ```

3. **API Health Issues:**
   ```bash
   # Check pod status
   kubectl get pods -n tosserp -l app=stock-api
   
   # Check logs
   kubectl logs -n tosserp -l app=stock-api --tail=100
   ```

### Rollback Procedures

1. **Kubernetes Rollback:**
   ```bash
   # Rollback to previous deployment
   kubectl rollout undo deployment/stock-api -n tosserp
   
   # Check rollback status
   kubectl rollout status deployment/stock-api -n tosserp
   ```

2. **Docker Compose Rollback:**
   ```bash
   # Stop current deployment
   docker-compose -f docker-compose.prod.yml down
   
   # Start previous version
   docker-compose -f docker-compose.prod.yml up -d
   ```

## 📈 Performance Tuning

### Resource Optimization

1. **Memory Settings:**
   ```yaml
   resources:
     requests:
       memory: "256Mi"
       cpu: "250m"
     limits:
       memory: "512Mi"
       cpu: "500m"
   ```

2. **Database Tuning:**
   ```sql
   -- Optimize PostgreSQL settings
   ALTER SYSTEM SET shared_buffers = '256MB';
   ALTER SYSTEM SET effective_cache_size = '1GB';
   ALTER SYSTEM SET work_mem = '4MB';
   ```

3. **Caching Strategy:**
   - Redis for frequently accessed data
   - In-memory caching for application data
   - Cache invalidation on data changes

### Scaling Strategies

1. **Horizontal Pod Autoscaling:**
   ```yaml
   apiVersion: autoscaling/v2
   kind: HorizontalPodAutoscaler
   metadata:
     name: stock-api-hpa
   spec:
     scaleTargetRef:
       apiVersion: apps/v1
       kind: Deployment
       name: stock-api
     minReplicas: 3
     maxReplicas: 10
     metrics:
     - type: Resource
       resource:
         name: cpu
         target:
           type: Utilization
           averageUtilization: 70
   ```

2. **Database Scaling:**
   - Read replicas for read-heavy workloads
   - Connection pooling optimization
   - Query performance monitoring

## 📚 Additional Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [PostgreSQL Tuning](https://www.postgresql.org/docs/current/runtime-config-resource.html)
- [Prometheus Monitoring](https://prometheus.io/docs/introduction/overview/)
