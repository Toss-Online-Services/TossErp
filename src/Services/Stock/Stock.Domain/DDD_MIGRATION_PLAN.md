# Stock.Domain DDD Migration Plan

## Overview

This document outlines the migration of the Stock.Domain from a traditional entity-based approach to a proper Domain-Driven Design (DDD) implementation with aggregates, value objects, and domain events.

## Current State Analysis

### Issues Identified

1. **Missing Aggregate Boundaries**: Entities were not properly organized into aggregates
2. **No IAggregateRoot Interface**: Missing the interface that defines aggregate boundaries
3. **Entity vs Value Object Confusion**: Many entities that should be value objects
4. **Inconsistent Domain Event Usage**: Events scattered and not consistently applied
5. **Missing Invariants**: Business rules not properly encapsulated
6. **Repository Pattern Issues**: Generic repository doesn't align with aggregate boundaries

## Migration Strategy

Based on the [Strangler Fig pattern](https://ddd-practitioners.com/2023/08/02/from-big-bang-to-iterative-evolution-embracing-the-strangler-fig-pattern/) and [gradual migration approach](https://www.eventstore.com/blog/a-recipe-for-gradually-migrating-from-crud-to-event-sourcing), we're implementing a step-by-step migration.

## Phase 1: Core Infrastructure ✅

### Completed Changes

1. **IAggregateRoot Interface**
   - Created marker interface for aggregate roots
   - Defines clear aggregate boundaries

2. **Entity Base Class**
   - Proper domain event handling
   - Identity management with Guid
   - Equality comparison support

3. **Value Objects**
   - ItemCode, WarehouseCode, BinCode
   - Quantity, Rate, StockEntryNo
   - Immutable and self-validating

## Phase 2: Aggregate Structure ✅

### Item Aggregate

**Aggregate Root**: `ItemAggregate`
**Child Entities**:
- `ItemVariant` - Product variations (size, color, etc.)
- `ItemPrice` - Pricing information for different price lists
- `ItemSupplier` - Supplier relationships
- `ItemBarcode` - Barcode information
- `ItemTax` - Tax configurations
- `ItemReorder` - Reorder level settings
- `ItemAttribute` - Product attributes
- `ItemAlternative` - Alternative products
- `ItemManufacturer` - Manufacturer information
- `UOMConversionDetail` - Unit of measure conversions

**Key Business Rules**:
- Items cannot be modified when disabled/deleted
- Pricing must be non-negative
- Reorder levels must be non-negative
- Variants must have unique codes within the item
- Suppliers must be unique per item

### StockEntry Aggregate

**Aggregate Root**: `StockEntryAggregate`
**Child Entities**:
- `StockEntryDetail` - Individual stock movement line items
- `StockEntryAdditionalCost` - Additional costs for the entry

**Key Business Rules**:
- Cannot modify posted entries
- Must have at least one detail to post
- All details must be valid before posting
- Entry numbers must be unique

### Warehouse Aggregate

**Aggregate Root**: `WarehouseAggregate`
**Child Entities**:
- `Bin` - Storage locations within warehouse

**Key Business Rules**:
- Group warehouses cannot have bins
- Cannot add child warehouses to non-group warehouses
- Warehouse cannot be its own parent
- Tree structure must be valid (lft < rgt)

## Phase 3: Repository Pattern ✅

### Repository Interfaces

1. **IItemRepository**
   - Aggregate-specific queries
   - Business-focused methods
   - Proper separation of concerns

2. **IStockEntryRepository**
   - Entry number management
   - Posted/unposted filtering
   - Date range queries

3. **IWarehouseRepository**
   - Hierarchy management
   - Group/leaf warehouse queries
   - Location-based filtering

## Phase 4: Domain Events ✅

### Event Structure

Each aggregate root raises domain events for significant state changes:

- **Item Events**: Created, Updated, Disabled, Enabled, Deleted, Restored
- **StockEntry Events**: Created, Posted, Updated, Deleted
- **Warehouse Events**: Created, Updated, GroupSet, Disabled, Enabled

### Event Benefits

1. **Decoupling**: Handlers can react to domain changes
2. **Audit Trail**: Complete history of changes
3. **Integration**: Cross-aggregate communication
4. **CQRS Support**: Event sourcing ready

## Phase 5: Value Objects ✅

### Implemented Value Objects

1. **ItemCode** - Unique item identifier
2. **WarehouseCode** - Unique warehouse identifier
3. **BinCode** - Unique bin identifier
4. **Quantity** - Stock quantity with validation
5. **Rate** - Monetary rate with validation
6. **StockEntryNo** - Stock entry number with format validation

### Value Object Benefits

1. **Immutability**: Cannot be changed after creation
2. **Self-Validation**: Encapsulates validation logic
3. **Type Safety**: Prevents primitive obsession
4. **Domain Semantics**: Clear business meaning

## Migration Benefits

### 1. **Clear Aggregate Boundaries**
- Each aggregate has a single entry point
- Business rules are encapsulated
- Consistency is enforced

### 2. **Improved Business Logic**
- Invariants are properly enforced
- Complex business rules are centralized
- Domain events provide audit trail

### 3. **Better Testability**
- Aggregates can be tested in isolation
- Business rules are explicit
- Domain events can be verified

### 4. **Enhanced Maintainability**
- Clear separation of concerns
- Domain logic is centralized
- Changes are localized to aggregates

### 5. **Scalability**
- Aggregates can be distributed
- Event-driven architecture
- CQRS ready

## Next Steps

### Phase 6: Application Layer Integration
1. Update Application services to use new aggregates
2. Implement CQRS pattern
3. Add command/query handlers

### Phase 7: Infrastructure Layer
1. Update EF Core configurations
2. Implement repository implementations
3. Add event handlers

### Phase 8: Testing
1. Unit tests for aggregates
2. Integration tests for repositories
3. End-to-end tests for workflows

### Phase 9: Performance Optimization
1. Add snapshots for large aggregates
2. Implement caching strategies
3. Optimize queries

## Comparison with References

### vs nopCommerce
- **Similar**: Product-centric approach, rich domain model
- **Improvement**: Proper aggregate boundaries, domain events

### vs ERPNext
- **Similar**: Comprehensive stock management
- **Improvement**: Modern .NET patterns, type safety

### vs eShop
- **Similar**: Clean aggregate structure, domain events
- **Improvement**: ERP-specific business rules, value objects

## Conclusion

The migration to proper DDD patterns provides:

1. **Better Domain Modeling**: Clear aggregate boundaries and business rules
2. **Improved Maintainability**: Centralized business logic and domain events
3. **Enhanced Testability**: Isolated aggregates and explicit business rules
4. **Future-Proof Architecture**: Event-driven, CQRS-ready design

This foundation supports the TOSS ERP III vision of a third-generation ERP with collaborative economy features, AI integration, and modern microservices architecture. 