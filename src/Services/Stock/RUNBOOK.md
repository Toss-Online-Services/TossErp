# Stock Module Runbook

This runbook provides operational procedures, troubleshooting guides, and emergency procedures for the Stock module.

## 🚨 Emergency Procedures

### Critical System Failure

#### Stock API Down
1. **Immediate Actions:**
   ```bash
   # Check API health
   curl -f http://stock-api:80/health/live
   
   # Check pod status
   kubectl get pods -n tosserp -l app=stock-api
   
   # Check logs for errors
   kubectl logs -n tosserp -l app=stock-api --tail=100
   ```

2. **Restart Procedure:**
   ```bash
   # Restart API deployment
   kubectl rollout restart deployment/stock-api -n tosserp
   
   # Wait for rollout
   kubectl rollout status deployment/stock-api -n tosserp
   ```

3. **Fallback:**
   - Switch to backup API instance if available
   - Enable maintenance mode for stock operations
   - Notify business users of temporary unavailability

#### Database Connection Issues
1. **Check Database Status:**
   ```bash
   # Check PostgreSQL pod
   kubectl get pods -n tosserp -l app=stock-postgres
   
   # Check database connectivity
   kubectl exec -n tosserp -it stock-postgres -- pg_isready -U postgres
   ```

2. **Database Recovery:**
   ```bash
   # Restart database if needed
   kubectl rollout restart statefulset/stock-postgres -n tosserp
   
   # Check connection pool
   kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "SELECT * FROM pg_stat_activity;"
   ```

#### Event Bus Failure
1. **Check RabbitMQ Status:**
   ```bash
   # Check RabbitMQ pod
   kubectl get pods -n tosserp -l app=stock-rabbitmq
   
   # Check queue status
   kubectl exec -n tosserp -it stock-rabbitmq -- rabbitmqctl list_queues
   ```

2. **Recovery Actions:**
   ```bash
   # Restart RabbitMQ if needed
   kubectl rollout restart deployment/stock-rabbitmq -n tosserp
   
   # Clear stuck messages
   kubectl exec -n tosserp -it stock-rabbitmq -- rabbitmqctl purge_queue stock_operations
   ```

## 🔧 Routine Maintenance

### Daily Checks

#### Health Status
```bash
# Check all stock components
kubectl get pods -n tosserp -l component=stock

# Check API health
curl -f http://stock-api:80/health

# Check processor status
curl -f http://stock-processor:80/health
```

#### Performance Metrics
```bash
# Check API response times
curl -w "@curl-format.txt" -o /dev/null -s "http://stock-api:80/api/items"

# Check database performance
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    schemaname,
    tablename,
    attname,
    n_distinct,
    correlation
FROM pg_stats 
WHERE schemaname = 'public' 
ORDER BY n_distinct DESC;"
```

### Weekly Tasks

#### Database Maintenance
```bash
# Run VACUUM and ANALYZE
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
VACUUM ANALYZE;
REINDEX DATABASE stock_db;"

# Check table sizes
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;"
```

#### Log Analysis
```bash
# Check for errors in last week
kubectl logs -n tosserp -l app=stock-api --since=168h | grep -i error | wc -l

# Check for warnings
kubectl logs -n tosserp -l app=stock-processor --since=168h | grep -i warning | wc -l
```

### Monthly Tasks

#### Security Review
```bash
# Check for security updates
docker images | grep stock

# Review access logs
kubectl logs -n tosserp -l app=stock-api --since=720h | grep -i "unauthorized\|forbidden"

# Check secret rotation
kubectl get secrets -n tosserp -l component=stock
```

#### Performance Review
```bash
# Generate performance report
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    query,
    calls,
    total_time,
    mean_time,
    rows
FROM pg_stat_statements 
ORDER BY total_time DESC 
LIMIT 20;"
```

## 🚨 Incident Response

### Low Stock Alert System Failure

#### Symptoms
- No low stock alerts generated
- Stock levels below threshold without notifications
- Processor logs show errors

#### Investigation Steps
1. **Check Processor Status:**
   ```bash
   kubectl get pods -n tosserp -l app=stock-processor
   kubectl logs -n tosserp -l app=stock-processor --tail=100
   ```

2. **Check Configuration:**
   ```bash
   kubectl get configmap -n tosserp stock-processor-config -o yaml
   ```

3. **Verify Business Logic:**
   ```bash
   # Check if items exist below reorder level
   kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
   SELECT 
       i.item_name,
       sl.quantity,
       i.reorder_level
   FROM stock_levels sl
   JOIN items i ON sl.item_id = i.id
   WHERE sl.quantity <= i.reorder_level;"
   ```

