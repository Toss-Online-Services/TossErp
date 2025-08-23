# TOSS ERP API Style Guide

## Overview

This style guide defines the design principles, naming conventions, and standards for TOSS ERP III APIs to ensure consistency, usability, and maintainability across all services.

## Design Principles

### 1. RESTful Design
- Use HTTP methods semantically (GET, POST, PUT, DELETE, PATCH)
- Design resource-oriented URLs
- Use proper HTTP status codes
- Support standard HTTP headers

### 2. Consistency
- Maintain consistent naming across all APIs
- Use standard patterns for common operations
- Follow established conventions for errors and responses
- Ensure predictable behavior across services

### 3. Developer Experience
- Provide clear and comprehensive documentation
- Use meaningful error messages with actionable guidance
- Include examples for all operations
- Support common development workflows

### 4. Performance
- Design for efficient data transfer
- Support pagination for large datasets
- Enable caching where appropriate
- Minimize round trips for common use cases

### 5. Security
- Implement proper authentication and authorization
- Validate all inputs rigorously
- Protect against common vulnerabilities
- Audit all sensitive operations

## URL Design

### Resource Naming
- Use **plural nouns** for collections: `/users`, `/accounts`, `/transactions`
- Use **lowercase** with **hyphens** for multi-word resources: `/purchase-orders`, `/tax-codes`
- Avoid verbs in URLs (use HTTP methods instead)
- Keep URLs short and readable

### URL Structure
```
/v{version}/{resource}
/v{version}/{resource}/{id}
/v{version}/{resource}/{id}/{sub-resource}
/v{version}/{resource}/{id}/{sub-resource}/{sub-id}
```

### Examples
```
✅ Good:
GET /v1/customers
GET /v1/customers/123
GET /v1/customers/123/orders
POST /v1/purchase-orders
PUT /v1/tax-codes/456

❌ Bad:
GET /v1/getCustomers
GET /v1/customer/123
POST /v1/createPurchaseOrder
PUT /v1/taxCode/456
```

### Query Parameters
- Use **camelCase** for parameter names: `?sortBy=name&orderBy=asc`
- Support standard parameters:
  - `cursor`: For pagination
  - `limit`: Maximum items per page (default: 20, max: 100)
  - `filter`: For filtering results
  - `sort`: For sorting results
  - `expand`: For including related resources

## HTTP Methods

### GET - Retrieve Resources
- **Safe**: No side effects
- **Idempotent**: Multiple calls have same effect
- Use for fetching data
- Support filtering, sorting, and pagination

```
GET /v1/products?category=electronics&sort=price&limit=50
```

### POST - Create Resources
- **Not safe**: Has side effects
- **Not idempotent**: Multiple calls may create multiple resources
- Use for creating new resources
- Support idempotency keys for safe retries

```
POST /v1/customers
Idempotency-Key: abc123-def456-789
Content-Type: application/json

{
  "name": "Acme Corp",
  "email": "contact@acme.com"
}
```

### PUT - Replace Resources
- **Not safe**: Has side effects
- **Idempotent**: Multiple calls have same effect
- Use for complete resource replacement
- Require all fields or use defaults
- Support conditional updates with If-Match

```
PUT /v1/customers/123
If-Match: "abc123"
Content-Type: application/json

{
  "name": "Acme Corporation",
  "email": "info@acme.com",
  "status": "active"
}
```

### PATCH - Partial Updates
- **Not safe**: Has side effects
- **Idempotent**: Multiple calls have same effect
- Use for partial resource updates
- Support JSON Patch format
- Support conditional updates with If-Match

```
PATCH /v1/customers/123
If-Match: "abc123"
Content-Type: application/json

{
  "email": "newemail@acme.com"
}
```

### DELETE - Remove Resources
- **Not safe**: Has side effects
- **Idempotent**: Multiple calls have same effect
- Use for resource deletion
- Return 204 No Content on success
- Return 404 if resource doesn't exist

```
DELETE /v1/customers/123
```

## Request/Response Format

### Content Types
- **Request**: `application/json` (primary), `application/x-www-form-urlencoded` (forms)
- **Response**: `application/json` (primary), `application/problem+json` (errors)
- **Documentation**: `text/html`, `application/pdf`

### Request Body Structure
```json
{
  "data": {
    // Resource data
  },
  "meta": {
    // Optional metadata
  }
}
```

### Response Body Structure
```json
{
  "data": {
    // Resource data or array of resources
  },
  "meta": {
    // Metadata (pagination, totals, etc.)
  }
}
```

