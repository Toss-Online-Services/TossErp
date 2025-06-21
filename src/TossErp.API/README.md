# TossErp API - Township and Rural Enterprise Management

## Overview

The TossErp API provides comprehensive management capabilities for township and rural enterprises in South Africa, including spaza shops, hawkers, shebeens, cooperatives, stokvels, and agricultural enterprises. This API follows Clean Architecture principles and implements modern .NET 8+ best practices.

## Architecture

The system follows Clean Architecture with the following layers:

- **Domain Layer**: Core business entities, value objects, and domain events
- **Application Layer**: Use cases, DTOs, and business logic services
- **Infrastructure Layer**: Data access, external integrations, and implementations
- **API Layer**: RESTful endpoints and controllers

## Features

### Township Enterprises
- Create and manage township businesses (spaza shops, hawkers, shebeens, etc.)
- Business registration and licensing
- Document management
- Contact management
- Location-based filtering

### Cooperatives
- Cooperative formation and management
- Member management with share values
- Document storage
- Meeting scheduling
- Financial tracking

### Stokvels
- Stokvel creation and management
- Member management
- Contribution tracking
- Payout processing
- Meeting coordination
- Rotation management

## API Endpoints

### Township Enterprises

#### Create Township Enterprise
```http
POST /api/townshipenterprise
Content-Type: application/json
Authorization: Bearer {token}

{
  "businessName": "Mama's Spaza Shop",
  "tradingName": "Mama's Corner",
  "businessType": "SpazaShop",
  "businessDescription": "Local convenience store",
  "businessAddress": "123 Main Street",
  "township": "Soweto",
  "province": "Gauteng",
  "postalCode": "1804",
  "contactNumber": "+27123456789",
  "emailAddress": "mama@spaza.co.za",
  "ownerId": "guid-here",
  "establishedDate": "2020-01-15T00:00:00Z"
}
```

#### Get Township Enterprise
```http
GET /api/townshipenterprise/{id}
Authorization: Bearer {token}
```

#### Update Township Enterprise
```http
PUT /api/townshipenterprise
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": "guid-here",
  "businessName": "Updated Business Name",
  "tradingName": "Updated Trading Name",
  "businessDescription": "Updated description",
  "businessAddress": "456 New Street",
  "township": "Alexandra",
  "province": "Gauteng",
  "postalCode": "2090",
  "contactNumber": "+27123456789",
  "emailAddress": "updated@email.co.za"
}
```

#### Register Township Enterprise
```http
POST /api/townshipenterprise/register
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": "guid-here",
  "registrationNumber": "REG123456",
  "taxNumber": "TAX789012"
}
```

#### Get Township Enterprises with Filtering
```http
GET /api/townshipenterprise?businessName=Mama&businessType=SpazaShop&township=Soweto&isRegistered=true&pageNumber=1&pageSize=10&sortBy=businessName&sortDescending=false
Authorization: Bearer {token}
```

### Cooperatives

#### Create Cooperative
```http
POST /api/cooperative
Content-Type: application/json
Authorization: Bearer {token}

{
  "cooperativeName": "Soweto Farmers Cooperative",
  "tradingName": "Soweto Fresh",
  "cooperativeType": "Agricultural",
  "description": "Agricultural cooperative for local farmers",
  "address": "789 Farm Road",
  "township": "Soweto",
  "province": "Gauteng",
  "postalCode": "1804",
  "contactNumber": "+27123456789",
  "emailAddress": "info@sowetofresh.co.za",
  "establishedDate": "2020-01-15T00:00:00Z"
}
```

#### Add Cooperative Member
```http
POST /api/cooperative/members
Content-Type: application/json
Authorization: Bearer {token}

{
  "cooperativeId": "guid-here",
  "memberId": "member-guid-here",
  "memberName": "John Doe",
  "memberNumber": "MEM001",
  "joinDate": "2024-01-15T00:00:00Z",
  "shareValue": 1000.00,
  "notes": "New member"
}
```

