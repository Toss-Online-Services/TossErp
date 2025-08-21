# Assets Infrastructure Layer

This document describes the comprehensive Infrastructure layer implementation for the Asset Management service in the TOSS ERP system.

## Overview

The Assets Infrastructure layer provides the complete data access, persistence, and external service integration layer for asset management operations. It implements modern EF Core 9 patterns with multi-tenant architecture, performance optimizations, and comprehensive business services.

## Architecture

### Multi-Tenant Design
- **Global Query Filters**: Automatic tenant isolation at the database level
- **Tenant-Aware Services**: All operations are scoped to the current tenant
- **Secure File Storage**: Tenant-isolated file storage with verification

### EF Core 9 Features
- **Fill Factors**: Optimized index performance with appropriate fill factors
- **Complex Types**: FinancialInfo value object as complex type
- **Hierarchical Partition Keys**: Optimized for multi-tenant queries
- **ExecuteUpdate/ExecuteDelete**: Bulk operations support
- **Improved LINQ Translation**: Better query performance

## Components

### 1. DbContext (`AssetsDbContext`)
```csharp
// Multi-tenant DbContext with EF Core 9 optimizations
public class AssetsDbContext : DbContext
```
**Features:**
- Global query filters for tenant isolation
- Automatic tenant ID assignment for new entities
- Audit field management (CreatedAt, UpdatedAt, etc.)
- EF Core 9 performance optimizations
- Temporal tables support for audit history

### 2. Entity Configurations
Located in `Data/Configurations/`:

#### Asset Configuration
- Complex type mapping for FinancialInfo
- Performance-optimized indexes with fill factors
- Comprehensive relationships
- Check constraints for data integrity

#### Category & Location Configuration
- Hierarchical relationships
- Unique constraints per tenant
- Optimized for lookup operations

#### Transfer & Maintenance Configuration
- Audit trail configurations
- Date-based indexing for reporting
- Status tracking optimizations

#### Document & Audit Configuration
- File management optimization
- Audit log performance tuning
- Metadata indexing

### 3. Repository Pattern

#### Generic Repository (`Repository<T>`)
```csharp
public class Repository<T> : IRepository<T> where T : class
```
**Features:**
- EF Core 9 ExecuteUpdate/ExecuteDelete support
- Specification pattern implementation
- Comprehensive CRUD operations
- Performance-optimized queries
- Bulk operations support

#### Asset Repository (`AssetRepository`)
```csharp
public class AssetRepository : Repository<Asset>, IAssetRepository
```
**Business-Specific Methods:**
- `GetByAssetTagAsync()` - Unique asset lookup
- `GetByStatusAsync()` - Status-based filtering
- `GetMaintenanceDueAsync()` - Maintenance scheduling
- `GetAssetsByValueRangeAsync()` - Financial reporting
- `SearchAssetsAsync()` - Advanced search capabilities

#### Unit of Work (`UnitOfWork`)
```csharp
public class UnitOfWork : IUnitOfWork
```
**Features:**
- Transaction management
- Multi-repository coordination
- Rollback capabilities
- Performance tracking

### 4. File Storage Services

#### Azure Blob Storage (`BlobFileStorageService`)
```csharp
public class BlobFileStorageService : IFileStorageService
```
**Features:**
- Tenant-isolated storage containers
- SAS URL generation for secure access
- Metadata management
- Comprehensive file operations
- Automatic container creation

#### Local File Storage (`LocalFileStorageService`)
```csharp
public class LocalFileStorageService : IFileStorageService
```
**Features:**
- Development/testing file storage
- Tenant-based folder structure
- File name sanitization
- Relative path management

### 5. Background Services

#### Maintenance Reminder Service
```csharp
public class AssetMaintenanceReminderService : BackgroundService
```
**Capabilities:**
- Automatic maintenance reminders
- Overdue notification handling
- Configurable reminder intervals
- Event-driven notifications

#### Depreciation Calculation Service
```csharp
public class AssetDepreciationCalculationService : BackgroundService
```
**Depreciation Methods:**
- Straight-line depreciation
- Declining balance depreciation
- Double declining balance
- Sum of years digits
- Units of production (planned)

### 6. Business Services

#### Asset Tag Generator (`AssetTagGeneratorService`)
```csharp
public class AssetTagGeneratorService : IAssetTagGeneratorService
```
**Features:**
- Category-based prefixes
- Sequential numbering
- Uniqueness validation
- QR code generation support
- Barcode generation support

#### Depreciation Service (`AssetDepreciationService`)
```csharp
public class AssetDepreciationService : IAssetDepreciationService
```
**Calculations:**
- Multiple depreciation methods
- Current value calculations
- Monthly/annual depreciation
- Configurable parameters

