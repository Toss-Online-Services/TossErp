# AI Service-as-a-Software API Reference

## Overview

This document provides a comprehensive reference for the TOSS ERP III AI Service-as-a-Software API endpoints. These endpoints enable autonomous business services through conversational interfaces and automated workflows.

## Base URL

```
Development: http://localhost:5000
Production: https://ai.toss-erp.com
```

## Authentication

All API endpoints require authentication using JWT Bearer tokens:

```http
Authorization: Bearer <your-jwt-token>
```

## Common Response Format

All API responses follow this standard format:

```json
{
    "success": true,
    "data": {
        // Response data here
    },
    "message": "Operation completed successfully",
    "timestamp": "2024-01-15T10:30:00Z",
    "requestId": "req_123456789"
}
```

## Error Response Format

```json
{
    "success": false,
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "Invalid request parameters",
        "details": {
            "field": "userId",
            "issue": "Required field is missing"
        }
    },
    "timestamp": "2024-01-15T10:30:00Z",
    "requestId": "req_123456789"
}
```

## Endpoints

### 1. Conversational Interface

#### POST /api/ai/conversation

Processes natural language conversation and returns AI response with autonomous actions.

**Request Body:**
```json
{
    "userId": "string (required)",
    "message": "string (required)",
    "language": "string (optional, default: 'en')",
    "channel": "string (optional, default: 'web')",
    "context": {
        "sessionId": "string (optional)",
        "previousMessages": ["array of previous messages (optional)"],
        "businessContext": {
            "businessType": "string (optional)",
            "location": "string (optional)",
            "preferences": "object (optional)"
        }
    }
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "response": "I've checked your inventory and found 3 items need reordering. I've automatically prepared purchase orders for bread, milk, and sugar.",
        "actions": [
            {
                "type": "inventory_check",
                "description": "Checked current stock levels",
                "timestamp": "2024-01-15T10:30:00Z"
            },
            {
                "type": "purchase_order_created",
                "description": "Created purchase order for low stock items",
                "timestamp": "2024-01-15T10:30:01Z"
            }
        ],
        "intent": {
            "primary": "inventory_status",
            "confidence": 0.95,
            "entities": {
                "business_function": "inventory",
                "action": "check_status"
            }
        },
        "suggestions": [
            "Would you like me to show you the purchase order details?",
            "Should I set up automatic reordering for these items?"
        ]
    },
    "message": "Conversation processed successfully",
    "timestamp": "2024-01-15T10:30:02Z"
}
```

**Error Codes:**
- `400 Bad Request`: Invalid request format or missing required fields
- `401 Unauthorized`: Missing or invalid authentication token
- `422 Unprocessable Entity`: Invalid business context or user permissions
- `500 Internal Server Error`: AI processing error

### 2. Automation Engine

#### POST /api/ai/automate

Executes automated business processes and workflows.

