# Service-as-a-Software Troubleshooting Guide

## Overview

This troubleshooting guide provides solutions for common issues encountered when using or developing the TOSS ERP III Service-as-a-Software platform. It covers both user-facing issues and technical problems that developers may encounter.

## Table of Contents

- [Common User Issues](#common-user-issues)
- [Technical Issues](#technical-issues)
- [Performance Problems](#performance-problems)
- [Integration Issues](#integration-issues)
- [Security and Authentication](#security-and-authentication)
- [Monitoring and Debugging](#monitoring-and-debugging)
- [Getting Help](#getting-help)

## Common User Issues

### Issue: AI Not Responding to Messages

**Symptoms:**
- AI doesn't respond to chat messages
- Messages appear to be sent but no response received
- Error message: "Service temporarily unavailable"

**Possible Causes:**
1. Network connectivity issues
2. AI service is down or overloaded
3. Authentication token expired
4. Rate limiting exceeded

**Solutions:**

1. **Check Network Connection**
   ```
   - Verify internet connection is stable
   - Try refreshing the page
   - Check if other services are working
   ```

2. **Restart the Application**
   ```
   - Close and reopen the browser/app
   - Clear browser cache and cookies
   - Try accessing from a different device
   ```

3. **Check Service Status**
   ```
   - Visit the service status page
   - Look for any maintenance notifications
   - Contact support if service is down
   ```

4. **Verify Authentication**
   ```
   - Log out and log back in
   - Check if your session has expired
   - Ensure you have proper permissions
   ```

### Issue: Wrong Actions Being Taken

**Symptoms:**
- AI places orders for wrong items
- Incorrect inventory adjustments
- Unexpected automation triggers

**Possible Causes:**
1. Misconfigured automation rules
2. Incorrect business settings
3. Data synchronization issues
4. AI misinterpreted user intent

**Solutions:**

1. **Review Automation Settings**
   ```
   - Go to Settings → AI Services
   - Check reorder thresholds and rules
   - Verify approval limits and conditions
   - Review recent automation history
   ```

2. **Check Business Configuration**
   ```
   - Verify inventory levels are correct
   - Check supplier information
   - Review customer data accuracy
   - Ensure business hours are set correctly
   ```

3. **Review Recent Actions**
   ```
   - Check the activity log
   - Review what triggered the action
   - Verify the action was appropriate
   - Contact support if action was incorrect
   ```

4. **Adjust AI Behavior**
   ```
   - Set higher approval thresholds
   - Enable manual confirmation for critical actions
   - Disable specific automations temporarily
   - Provide feedback to improve AI accuracy
   ```

### Issue: Too Many Notifications

**Symptoms:**
- Receiving excessive email/SMS notifications
- Notifications at inappropriate times
- Duplicate notifications

**Possible Causes:**
1. Notification settings too aggressive
2. Multiple automation rules triggering
3. System errors causing repeated notifications
4. Quiet hours not configured properly

**Solutions:**

1. **Adjust Notification Settings**
   ```
   - Go to Settings → Notifications
   - Reduce notification frequency
   - Set up quiet hours (e.g., 10 PM - 7 AM)
   - Choose only critical alerts
   ```

2. **Review Automation Rules**
   ```
   - Check for duplicate automation rules
   - Adjust trigger conditions
   - Set longer intervals between checks
   - Disable non-essential automations
   ```

3. **Configure Quiet Hours**
   ```
   - Set business hours appropriately
   - Configure quiet hours for off-hours
   - Set up emergency contact for urgent issues
   - Use different notification levels
   ```

### Issue: Data Seems Incorrect

**Symptoms:**
- Inventory levels don't match reality
- Sales figures seem wrong
- Customer information outdated
- Financial reports inaccurate

**Possible Causes:**
1. Data synchronization issues
2. Manual data entry errors
3. System integration problems
4. Cache or stale data

**Solutions:**

1. **Check Data Synchronization**
   ```
   - Verify last sync timestamp
   - Manually trigger data sync
   - Check for sync errors in logs
   - Contact support if sync is failing
   ```

2. **Verify Manual Entries**
   ```
   - Review recent manual transactions
   - Check for duplicate entries
   - Verify data entry accuracy
   - Reconcile with external records
   ```

3. **Clear Cache and Refresh**
   ```
   - Clear browser cache
   - Refresh data from server
   - Restart the application
   - Check if issue persists
   ```

4. **Contact Support**
   ```
   - Provide specific examples of incorrect data
   - Include screenshots if possible
   - Mention when the issue started
   - Share any recent changes made
   ```

## Technical Issues

### Issue: Service Won't Start

**Symptoms:**
- AI service fails to start
- Error messages during startup
- Service crashes immediately

**Possible Causes:**
1. Configuration errors
2. Missing dependencies
3. Port conflicts
4. Database connection issues

**Solutions:**

1. **Check Configuration**
   ```bash
   # Verify configuration files
   cat appsettings.json
   cat appsettings.Development.json
   
   # Check for syntax errors
   dotnet build
   ```

2. **Verify Dependencies**
   ```bash
   # Restore packages
   dotnet restore
   
   # Check for missing packages
   dotnet list package
   
   # Update packages if needed
   dotnet add package <package-name>
   ```

3. **Check Port Availability**
   ```bash
   # Check if port is in use
   netstat -an | grep :5000
   
   # Kill process using port if needed
   sudo kill -9 <PID>
   ```

4. **Database Connection**
   ```bash
   # Test database connection
   dotnet ef database update
   
   # Check connection string
   echo $ConnectionStrings__DefaultConnection
   ```

### Issue: Agents Not Responding

**Symptoms:**
- Specific agents not working
- Actions fail with "Agent not found" errors
- Agent status shows as "inactive"

**Possible Causes:**
1. Agent not registered in DI container
2. Agent configuration issues
3. Agent dependencies missing
4. Agent disabled in settings

**Solutions:**

1. **Check Agent Registration**
   ```csharp
   // In Program.cs, verify agent is registered
   builder.Services.AddScoped<IInventoryAgent, InventoryAgent>();
   builder.Services.AddScoped<IAutonomousAgent>(sp => sp.GetRequiredService<IInventoryAgent>());
   ```

2. **Verify Agent Configuration**
   ```json
   {
     "AgentSettings": {
       "Inventory": {
         "Enabled": true,
         "CheckInterval": "00:04:00"
       }
     }
   }
   ```

3. **Check Agent Dependencies**
   ```csharp
   // Ensure all dependencies are registered
   builder.Services.AddScoped<IInventoryService, InventoryService>();
   builder.Services.AddScoped<IPurchasingService, PurchasingService>();
   builder.Services.AddScoped<IEventBus, EventBus>();
   ```

4. **Enable Agent in Settings**
   ```csharp
   // Check if agent is enabled
   var config = configuration.GetSection("AgentSettings:Inventory").Get<InventoryAgentConfiguration>();
   if (!config.Enabled)
   {
       // Enable the agent
   }
   ```

### Issue: Event Bus Not Working

**Symptoms:**
- Events not being published
- Event handlers not triggered
- Integration events failing

**Possible Causes:**
1. Event bus configuration issues
2. Message broker connection problems
3. Event handler registration issues
4. Serialization problems

**Solutions:**

1. **Check Event Bus Configuration**
   ```csharp
   // Verify MassTransit configuration
   builder.Services.AddMassTransit(x =>
   {
       x.UsingRabbitMq((context, cfg) =>
       {
           cfg.Host("localhost", "/", h =>
           {
               h.Username("guest");
               h.Password("guest");
           });
       });
   });
   ```

2. **Verify Message Broker**
   ```bash
   # Check RabbitMQ status
   sudo systemctl status rabbitmq-server
   
   # Restart if needed
   sudo systemctl restart rabbitmq-server
   
   # Check connection
   rabbitmqctl status
   ```

3. **Register Event Handlers**
   ```csharp
   // Ensure event handlers are registered
   builder.Services.AddScoped<IIntegrationEventHandler<InventoryLowStockEvent>, InventoryEventHandler>();
   ```

4. **Check Event Serialization**
   ```csharp
   // Verify event classes are serializable
   [Serializable]
   public class InventoryLowStockEvent : IntegrationEvent
   {
       public Guid ItemId { get; set; }
       public string ItemName { get; set; }
   }
   ```

## Performance Problems

### Issue: Slow Response Times

**Symptoms:**
- AI responses take too long
- Actions timeout
- System feels sluggish

**Possible Causes:**
1. High CPU/memory usage
2. Database query performance
3. External API delays
4. Network latency

**Solutions:**

1. **Monitor System Resources**
   ```bash
   # Check CPU and memory usage
   top
   htop
   
   # Check disk usage
   df -h
   
   # Check network
   netstat -i
   ```

2. **Optimize Database Queries**
   ```sql
   -- Check slow queries
   SHOW PROCESSLIST;
   
   -- Add indexes if needed
   CREATE INDEX idx_inventory_stock_level ON InventoryItems(StockLevel);
   
   -- Optimize queries
   EXPLAIN SELECT * FROM InventoryItems WHERE StockLevel < 10;
   ```

3. **Implement Caching**
   ```csharp
   // Add caching for frequently accessed data
   builder.Services.AddMemoryCache();
   
   // Use caching in agents
   if (_cache.TryGetValue(cacheKey, out var cachedData))
   {
       return cachedData;
   }
   ```

4. **Optimize External Calls**
   ```csharp
   // Implement retry with exponential backoff
   var policy = Policy
       .Handle<HttpRequestException>()
       .WaitAndRetryAsync(3, retryAttempt => 
           TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
   ```

### Issue: High Memory Usage

**Symptoms:**
- Application using excessive memory
- Out of memory errors
- System becomes unresponsive

**Possible Causes:**
1. Memory leaks
2. Large data sets in memory
3. Inefficient data structures
4. Background services consuming memory

**Solutions:**

1. **Profile Memory Usage**
   ```bash
   # Monitor memory usage
   dotnet-counters monitor --process-id <PID>
   
   # Generate memory dump
   dotnet-dump collect --process-id <PID>
   ```

2. **Implement Memory Management**
   ```csharp
   // Use using statements for disposable objects
   using var scope = serviceProvider.CreateScope();
   using var context = scope.ServiceProvider.GetRequiredService<DbContext>();
   
   // Dispose of large objects
   GC.Collect();
   GC.WaitForPendingFinalizers();
   ```

3. **Optimize Data Loading**
   ```csharp
   // Use pagination for large datasets
   var items = await context.InventoryItems
       .Skip(page * pageSize)
       .Take(pageSize)
       .ToListAsync();
   
   // Use projection to load only needed fields
   var summaries = await context.InventoryItems
       .Select(i => new { i.Id, i.Name, i.StockLevel })
       .ToListAsync();
   ```

4. **Monitor Background Services**
   ```csharp
   // Add memory monitoring to background services
   public class InventoryMonitoringService : BackgroundService
   {
       protected override async Task ExecuteAsync(CancellationToken stoppingToken)
       {
           while (!stoppingToken.IsCancellationRequested)
           {
               // Monitor memory usage
               var memoryUsage = GC.GetTotalMemory(false);
               if (memoryUsage > threshold)
               {
                   GC.Collect();
               }
               
               await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
           }
       }
   }
   ```

## Integration Issues

### Issue: External API Failures

**Symptoms:**
- Supplier API calls failing
- Payment processing errors
- Email/SMS delivery issues

**Possible Causes:**
1. API credentials expired
2. Rate limiting
3. Network connectivity
4. API changes

**Solutions:**

1. **Check API Credentials**
   ```csharp
   // Verify API keys are valid
   var response = await httpClient.GetAsync("/api/health");
   if (!response.IsSuccessStatusCode)
   {
       // Refresh credentials or contact API provider
   }
   ```

2. **Implement Retry Logic**
   ```csharp
   var retryPolicy = Policy
       .Handle<HttpRequestException>()
       .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
       .WaitAndRetryAsync(3, retryAttempt => 
           TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
   ```

3. **Monitor API Limits**
   ```csharp
   // Check rate limit headers
   var remaining = response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault();
   if (int.TryParse(remaining, out var remainingCalls) && remainingCalls < 10)
   {
       // Implement backoff strategy
   }
   ```

4. **Handle API Changes**
   ```csharp
   // Version API calls
   var apiVersion = "v2";
   var response = await httpClient.GetAsync($"/api/{apiVersion}/orders");
   
   // Implement fallback for API changes
   try
   {
       return await CallNewApiVersion();
   }
   catch (ApiException ex) when (ex.StatusCode == 404)
   {
       return await CallLegacyApiVersion();
   }
   ```

### Issue: Database Connection Problems

**Symptoms:**
- Database connection timeouts
- Connection pool exhaustion
- Data synchronization failures

**Possible Causes:**
1. Database server overloaded
2. Connection string issues
3. Network connectivity
4. Connection pool limits

**Solutions:**

1. **Check Database Health**
   ```sql
   -- Check database status
   SELECT @@VERSION;
   SELECT COUNT(*) FROM sys.dm_exec_requests;
   
   -- Check for blocking
   SELECT * FROM sys.dm_tran_locks WHERE request_session_id != @@SPID;
   ```

2. **Optimize Connection String**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TossErp;Trusted_Connection=true;Max Pool Size=100;Connection Timeout=30;"
     }
   }
   ```

3. **Implement Connection Resilience**
   ```csharp
   // Use connection resilience
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(connectionString, sqlOptions =>
       {
           sqlOptions.EnableRetryOnFailure(
               maxRetryCount: 3,
               maxRetryDelay: TimeSpan.FromSeconds(30),
               errorNumbersToAdd: null);
       }));
   ```

4. **Monitor Connection Pool**
   ```csharp
   // Monitor connection pool usage
   var poolStats = await context.Database.GetConnectionAsync();
   // Log pool statistics
   ```

## Security and Authentication

### Issue: Authentication Failures

**Symptoms:**
- Users can't log in
- Token expiration errors
- Permission denied errors

**Possible Causes:**
1. JWT token expired
2. Invalid credentials
3. User account locked
4. Permission configuration issues

**Solutions:**

1. **Check Token Validity**
   ```csharp
   // Verify JWT token
   var tokenHandler = new JwtSecurityTokenHandler();
   var token = tokenHandler.ReadJwtToken(jwtToken);
   
   if (token.ValidTo < DateTime.UtcNow)
   {
       // Token expired, refresh or re-authenticate
   }
   ```

2. **Verify User Permissions**
   ```csharp
   // Check user permissions
   var user = await userManager.FindByIdAsync(userId);
   var roles = await userManager.GetRolesAsync(user);
   
   if (!roles.Contains("BusinessOwner"))
   {
       // Handle insufficient permissions
   }
   ```

3. **Implement Token Refresh**
   ```csharp
   // Implement automatic token refresh
   if (token.ValidTo < DateTime.UtcNow.AddMinutes(5))
   {
       var newToken = await RefreshTokenAsync(refreshToken);
       // Update stored token
   }
   ```

4. **Check Account Status**
   ```csharp
   // Verify account is not locked
   if (user.LockoutEnd > DateTime.UtcNow)
   {
       // Account is locked
       return BadRequest("Account is temporarily locked");
   }
   ```

### Issue: Data Security Concerns

**Symptoms:**
- Sensitive data exposed in logs
- Unauthorized access attempts
- Data encryption issues

**Possible Causes:**
1. Logging sensitive information
2. Missing encryption
3. Weak access controls
4. Configuration exposure

**Solutions:**

1. **Secure Logging**
   ```csharp
   // Don't log sensitive data
   _logger.LogInformation("Processing order for user {UserId}", userId);
   // NOT: _logger.LogInformation("Processing order with credit card {CardNumber}", cardNumber);
   ```

2. **Implement Data Encryption**
   ```csharp
   // Encrypt sensitive data
   var protector = dataProtectionProvider.CreateProtector("SensitiveData");
   var encrypted = protector.Protect(sensitiveData);
   var decrypted = protector.Unprotect(encrypted);
   ```

3. **Add Access Controls**
   ```csharp
   // Implement role-based access
   [Authorize(Roles = "BusinessOwner")]
   public async Task<IActionResult> GetFinancialData()
   {
       // Only business owners can access financial data
   }
   ```

4. **Secure Configuration**
   ```json
   // Use user secrets for sensitive configuration
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TossErp;User Id=sa;Password=<password>;"
     }
   }
   ```

## Monitoring and Debugging

### Issue: Difficult to Debug Problems

**Symptoms:**
- Hard to identify root cause
- Insufficient logging
- No error tracking

**Possible Causes:**
1. Poor logging implementation
2. Missing error tracking
3. No monitoring setup
4. Inadequate debugging tools

**Solutions:**

1. **Implement Structured Logging**
   ```csharp
   // Use structured logging
   _logger.LogInformation("Processing inventory action {Action} for user {UserId} with parameters {@Parameters}", 
       action, userId, parameters);
   
   // Log exceptions with context
   _logger.LogError(ex, "Failed to process inventory action {Action} for user {UserId}", 
       action, userId);
   ```

2. **Add Error Tracking**
   ```csharp
   // Implement error tracking
   try
   {
       await ProcessActionAsync();
   }
   catch (Exception ex)
   {
       // Log to error tracking service
       await errorTracker.CaptureExceptionAsync(ex);
       throw;
   }
   ```

3. **Set Up Monitoring**
   ```csharp
   // Add health checks
   builder.Services.AddHealthChecks()
       .AddCheck<DatabaseHealthCheck>("database")
       .AddCheck<ExternalApiHealthCheck>("external-api");
   
   app.MapHealthChecks("/health");
   ```

4. **Use Debugging Tools**
   ```csharp
   // Add debugging endpoints (development only)
   if (app.Environment.IsDevelopment())
   {
       app.MapGet("/debug/agents", async (IAutonomousAgentManager manager) =>
       {
           var status = await manager.GetServiceStatusAsync();
           return Results.Ok(status);
       });
   }
   ```

### Issue: No Visibility into System Health

**Symptoms:**
- Can't tell if system is working
- No performance metrics
- Difficult to identify issues

**Possible Causes:**
1. No monitoring setup
2. Missing health checks
3. No performance tracking
4. Inadequate alerting

**Solutions:**

1. **Implement Health Checks**
   ```csharp
   public class AgentHealthCheck : IHealthCheck
   {
       public async Task<HealthCheckResult> CheckHealthAsync(
           HealthCheckContext context, 
           CancellationToken cancellationToken = default)
       {
           try
           {
               var status = await _agentManager.GetServiceStatusAsync();
               return status.OverallStatus == "healthy" 
                   ? HealthCheckResult.Healthy() 
                   : HealthCheckResult.Degraded();
           }
           catch (Exception ex)
           {
               return HealthCheckResult.Unhealthy(ex);
           }
       }
   }
   ```

2. **Add Performance Metrics**
   ```csharp
   // Track performance metrics
   var timer = _metrics.CreateTimer("agent_action_duration");
   try
   {
       var result = await ExecuteActionAsync();
       _metrics.Increment("agent_action_success");
       return result;
   }
   finally
   {
       timer.Dispose();
   }
   ```

3. **Set Up Alerting**
   ```csharp
   // Configure alerts for critical issues
   if (errorRate > 0.05) // 5% error rate
   {
       await alertService.SendAlertAsync("High error rate detected");
   }
   ```

4. **Create Dashboard**
   ```csharp
   // Expose metrics endpoint
   app.MapGet("/metrics", async (IMetrics metrics) =>
   {
       var snapshot = metrics.Snapshot();
       return Results.Ok(snapshot);
   });
   ```

## Getting Help

### When to Contact Support

Contact support when you encounter:

1. **Critical Issues:**
   - System completely down
   - Data loss or corruption
   - Security breaches
   - Financial transaction failures

2. **Persistent Problems:**
   - Issues that persist after trying solutions
   - Performance problems affecting business
   - Integration failures with external systems

3. **Configuration Issues:**
   - Complex setup problems
   - Custom integration requirements
   - Advanced configuration needs

### Information to Provide

When contacting support, provide:

1. **Issue Description:**
   - Clear description of the problem
   - Steps to reproduce
   - Expected vs actual behavior

2. **Environment Details:**
   - System version and configuration
   - Browser/app version
   - Network environment

3. **Error Information:**
   - Error messages and codes
   - Screenshots if applicable
   - Log files if available

4. **Recent Changes:**
   - Any recent configuration changes
   - System updates or modifications
   - New integrations or features

### Support Channels

1. **In-App Support:**
   - Use the chat interface: "I need help"
   - Submit support ticket through the app

2. **Email Support:**
   - support@toss-erp.com
   - Include detailed information and screenshots

3. **Phone Support:**
   - +27 11 123 4567
   - Available during business hours

4. **Documentation:**
   - Check this troubleshooting guide
   - Review user guides and API documentation
   - Visit the help center

5. **Community Forum:**
   - Connect with other users
   - Share solutions and best practices
   - Get peer support

### Escalation Process

If your issue is not resolved:

1. **First Level:** Technical support team
2. **Second Level:** Senior technical specialists
3. **Third Level:** Development team
4. **Management:** For critical business impact issues

### Self-Service Resources

1. **Knowledge Base:**
   - Common solutions and workarounds
   - Best practices and tips
   - Video tutorials

2. **Status Page:**
   - Real-time system status
   - Maintenance schedules
   - Incident updates

3. **Training Resources:**
   - User training materials
   - Webinar recordings
   - Certification programs

## Conclusion

This troubleshooting guide covers the most common issues you may encounter with the TOSS ERP III Service-as-a-Software platform. Most problems can be resolved by following the solutions provided here.

Remember to:

1. **Start with the basics** - Check network, restart services, verify configuration
2. **Use systematic approach** - Identify symptoms, determine causes, apply solutions
3. **Document issues** - Keep records of problems and solutions for future reference
4. **Stay updated** - Keep the system and documentation current
5. **Seek help when needed** - Don't hesitate to contact support for complex issues

For the most up-to-date troubleshooting information, always check the latest documentation and support resources.