### Field Naming
- Use **camelCase** for JSON field names
- Use descriptive names: `firstName` instead of `fname`
- Be consistent across similar resources
- Avoid abbreviations unless widely understood

## Status Codes

### Success Codes
- **200 OK**: Successful GET, PUT, PATCH
- **201 Created**: Successful POST with resource creation
- **202 Accepted**: Accepted for async processing
- **204 No Content**: Successful DELETE or PUT with no response body

### Client Error Codes
- **400 Bad Request**: Invalid request format or validation errors
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Resource conflict (duplicate, constraint violation)
- **422 Unprocessable Entity**: Valid format but semantic errors
- **429 Too Many Requests**: Rate limit exceeded

### Server Error Codes
- **500 Internal Server Error**: Unexpected server error
- **502 Bad Gateway**: Upstream service error
- **503 Service Unavailable**: Service temporarily unavailable
- **504 Gateway Timeout**: Upstream service timeout

## Error Handling

### Error Response Format (RFC 9457)
```json
{
  "type": "https://docs.tosserp.com/problems/validation-error",
  "title": "Validation Error",
  "status": 422,
  "detail": "One or more fields contain invalid data",
  "instance": "/v1/customers",
  "traceId": "abc123-def456-789",
  "errors": [
    {
      "field": "email",
      "code": "INVALID_FORMAT",
      "message": "Email address format is invalid"
    }
  ]
}
```

### Error Types
- **Validation Errors**: Field-specific validation failures
- **Authentication Errors**: Missing or invalid credentials
- **Authorization Errors**: Insufficient permissions
- **Business Logic Errors**: Rule violations or constraint failures
- **System Errors**: Internal failures or service unavailability

### Error Messages
- Provide clear, actionable error messages
- Include field-specific errors when applicable
- Use consistent error codes across services
- Include trace IDs for debugging
- Avoid exposing sensitive information

## Pagination

### Cursor-Based Pagination
```json
{
  "data": [
    // Array of resources
  ],
  "pagination": {
    "nextCursor": "eyJpZCI6MTIzfQ==",
    "prevCursor": null,
    "hasNext": true,
    "hasPrev": false,
    "count": 20
  }
}
```

### Query Parameters
- `cursor`: Position marker for next page
- `limit`: Number of items per page (default: 20, max: 100)

### Response Headers
- `Link`: RFC 5988 web linking for next/prev pages
- `X-Total-Count`: Total number of items (if available)

## Authentication & Authorization

### OAuth 2.0 Scopes
- Use descriptive, hierarchical scopes: `customers:read`, `orders:write`
- Follow least privilege principle
- Document required scopes for each endpoint
- Support scope inheritance where logical

### Headers
```
Authorization: Bearer {access_token}
X-Tenant-ID: {tenant_uuid}
X-API-Key: {api_key} // Alternative authentication
```

### Security Headers
```
X-Request-ID: {unique_request_id}
X-Correlation-ID: {correlation_id}
Idempotency-Key: {idempotency_key}
If-Match: {etag_value}
```

## Caching & Concurrency

### ETag Support
```
Response:
ETag: "abc123"

Conditional Request:
If-Match: "abc123"      // For updates
If-None-Match: "abc123" // For gets
```

### Cache Headers
```
Cache-Control: public, max-age=3600
Last-Modified: Tue, 15 Nov 2024 08:12:31 GMT
Expires: Tue, 15 Nov 2024 09:12:31 GMT
```

## Rate Limiting

### Headers
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1609459200
Retry-After: 60 // When rate limited
```

### Limits
- **Default**: 1000 requests per hour per client
- **Burst**: 100 requests per minute per client
- **Enterprise**: Custom limits based on agreement

## Webhooks

### Event Naming
- Use dot notation: `customer.created`, `order.updated`, `payment.failed`
- Past tense for completed events
- Hierarchical organization by domain

### Payload Structure
```json
{
  "eventType": "customer.created",
  "eventId": "uuid",
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    // Resource data
  },
  "signature": "hmac_signature"
}
```

### Security
- HMAC signatures for payload verification
- Retry logic with exponential backoff
- Timeout handling (5 seconds max)
- SSL/TLS required for webhook endpoints

## Documentation Standards

### OpenAPI Specifications
- Complete operation descriptions
- Request/response examples
- Error response documentation
- Security requirement specifications
- Schema definitions with examples

### Operation Documentation
```yaml
/customers:
  post:
    summary: Create a new customer
    description: |
      Creates a new customer record in the system. The customer
      will be assigned to the current tenant context.
    operationId: createCustomer
    tags: [customers]
    security:
      - OAuth2: [customers:write]
    parameters:
      - $ref: '#/components/parameters/TenantHeader'
      - $ref: '#/components/parameters/IdempotencyKey'
    requestBody:
      required: true
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/CustomerCreate'
          examples:
            basic:
              summary: Basic customer creation
              value:
                name: "Acme Corp"
                email: "contact@acme.com"
    responses:
      '201':
        description: Customer created successfully
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/Customer'
```

### Schema Documentation
```yaml
Customer:
  type: object
  description: A customer entity in the system
  properties:
    id:
      type: string
      format: uuid
      description: Unique customer identifier
      readOnly: true
      example: "123e4567-e89b-12d3-a456-426614174000"
    name:
      type: string
      description: Customer display name
      example: "Acme Corporation"
      minLength: 1
      maxLength: 255
    email:
      type: string
      format: email
      description: Primary contact email address
      example: "contact@acme.com"
  required:
    - name
    - email