### Stokvels

#### Create Stokvel
```http
POST /api/stokvel
Content-Type: application/json
Authorization: Bearer {token}

{
  "stokvelName": "Soweto Savings Group",
  "stokvelType": "Savings",
  "description": "Monthly savings stokvel",
  "contributionAmount": 500.00,
  "contributionFrequency": "Monthly",
  "memberLimit": 20,
  "establishedDate": "2020-01-15T00:00:00Z"
}
```

#### Record Contribution
```http
POST /api/stokvel/contributions
Content-Type: application/json
Authorization: Bearer {token}

{
  "stokvelId": "guid-here",
  "memberId": "member-guid-here",
  "amount": 500.00,
  "contributionDate": "2024-01-15T00:00:00Z",
  "referenceNumber": "REF123456",
  "notes": "January contribution"
}
```

#### Process Payout
```http
POST /api/stokvel/payouts
Content-Type: application/json
Authorization: Bearer {token}

{
  "stokvelId": "guid-here",
  "memberId": "member-guid-here",
  "amount": 10000.00,
  "payoutDate": "2024-01-15T00:00:00Z",
  "referenceNumber": "PAY123456",
  "notes": "Monthly payout"
}
```

## Authentication

The API uses JWT Bearer token authentication. Include the token in the Authorization header:

```http
Authorization: Bearer {your-jwt-token}
```

## Configuration

### Database Connection
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=TossErpDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### JWT Configuration
Configure JWT settings in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHereThatShouldBeAtLeast32CharactersLong",
    "Issuer": "TossErp",
    "Audience": "TossErpUsers",
    "ExpiryInMinutes": 60
  }
}
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
2. Navigate to the API project:
   ```bash
   cd src/TossErp.API
   ```

3. Update the connection string in `appsettings.json`

4. Run the application:
   ```bash
   dotnet run
   ```

5. Access the Swagger UI at `https://localhost:7001` (or the configured port)

### Database Setup

The database will be created automatically when you first run the application. The system uses Entity Framework Core with code-first migrations.

## Business Logic Features

### Township Enterprises
- **Business Types**: Spaza shops, hawkers, shebeens, agricultural enterprises, etc.
- **Licensing**: Track business licenses with expiry dates and validation
- **Documents**: Store and manage business documents
- **Contacts**: Manage multiple business contacts
- **Location Services**: Filter by township and province

### Cooperatives
- **Member Management**: Add/remove members with share values
- **Financial Tracking**: Track total share values and member contributions
- **Meeting Management**: Schedule and track cooperative meetings
- **Document Storage**: Store cooperative documents

### Stokvels
- **Contribution Tracking**: Record and track member contributions
- **Payout Processing**: Process payouts to members
- **Rotation Management**: Manage member rotation for payouts
- **Balance Tracking**: Track individual member balances
- **Meeting Coordination**: Schedule stokvel meetings

## Error Handling

The API returns standardized error responses:

```json
{
  "error": "Error message description"
}
```

Common HTTP status codes:
- `200 OK`: Success
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Security

- JWT Bearer token authentication
- Role-based authorization
- Input validation and sanitization
- SQL injection prevention through Entity Framework
- CORS configuration for cross-origin requests

## Performance

- Entity Framework Core with optimized queries
- Pagination for large datasets
- Indexed database fields for fast queries
- Async/await patterns for non-blocking operations

## Monitoring and Logging

- Structured logging with Serilog
- Request/response logging
- Error tracking and monitoring
- Performance metrics

## Testing

The API includes comprehensive unit tests and integration tests:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/TossErp.API.Tests
```

## Deployment

### Docker
```bash
# Build the image
docker build -t tosserp-api .

# Run the container
docker run -p 8080:80 tosserp-api
```

### Azure
The API can be deployed to Azure App Service or Azure Container Instances.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For support and questions, please contact the development team or create an issue in the repository. 