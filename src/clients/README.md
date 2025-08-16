# Client Applications

This directory contains all front-end client applications for the TossErp system, properly configured to communicate with backend services through the API Gateway.

## Structure

```
clients/
├── mobile/          # Mobile application (Flutter)
├── web/             # Web application (Vue.js/Nuxt.js)
├── admin/           # ERP III Admin Panel (React)
└── README.md        # This file
```

## Applications

### Mobile App (`mobile/`)
- **Technology**: Flutter with Dart
- **Purpose**: Mobile interface for field operations, inventory management, and sales
- **Features**: 
  - Inventory scanning and management
  - Sales order processing
  - Real-time stock updates
  - Offline capabilities
- **API Configuration**: Points to Gateway at `http://localhost:8080/api`
- **Environment File**: `env.template` (copy to `.env` for customization)

### Web App (`web/`)
- **Technology**: Vue.js with Nuxt.js
- **Purpose**: Web-based interface for general business operations
- **Features**:
  - Stock management
  - Purchase orders
  - Sales management
  - Basic reporting
- **API Configuration**: Uses Nuxt runtime config, points to Gateway
- **Environment File**: `env.template` (copy to `.env` for customization)

### Admin Panel (`admin/`)
- **Technology**: React with TypeScript and Redux Toolkit
- **Purpose**: ERP III comprehensive administrative interface
- **Features**:
  - Advanced analytics and reporting
  - User and role management
  - System configuration
  - Master data management
  - Financial management
  - Advanced inventory control
- **API Configuration**: RTK Query with Gateway endpoint
- **Environment File**: `env.template` (copy to `.env` for customization)

## Development Setup

### Prerequisites
1. **API Gateway Running**: Ensure the Gateway service is running on `http://localhost:8080`
2. **Stock API Running**: Ensure the Stock API service is accessible through the Gateway
3. **Environment Configuration**: Copy environment template files and configure as needed

### Setup Steps

#### 1. Configure Environment Variables
```bash
# For each client, copy the template and configure:
cd src/clients/mobile
cp env.template .env
# Edit .env with your configuration

cd ../web
cp env.template .env
# Edit .env with your configuration

cd ../admin
cp env.template .env
# Edit .env with your configuration
```

#### 2. Start Dependencies
```bash
# Start the API Gateway
cd src/Gateway
dotnet run

# Start the Stock API (in another terminal)
cd src/Services/Stock/Stock.API
dotnet run
```

#### 3. Start Client Applications
```bash
# Mobile (Flutter)
cd src/clients/mobile
flutter run

# Web (Nuxt)
cd src/clients/web
npm run dev

# Admin (React)
cd src/clients/admin
npm start
```

## Integration Architecture

### API Gateway Pattern
All clients communicate with backend services through the centralized API Gateway:

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Mobile    │    │    Web      │    │    Admin    │
│   Client    │    │   Client    │    │   Client    │
└─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │
       └───────────────────┼───────────────────┘
                           │
                    ┌─────────────┐
                    │    API      │
                    │  Gateway    │
                    │  :8080      │
                    └─────────────┘
                           │
                    ┌─────────────┐
                    │   Stock     │
                    │    API      │
                    │  :80        │
                    └─────────────┘
```

### Routing Configuration
The Gateway routes requests as follows:
- `/api/stock/*` → Stock API Service
- `/api/mobile/*` → Mobile-specific endpoints
- `/api/web/*` → Web-specific endpoints
- `/services/*` → Legacy service endpoints

### Authentication
- **JWT-based**: All clients use JWT tokens for authentication
- **Centralized**: Authentication handled by the Gateway
- **Role-based**: Access control based on user roles and permissions

## Testing Integration

### Run Integration Tests
```bash
# Test all client connectivity
./scripts/test-client-integration.sh
```

### Manual Testing
1. **Gateway Health**: `curl http://localhost:8080/health`
2. **Stock API**: `curl http://localhost:8080/api/stock/items`
3. **Mobile Dashboard**: `curl http://localhost:8080/api/mobile/dashboard`
4. **Web Dashboard**: `curl http://localhost:8080/api/web/dashboard`

## Environment Variables

### Common Configuration
All clients use these environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `API_BASE_URL` | Base URL for API endpoints | `http://localhost:8080/api` |
| `GATEWAY_URL` | Gateway service URL | `http://localhost:8080` |
| `API_TIMEOUT` | Request timeout in milliseconds | `30000` |

### Client-Specific Variables
Each client has additional environment-specific variables documented in their respective `env.template` files.

## Troubleshooting

### Common Issues

#### 1. Connection Refused
- Ensure Gateway service is running on port 8080
- Check if Stock API service is accessible
- Verify firewall settings

#### 2. CORS Errors
- Gateway CORS policy includes all client origins
- Check browser console for specific CORS errors
- Verify client origin is in Gateway CORS configuration

#### 3. Authentication Failures
- Verify JWT configuration in Gateway
- Check token expiration
- Ensure proper Authorization headers

#### 4. API Endpoint Not Found
- Verify Gateway routing configuration
- Check if Stock API endpoints are properly exposed
- Review Gateway logs for routing errors

### Debug Mode
Enable debug logging in client applications:
- **Mobile**: Set `DEBUG=true` in environment
- **Web**: Set `NODE_ENV=development`
- **Admin**: Set `REACT_APP_ENVIRONMENT=development`

## Performance Considerations

### Caching
- **Client-side**: Implement appropriate caching strategies
- **Gateway**: Response caching for frequently accessed data
- **API**: Database query optimization and result caching

### Connection Pooling
- **Gateway**: HTTP client connection pooling
- **Database**: Connection pooling for database operations
- **Redis**: Connection pooling for caching layer

### Monitoring
- **Health Checks**: Regular health check endpoints
- **Metrics**: Prometheus metrics collection
- **Logging**: Structured logging with correlation IDs

## Security

### Best Practices
- **HTTPS**: Use HTTPS in production environments
- **Input Validation**: Validate all client inputs
- **Rate Limiting**: Implement rate limiting at Gateway level
- **Audit Logging**: Log all client interactions
- **Token Management**: Secure JWT token storage and rotation

### Environment Security
- Never commit `.env` files to version control
- Use environment-specific configuration files
- Rotate API keys and secrets regularly
- Implement proper access controls for production environments