#### Resolution
1. **Restart Processor:**
   ```bash
   kubectl rollout restart deployment/stock-processor -n tosserp
   ```

2. **Check Alert Generation:**
   ```bash
   # Monitor for new alerts
   kubectl logs -n tosserp -l app=stock-processor -f
   ```

### Stock Reconciliation Issues

#### Symptoms
- Stock levels don't match expected values
- Discrepancies between physical and system inventory
- Reconciliation reports show errors

#### Investigation Steps
1. **Check Reconciliation Jobs:**
   ```bash
   kubectl logs -n tosserp -l app=stock-processor | grep -i reconciliation
   ```

2. **Verify Stock Calculations:**
   ```bash
   kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
   SELECT 
       i.item_name,
       sl.quantity as system_quantity,
       COALESCE(SUM(se.quantity), 0) as calculated_quantity
   FROM stock_levels sl
   JOIN items i ON sl.item_id = i.id
   LEFT JOIN stock_entries se ON i.id = se.item_id
   GROUP BY i.id, i.item_name, sl.quantity
   HAVING sl.quantity != COALESCE(SUM(se.quantity), 0);"
   ```

#### Resolution
1. **Run Manual Reconciliation:**
   ```bash
   # Trigger reconciliation manually
   curl -X POST "http://stock-processor:80/api/reconciliation/run" \
     -H "Content-Type: application/json"
   ```

2. **Update Stock Levels:**
   ```bash
   # If discrepancies found, update system
   kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
   UPDATE stock_levels 
   SET quantity = calculated_quantity 
   FROM (
       SELECT 
           item_id,
           SUM(quantity) as calculated_quantity
       FROM stock_entries 
       GROUP BY item_id
   ) calc
   WHERE stock_levels.item_id = calc.item_id;"
   ```

## 📊 Monitoring & Alerting

### Key Metrics to Monitor

#### API Performance
- Response time (p95 < 200ms)
- Error rate (< 1%)
- Throughput (requests per second)

#### Database Performance
- Connection pool utilization (< 80%)
- Query execution time (p95 < 100ms)
- Lock wait time (< 50ms)

#### Background Processing
- Stock processing queue length (< 1000)
- Processing latency (< 5 minutes)
- Failed job rate (< 5%)

### Alerting Rules

#### Critical Alerts
- Stock API down for > 1 minute
- Database connection failures > 5 per minute
- Event bus queue length > 5000

#### Warning Alerts
- API response time > 500ms
- Database connection pool > 90% utilization
- Low stock items > 100

#### Info Alerts
- New stock items created
- Stock levels below threshold
- Reconciliation completed

## 🔄 Backup & Recovery

### Database Backup

#### Automated Backups
```bash
# Check backup schedule
kubectl get cronjob -n tosserp -l component=stock

# Manual backup
kubectl exec -n tosserp -it stock-postgres -- pg_dump -U postgres stock_db > stock_backup_$(date +%Y%m%d_%H%M%S).sql
```

#### Backup Verification
```bash
# Verify backup integrity
pg_restore --list stock_backup_20241201_120000.sql

# Test restore to temporary database
kubectl exec -n tosserp -it stock-postgres -- createdb -U postgres test_restore
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d test_restore < stock_backup_20241201_120000.sql
```

### Recovery Procedures

#### Point-in-Time Recovery
```bash
# Stop applications
kubectl scale deployment stock-api --replicas=0 -n tosserp
kubectl scale deployment stock-processor --replicas=0 -n tosserp

# Restore database
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "DROP SCHEMA public CASCADE;"
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db < stock_backup_20241201_120000.sql

# Restart applications
kubectl scale deployment stock-api --replicas=3 -n tosserp
kubectl scale deployment stock-processor --replicas=2 -n tosserp
```

#### Data Corruption Recovery
```bash
# Identify corrupted records
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    schemaname,
    tablename,
    attname,
    n_distinct,
    correlation
FROM pg_stats 
WHERE schemaname = 'public' 
AND n_distinct < 0;"

# Repair corrupted data
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
UPDATE stock_levels 
SET quantity = 0 
WHERE quantity < 0;"
```

## 🚀 Performance Optimization

### Database Optimization

#### Index Management
```bash
# Check missing indexes
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    schemaname,
    tablename,
    attname,
    n_distinct,
    correlation
FROM pg_stats 
WHERE schemaname = 'public' 
AND correlation < 0.1;"

# Create missing indexes
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
CREATE INDEX CONCURRENTLY idx_stock_levels_item_warehouse 
ON stock_levels(item_id, warehouse_id);"
```