**Request Body:**
```json
{
    "userId": "string (required)",
    "service": "string (required) - inventory|sales|purchasing|finance|customer_service",
    "action": "string (required) - action to perform",
    "parameters": {
        "check_interval": "string (optional) - 1_hour|4_hours|daily|weekly",
        "threshold": "number (optional) - threshold values",
        "notifications": "boolean (optional) - enable notifications",
        "auto_approve": "boolean (optional) - auto-approve actions"
    },
    "schedule": {
        "type": "string (optional) - immediate|scheduled|recurring",
        "startTime": "ISO 8601 datetime (optional)",
        "frequency": "string (optional) - daily|weekly|monthly"
    }
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "automationId": "auto_123456789",
        "status": "running",
        "service": "inventory",
        "action": "monitor_and_reorder",
        "startedAt": "2024-01-15T10:30:00Z",
        "estimatedCompletion": "2024-01-15T10:35:00Z",
        "progress": {
            "currentStep": "checking_inventory_levels",
            "completedSteps": 2,
            "totalSteps": 5,
            "percentage": 40
        },
        "results": [
            {
                "step": "inventory_check",
                "status": "completed",
                "details": "Checked 150 items, found 3 below threshold"
            }
        ]
    },
    "message": "Automation started successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

**Error Codes:**
- `400 Bad Request`: Invalid service or action parameters
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: User not authorized for requested automation
- `409 Conflict`: Automation already running for this service
- `500 Internal Server Error`: Automation engine error

#### GET /api/ai/automate/{automationId}

Get the status and results of a specific automation.

**Response:**
```json
{
    "success": true,
    "data": {
        "automationId": "auto_123456789",
        "status": "completed",
        "service": "inventory",
        "action": "monitor_and_reorder",
        "startedAt": "2024-01-15T10:30:00Z",
        "completedAt": "2024-01-15T10:32:15Z",
        "duration": "PT2M15S",
        "results": [
            {
                "step": "inventory_check",
                "status": "completed",
                "details": "Checked 150 items, found 3 below threshold"
            },
            {
                "step": "purchase_order_creation",
                "status": "completed",
                "details": "Created 3 purchase orders for low stock items"
            },
            {
                "step": "supplier_notification",
                "status": "completed",
                "details": "Sent notifications to 3 suppliers"
            }
        ],
        "summary": {
            "itemsChecked": 150,
            "ordersCreated": 3,
            "suppliersNotified": 3,
            "estimatedSavings": 2500.00
        }
    },
    "message": "Automation completed successfully",
    "timestamp": "2024-01-15T10:32:15Z"
}
```

### 3. Service Status

#### GET /api/ai/services/status

Get the status of all autonomous services and agents.

**Response:**
```json
{
    "success": true,
    "data": {
        "overallStatus": "healthy",
        "lastUpdated": "2024-01-15T10:30:00Z",
        "services": [
            {
                "name": "inventory",
                "status": "active",
                "uptime": "99.8%",
                "lastActivity": "2024-01-15T10:25:00Z",
                "automations": {
                    "running": 2,
                    "completed": 45,
                    "failed": 0
                },
                "performance": {
                    "averageResponseTime": "1.2s",
                    "successRate": "99.5%",
                    "activeConnections": 5
                }
            },
            {
                "name": "sales",
                "status": "active",
                "uptime": "99.9%",
                "lastActivity": "2024-01-15T10:28:00Z",
                "automations": {
                    "running": 1,
                    "completed": 23,
                    "failed": 0
                },
                "performance": {
                    "averageResponseTime": "0.8s",
                    "successRate": "99.8%",
                    "activeConnections": 3
                }
            }
        ],
        "systemMetrics": {
            "cpuUsage": "15%",
            "memoryUsage": "45%",
            "diskUsage": "30%",
            "activeUsers": 12
        }
    },
    "message": "Service status retrieved successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

### 4. Business Outcomes

#### GET /api/ai/outcomes

Get business outcomes and ROI analysis for autonomous services.

**Query Parameters:**
- `period`: `string (optional)` - daily|weekly|monthly|quarterly|yearly
- `service`: `string (optional)` - specific service to filter
- `startDate`: `ISO 8601 date (optional)` - start date for period
- `endDate`: `ISO 8601 date (optional)` - end date for period

**Response:**
```json
{
    "success": true,
    "data": {
        "period": "monthly",
        "startDate": "2024-01-01",
        "endDate": "2024-01-31",
        "summary": {
            "totalValueGenerated": 15000.00,
            "timeSaved": "120 hours",
            "tasksAutomated": 450,
            "costSavings": 8000.00,
            "revenueIncrease": 7000.00
        },
        "serviceBreakdown": [
            {
                "service": "inventory",
                "valueGenerated": 6000.00,
                "timeSaved": "40 hours",
                "tasksAutomated": 180,
                "roi": "300%",
                "keyMetrics": {
                    "stockoutsPrevented": 15,
                    "ordersOptimized": 45,
                    "inventoryTurnover": "8.5"
                }
            },
            {
                "service": "sales",
                "valueGenerated": 4000.00,
                "timeSaved": "35 hours",
                "tasksAutomated": 120,
                "roi": "250%",
                "keyMetrics": {
                    "customersRetained": 25,
                    "salesProcessed": 180,
                    "customerSatisfaction": "4.8/5"
                }
            }
        ],
        "trends": {
            "valueGrowth": "+15%",
            "efficiencyImprovement": "+20%",
            "costReduction": "-25%"
        }
    },
    "message": "Business outcomes retrieved successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

#### POST /api/ai/outcomes/record

Record a new business outcome for tracking and analysis.

**Request Body:**
```json
{
    "userId": "string (required)",
    "service": "string (required)",
    "action": "string (required)",
    "outcome": {
        "type": "string (required) - time_saved|cost_saved|revenue_generated|error_prevented",
        "value": "number (required)",
        "currency": "string (optional, default: 'ZAR')",
        "description": "string (required)",
        "timestamp": "ISO 8601 datetime (optional)"
    },
    "context": {
        "businessFunction": "string (optional)",
        "automationId": "string (optional)",
        "relatedTasks": ["array of related task IDs (optional)"]
    }
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "outcomeId": "outcome_123456789",
        "recordedAt": "2024-01-15T10:30:00Z",
        "status": "recorded"
    },
    "message": "Business outcome recorded successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

### 5. Service Configuration

#### GET /api/ai/services/config

Get current configuration for all autonomous services.

**Response:**
```json
{
    "success": true,
    "data": {
        "services": [
            {
                "name": "inventory",
                "enabled": true,
                "config": {
                    "reorderThreshold": 10,
                    "autoReorder": true,
                    "checkInterval": "4_hours",
                    "notifications": {
                        "email": true,
                        "sms": false,
                        "push": true
                    },
                    "approvalRequired": {
                        "ordersAbove": 5000.00,
                        "newSuppliers": true
                    }
                }
            }
        ],
        "globalSettings": {
            "language": "en",
            "timezone": "Africa/Johannesburg",
            "currency": "ZAR",
            "notifications": {
                "defaultChannel": "email",
                "quietHours": {
                    "start": "22:00",
                    "end": "07:00"
                }
            }
        }
    },
    "message": "Service configuration retrieved successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

#### PUT /api/ai/services/config

Update configuration for autonomous services.

**Request Body:**
```json
{
    "service": "string (required)",
    "config": {
        "enabled": "boolean (optional)",
        "reorderThreshold": "number (optional)",
        "autoReorder": "boolean (optional)",
        "checkInterval": "string (optional)",
        "notifications": {
            "email": "boolean (optional)",
            "sms": "boolean (optional)",
            "push": "boolean (optional)"
        }
    }
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "service": "inventory",
        "configUpdated": true,
        "appliedAt": "2024-01-15T10:30:00Z"
    },
    "message": "Service configuration updated successfully",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

## Rate Limiting

The API implements rate limiting to ensure fair usage:

- **Conversational endpoints**: 100 requests per minute per user
- **Automation endpoints**: 20 requests per minute per user
- **Status endpoints**: 60 requests per minute per user
- **Configuration endpoints**: 10 requests per minute per user

Rate limit headers are included in responses:

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1642234567
```

## Webhooks

The AI service can send webhooks for important events:

### Webhook Events

- `automation.completed` - When an automation workflow completes
- `service.status_changed` - When a service status changes
- `outcome.recorded` - When a business outcome is recorded
- `conversation.processed` - When a conversation is processed

### Webhook Payload Example

```json
{
    "event": "automation.completed",
    "timestamp": "2024-01-15T10:30:00Z",
    "data": {
        "automationId": "auto_123456789",
        "service": "inventory",
        "status": "completed",
        "results": {
            "itemsChecked": 150,
            "ordersCreated": 3
        }
    }
}
```

## SDKs and Libraries

### .NET Client

```csharp
using TossErp.AI.Client;

var client = new TossAiClient("https://ai.toss-erp.com", "your-api-key");

// Process conversation
var response = await client.ProcessConversationAsync(new ConversationRequest
{
    UserId = "user123",
    Message = "What's my inventory status?"
});

// Start automation
var automation = await client.StartAutomationAsync(new AutomationRequest
{
    UserId = "user123",
    Service = "inventory",
    Action = "monitor_and_reorder"
});
```

### JavaScript Client

```javascript
import { TossAiClient } from '@toss-erp/ai-client';

const client = new TossAiClient('https://ai.toss-erp.com', 'your-api-key');

// Process conversation
const response = await client.processConversation({
    userId: 'user123',
    message: 'What\'s my inventory status?'
});

// Start automation
const automation = await client.startAutomation({
    userId: 'user123',
    service: 'inventory',
    action: 'monitor_and_reorder'
});
```

## Error Handling

### Common Error Codes

| Code | Description | Resolution |
|------|-------------|------------|
| `AUTH_REQUIRED` | Authentication token missing or invalid | Include valid JWT token in Authorization header |
| `RATE_LIMIT_EXCEEDED` | Rate limit exceeded | Wait for rate limit reset or implement exponential backoff |
| `SERVICE_UNAVAILABLE` | Service temporarily unavailable | Retry with exponential backoff |
| `INVALID_PARAMETERS` | Request parameters invalid | Check parameter validation rules |
| `BUSINESS_RULE_VIOLATION` | Request violates business rules | Review business logic and adjust request |
| `AUTOMATION_CONFLICT` | Conflicting automation already running | Wait for existing automation to complete |

### Retry Strategy

For transient errors (5xx status codes), implement exponential backoff:

```csharp
public async Task<T> RetryWithBackoff<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int i = 0; i <= maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (HttpRequestException ex) when (i < maxRetries)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
            await Task.Delay(delay);
        }
    }
    throw new Exception("Max retries exceeded");
}
```

## Testing

### Test Environment

Use the test environment for development and testing:

```
Base URL: https://ai-test.toss-erp.com
```

### Test Data

The test environment includes sample data for testing:

- **Test Users**: `test-user-1`, `test-user-2`, `test-user-3`
- **Test Businesses**: Sample inventory, sales, and financial data
- **Test Automations**: Pre-configured automation workflows

### Integration Tests

```csharp
[Test]
public async Task ProcessConversation_WithValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new ConversationRequest
    {
        UserId = "test-user-1",
        Message = "Show me my inventory status"
    };

    // Act
    var response = await client.ProcessConversationAsync(request);

    // Assert
    Assert.IsTrue(response.Success);
    Assert.IsNotNull(response.Data.Response);
    Assert.IsNotEmpty(response.Data.Actions);
}
```

## Support

For API support and questions:

- **Documentation**: [https://docs.toss-erp.com/ai-api](https://docs.toss-erp.com/ai-api)
- **Developer Portal**: [https://developers.toss-erp.com](https://developers.toss-erp.com)
- **Support Email**: api-support@toss-erp.com
- **Community Forum**: [https://community.toss-erp.com](https://community.toss-erp.com)