### 7. Migration Support

#### Migration Helper (`MigrationHelper`)
**Features:**
- Seed data for categories and locations
- Index configuration with fill factors
- Database optimization setup
- Trigger configuration
- Tenant data seeding

## Configuration

### Dependency Injection (`DependencyInjection.cs`)
```csharp
services.AddAssetsInfrastructure(configuration, connectionString);
```

**Configured Services:**
- DbContext with connection resilience
- DbContext factory for advanced scenarios
- Repository pattern services
- File storage services (Azure Blob or Local)
- Background services
- Health checks

### Health Checks
- Database connectivity
- Azure Blob Storage availability
- Repository functionality

## Performance Optimizations

### 1. Indexing Strategy
- **Fill Factors**: 90% for normal operations, 80% for high-insert tables
- **Composite Indexes**: Tenant-aware multi-column indexes
- **Covering Indexes**: Include frequently accessed columns

### 2. Query Optimization
- **Global Query Filters**: Automatic tenant filtering
- **Query Splitting**: EF Core 9 split query behavior
- **Bulk Operations**: ExecuteUpdate/ExecuteDelete for performance
- **Specification Pattern**: Reusable, testable query logic

### 3. Connection Management
- **Connection Resilience**: Automatic retry policies
- **Connection Pooling**: Optimized for multi-tenant scenarios
- **Command Timeout**: Configurable for long-running operations

## Security Features

### 1. Multi-Tenancy
- Global query filters prevent cross-tenant data access
- Automatic tenant ID assignment
- Tenant verification in file operations

### 2. File Storage Security
- Tenant-isolated containers/folders
- SAS URLs with expiration
- Secure file name handling
- Metadata validation

### 3. Audit Trail
- Comprehensive audit logging
- Temporal tables for history
- User tracking for all operations
- Change detection and logging

## Usage Examples

### 1. Basic Asset Operations
```csharp
// Create asset
var asset = new Asset { Name = "Laptop", AssetTag = "COMP001234" };
await _assetRepository.CreateAsync(asset);
await _unitOfWork.SaveChangesAsync();

// Search assets
var searchSpec = new AssetSearchSpecification("laptop", AssetStatus.InUse);
var assets = await _assetRepository.GetBySpecificationAsync(searchSpec);
```

### 2. File Operations
```csharp
// Upload document
using var fileStream = File.OpenRead("document.pdf");
var filePath = await _fileStorageService.UploadFileAsync(
    fileStream, "asset-manual.pdf", "application/pdf");

// Generate secure URL
var secureUrl = await _fileStorageService.GetFileUrlAsync(
    filePath, TimeSpan.FromHours(1));
```

### 3. Depreciation Calculation
```csharp
// Calculate current depreciation
var depreciation = _depreciationService.CalculateDepreciation(
    purchasePrice: 5000m,
    salvageValue: 500m,
    usefulLifeYears: 5,
    purchaseDate: DateTime.Parse("2020-01-01"),
    calculationDate: DateTime.UtcNow,
    method: "StraightLine"
);
```

## Migration and Deployment

### 1. Database Migrations
```bash
# Create migration
dotnet ef migrations add InitialAssetSchema

# Update database
dotnet ef database update
```

### 2. Tenant Setup
```csharp
// Seed tenant-specific data
await context.SeedTenantData(tenantId);
```

### 3. Health Check Monitoring
```csharp
// Health check endpoints
app.MapHealthChecks("/health/assets");
```

## Event-Driven Architecture

### Published Events
- `AssetDepreciationCalculatedEvent` - When depreciation is calculated
- `MaintenanceReminderEvent` - When maintenance reminder is sent
- `MaintenanceOverdueEvent` - When maintenance becomes overdue

### Event Handlers
Events are published through MediatR and can be handled by:
- Notification services
- Reporting systems
- Integration services
- Audit systems

## Future Enhancements

### Planned Features
1. **Geolocation Services** - GPS tracking for mobile assets
2. **IoT Integration** - Sensor data collection and monitoring
3. **AI-Powered Insights** - Predictive maintenance algorithms
4. **Advanced Reporting** - Custom report generation
5. **Blockchain Integration** - Immutable asset provenance

### Performance Improvements
1. **Read Replicas** - Separate read/write operations
2. **Caching Layer** - Redis integration for frequently accessed data
3. **Event Sourcing** - Complete audit trail through events
4. **CQRS Optimization** - Separate read/write models

This Infrastructure layer provides a robust, scalable, and secure foundation for asset management operations in the TOSS ERP system, leveraging the latest EF Core 9 features and best practices for multi-tenant applications.