#### Query Optimization
```bash
# Analyze slow queries
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    query,
    calls,
    total_time,
    mean_time,
    rows
FROM pg_stat_statements 
WHERE mean_time > 100 
ORDER BY mean_time DESC;"
```

### Application Optimization

#### Caching Strategy
```bash
# Check Redis cache hit rate
kubectl exec -n tosserp -it stock-redis -- redis-cli info stats | grep keyspace

# Monitor cache performance
kubectl exec -n tosserp -it stock-redis -- redis-cli monitor
```

#### Connection Pooling
```bash
# Check connection pool status
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT 
    datname,
    numbackends,
    max_connections
FROM pg_stat_database;"
```

## 🔒 Security Procedures

### Access Control

#### User Management
```bash
# List current users
kubectl get serviceaccount -n tosserp -l component=stock

# Check user permissions
kubectl auth can-i get pods --as=system:serviceaccount:tosserp:stock-api -n tosserp
```

#### Secret Rotation
```bash
# Update database password
kubectl patch secret stock-db-secret -n tosserp -p '{"data":{"password":"'$(echo -n "newpassword" | base64)'"}}'

# Update API keys
kubectl patch secret openai-secret -n tosserp -p '{"data":{"api-key":"'$(echo -n "new-api-key" | base64)'"}}'
```

### Security Monitoring

#### Audit Logs
```bash
# Check authentication logs
kubectl logs -n tosserp -l app=stock-api | grep -i "authentication\|authorization"

# Check access patterns
kubectl logs -n tosserp -l app=stock-api | grep -i "GET\|POST\|PUT\|DELETE"
```

#### Vulnerability Scanning
```bash
# Scan container images
trivy image tosserp/stock-api:latest

# Check for known vulnerabilities
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db -c "
SELECT version();"
```

## 📞 Escalation Procedures

### On-Call Rotation

#### Primary On-Call
- **Name**: [Primary Engineer Name]
- **Phone**: [Phone Number]
- **Email**: [Email Address]
- **Escalation Time**: 15 minutes

#### Secondary On-Call
- **Name**: [Secondary Engineer Name]
- **Phone**: [Phone Number]
- **Email**: [Email Address]
- **Escalation Time**: 30 minutes

#### Team Lead
- **Name**: [Team Lead Name]
- **Phone**: [Phone Number]
- **Email**: [Email Address]
- **Escalation Time**: 1 hour

### Escalation Matrix

#### Level 1 (0-15 minutes)
- Primary on-call engineer
- Initial incident assessment
- Basic troubleshooting

#### Level 2 (15-30 minutes)
- Secondary on-call engineer
- Advanced troubleshooting
- Team collaboration

#### Level 3 (30-60 minutes)
- Team lead involvement
- Architecture review
- Business impact assessment

#### Level 4 (60+ minutes)
- Management notification
- External vendor support
- Business continuity planning

## 📋 Checklists

### Pre-Deployment Checklist
- [ ] All tests passing
- [ ] Security scan completed
- [ ] Performance benchmarks met
- [ ] Documentation updated
- [ ] Rollback plan prepared
- [ ] Team notified of deployment

### Post-Deployment Checklist
- [ ] Health checks passing
- [ ] Performance metrics normal
- [ ] Error rates acceptable
- [ ] User acceptance testing completed
- [ ] Monitoring alerts configured
- [ ] Team notified of successful deployment

### Incident Response Checklist
- [ ] Incident identified and logged
- [ ] Initial assessment completed
- [ ] Team notified
- [ ] Investigation started
- [ ] Resolution implemented
- [ ] Post-incident review scheduled
- [ ] Documentation updated

## 📚 Reference Materials

### Useful Commands
```bash
# Quick health check
kubectl get pods -n tosserp -l component=stock

# Check logs
kubectl logs -n tosserp -l app=stock-api --tail=100

# Execute commands in containers
kubectl exec -n tosserp -it stock-postgres -- psql -U postgres -d stock_db

# Port forward for local access
kubectl port-forward -n tosserp svc/stock-api-service 8080:80
```

### Documentation Links
- [Stock Module README](./README.md)
- [API Documentation](./Stock.API/README.md)
- [Domain Model Documentation](./Stock.Domain/README.md)
- [Infrastructure Documentation](./Stock.Infrastructure/README.md)

### Contact Information
- **Development Team**: [Team Email]
- **DevOps Team**: [DevOps Email]
- **Security Team**: [Security Email]
- **Business Stakeholders**: [Stakeholder Email]