```

## Testing Standards

### Contract Testing
- Maintain OpenAPI specifications as source of truth
- Use tools like Prism for mock servers
- Validate requests and responses against schemas
- Test examples in documentation

### Integration Testing
- Test complete user workflows
- Validate error handling scenarios
- Test authentication and authorization
- Verify rate limiting behavior

### Performance Testing
- Test pagination with large datasets
- Validate response times under load
- Test concurrent request handling
- Measure rate limiting accuracy

## Monitoring & Observability

### Logging Standards
- Log all API requests and responses
- Include correlation IDs for tracing
- Log business events and errors
- Maintain structured logging format

### Metrics
- Request/response times by endpoint
- Error rates by status code
- Rate limiting trigger frequency
- Authentication failure rates

### Tracing
- End-to-end request tracing
- Service dependency mapping
- Performance bottleneck identification
- Error propagation tracking

## Versioning

### URL Versioning
- Include major version in URL: `/v1/`, `/v2/`
- Support multiple versions simultaneously
- Provide migration paths between versions
- Follow semantic versioning principles

### Header Versioning
```
API-Version: 1.2.3
API-Deprecated: 2025-01-01
API-Sunset: 2026-01-01
```

## Examples

### Complete CRUD Operations

#### Create Customer
```
POST /v1/customers
Authorization: Bearer abc123
X-Tenant-ID: tenant-uuid
Idempotency-Key: create-customer-001
Content-Type: application/json

{
  "name": "Acme Corporation",
  "email": "contact@acme.com",
  "phone": "+1-555-0123"
}

Response:
201 Created
ETag: "version-1"
Location: /v1/customers/customer-uuid

{
  "data": {
    "id": "customer-uuid",
    "name": "Acme Corporation",
    "email": "contact@acme.com",
    "phone": "+1-555-0123",
    "status": "active",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Get Customer
```
GET /v1/customers/customer-uuid
Authorization: Bearer abc123
X-Tenant-ID: tenant-uuid

Response:
200 OK
ETag: "version-1"
Cache-Control: private, max-age=300

{
  "data": {
    "id": "customer-uuid",
    "name": "Acme Corporation",
    "email": "contact@acme.com",
    "phone": "+1-555-0123",
    "status": "active",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Update Customer
```
PATCH /v1/customers/customer-uuid
Authorization: Bearer abc123
X-Tenant-ID: tenant-uuid
If-Match: "version-1"
Content-Type: application/json

{
  "email": "newemail@acme.com"
}

Response:
200 OK
ETag: "version-2"

{
  "data": {
    "id": "customer-uuid",
    "name": "Acme Corporation",
    "email": "newemail@acme.com",
    "phone": "+1-555-0123",
    "status": "active",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:45:00Z"
  }
}
```

#### List Customers
```
GET /v1/customers?limit=10&cursor=eyJpZCI6MTIzfQ==
Authorization: Bearer abc123
X-Tenant-ID: tenant-uuid

Response:
200 OK
Link: </v1/customers?cursor=eyJpZCI6MTMzfQ==&limit=10>; rel="next"

{
  "data": [
    {
      "id": "customer-uuid-1",
      "name": "Customer 1",
      "email": "customer1@example.com",
      "status": "active"
    }
  ],
  "pagination": {
    "nextCursor": "eyJpZCI6MTMzfQ==",
    "prevCursor": null,
    "hasNext": true,
    "hasPrev": false,
    "count": 10
  }
}
```

#### Delete Customer
```
DELETE /v1/customers/customer-uuid
Authorization: Bearer abc123
X-Tenant-ID: tenant-uuid

Response:
204 No Content
```

This style guide serves as the foundation for all TOSS ERP API development and should be referenced throughout the development lifecycle.
