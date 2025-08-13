# Kubernetes Deployment

## Overview
This directory contains Kubernetes manifests and deployment configurations for the TossErp microservices architecture.

## Architecture
- **Namespace Isolation**: Separate namespaces for different environments
- **Service Mesh**: Istio for service-to-service communication
- **Ingress Controller**: NGINX Ingress for external traffic
- **Secrets Management**: Kubernetes secrets and external secret operators
- **Monitoring**: Prometheus and Grafana for observability
- **Logging**: ELK stack for centralized logging

## Structure
```
kubernetes/
├── base/                 # Base Kustomize configurations
│   ├── namespaces/       # Namespace definitions
│   ├── services/         # Service definitions
│   ├── deployments/      # Deployment configurations
│   ├── configmaps/       # Configuration maps
│   └── secrets/          # Secret templates
├── overlays/             # Environment-specific overlays
│   ├── development/      # Development environment
│   ├── staging/          # Staging environment
│   └── production/       # Production environment
├── monitoring/           # Monitoring stack
│   ├── prometheus/       # Prometheus configuration
│   ├── grafana/          # Grafana dashboards
│   └── alertmanager/     # Alerting rules
├── logging/              # Logging stack
│   ├── elasticsearch/    # Elasticsearch configuration
│   ├── kibana/           # Kibana configuration
│   └── filebeat/         # Log collection
└── README.md            # This file
```

## Services

### Core Services
- **web-gateway**: API Gateway service
- **stock-service**: Inventory management service
- **user-service**: User management service
- **order-service**: Order processing service
- **payment-service**: Payment processing service

### Infrastructure Services
- **postgresql**: Primary database
- **redis**: Caching layer
- **rabbitmq**: Message broker
- **elasticsearch**: Search and analytics
- **prometheus**: Metrics collection
- **grafana**: Monitoring dashboards

## Deployment

### Prerequisites
```bash
# Install kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"

# Install kustomize
curl -s "https://raw.githubusercontent.com/kubernetes-sigs/kustomize/master/hack/install_kustomize.sh" | bash

# Install helm
curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
```

### Development Environment
```bash
# Deploy to development
kubectl apply -k overlays/development

# Check deployment status
kubectl get pods -n tosserp-dev

# Access services
kubectl port-forward svc/web-gateway 8080:80 -n tosserp-dev
```

### Production Environment
```bash
# Deploy to production
kubectl apply -k overlays/production

# Check deployment status
kubectl get pods -n tosserp-prod

# Monitor deployment
kubectl rollout status deployment/web-gateway -n tosserp-prod
```

## Configuration

### Environment Variables
```yaml
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
  - name: DATABASE_CONNECTION_STRING
    valueFrom:
      secretKeyRef:
        name: database-secret
        key: connection-string
```

### Resource Limits
```yaml
resources:
  requests:
    memory: "256Mi"
    cpu: "250m"
  limits:
    memory: "512Mi"
    cpu: "500m"
```

### Health Checks
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 80
  initialDelaySeconds: 30
  periodSeconds: 10
readinessProbe:
  httpGet:
    path: /health/ready
    port: 80
  initialDelaySeconds: 5
  periodSeconds: 5
```

## Monitoring

### Prometheus Configuration
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: prometheus-config
data:
  prometheus.yml: |
    global:
      scrape_interval: 15s
    scrape_configs:
      - job_name: 'tosserp-services'
        kubernetes_sd_configs:
          - role: pod
```

### Grafana Dashboards
- **Service Overview**: High-level service metrics
- **Database Performance**: Database connection and query metrics
- **API Gateway**: Gateway performance and error rates
- **Business Metrics**: Business-specific KPIs

## Security

### Network Policies
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  - Egress
```

### RBAC Configuration
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  namespace: tosserp-prod
  name: service-role
rules:
- apiGroups: [""]
  resources: ["pods", "services"]
  verbs: ["get", "list", "watch"]
```

## Scaling

### Horizontal Pod Autoscaler
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: stock-service-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: stock-service
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

## Troubleshooting

### Common Issues
1. **Pod Startup Failures**: Check logs and health checks
2. **Service Communication**: Verify network policies
3. **Resource Constraints**: Monitor resource usage
4. **Configuration Issues**: Validate ConfigMaps and Secrets

### Debug Commands
```bash
# Check pod logs
kubectl logs -f deployment/stock-service -n tosserp-prod

# Describe pod status
kubectl describe pod <pod-name> -n tosserp-prod

# Check service endpoints
kubectl get endpoints -n tosserp-prod

# Check network policies
kubectl get networkpolicies -n tosserp-prod
```

## Best Practices
- **Resource Management**: Set appropriate resource limits
- **Security**: Use RBAC and network policies
- **Monitoring**: Implement comprehensive monitoring
- **Backup**: Regular backups of persistent data
- **Updates**: Rolling updates with zero downtime
- **Testing**: Test deployments in staging first
