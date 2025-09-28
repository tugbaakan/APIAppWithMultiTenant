# Tenant Type Architecture: Hybrid Approach

## Overview

This document explains the hybrid tenant management approach that combines the flexibility of database-driven tenant management with the type safety of enums for business logic decisions.

## Problem Statement

The original implementation used string comparisons for tenant-specific business logic:

```csharp
// ❌ Problematic: Runtime errors, no compile-time safety
if (currentTenant.Name.Equals("Company2", StringComparison.OrdinalIgnoreCase))
{
    employees = employees.Where(e => e.IsDepartmentManager);
}
```

**Issues with string-based approach:**
- Runtime errors due to typos
- No compile-time validation
- Hard to refactor
- Poor IDE support

## Solution: Hybrid Approach

### 1. TenantType Enum

```csharp
public enum TenantType
{
    Standard = 1,    // Full employee access
    Restricted = 2,  // Managers only
    Enterprise = 3,  // Advanced features
    Trial = 4        // Limited features
}
```

### 2. Enhanced Tenant Entity

```csharp
public class Tenant : BaseEntity
{
    // ... existing properties ...
    
    /// <summary>
    /// The type of tenant, used for business logic decisions
    /// </summary>
    public TenantType TenantType { get; set; } = TenantType.Standard;
    
    // ... rest of properties ...
}
```

### 3. Type-Safe Business Logic

```csharp
// ✅ Improved: Compile-time safety, easy to refactor
var currentTenantType = await _tenantService.GetCurrentTenantTypeAsync();
if (currentTenantType.HasValue)
{
    switch (currentTenantType.Value)
    {
        case TenantType.Restricted:
            employees = employees.Where(e => e.IsDepartmentManager);
            break;
        case TenantType.Standard:
        case TenantType.Enterprise:
        case TenantType.Trial:
        default:
            // No additional filtering
            break;
    }
}
```

## Benefits of Hybrid Approach

### ✅ Advantages

1. **Type Safety**: Compile-time checking prevents typos and runtime errors
2. **Maintainability**: Easy to refactor and add new tenant types
3. **Performance**: Cached tenant lookups with enum-based logic
4. **Flexibility**: Still supports dynamic tenant management via database
5. **Scalability**: Can handle hundreds of tenants with different types
6. **IDE Support**: IntelliSense, refactoring tools work perfectly
7. **Documentation**: Self-documenting code with enum values

### 🔄 Comparison with Alternatives

| Approach | Type Safety | Flexibility | Performance | Scalability |
|----------|-------------|-------------|-------------|-------------|
| **Hybrid (Recommended)** | ✅ | ✅ | ✅ | ✅ |
| Pure Enum | ✅ | ❌ | ✅ | ❌ |
| Pure Database/String | ❌ | ✅ | ⚠️ | ✅ |

## Implementation Details

### Database Schema

```sql
-- Migration adds TenantType column
ALTER TABLE Tenants ADD TenantType int NOT NULL DEFAULT 1;
```

### Current Tenant Mapping

| Tenant | Name | TenantType | Business Logic |
|--------|------|------------|----------------|
| Company1 | Company1 | Standard (1) | Shows all employees |
| Company2 | Company2 | Restricted (2) | Shows only managers |

### Service Layer Enhancement

```csharp
public interface ITenantService
{
    // ... existing methods ...
    Task<TenantType?> GetCurrentTenantTypeAsync();
}
```

## Migration Guide

### Step 1: Apply Database Migration

```bash
# Apply the migration to add TenantType column
dotnet ef database update --context MasterDbContext
```

### Step 2: Update Existing Data

```sql
-- Run the update script
sqlcmd -S .\MSSQLSERVER01 -E -i scripts/update_tenant_types.sql
```

### Step 3: Deploy Code Changes

The code changes are backward compatible and will work immediately after deployment.

## Usage Examples

### Adding New Tenant Types

1. **Add to Enum**:
```csharp
public enum TenantType
{
    Standard = 1,
    Restricted = 2,
    Enterprise = 3,
    Trial = 4,
    Premium = 5  // New type
}
```

2. **Update Business Logic**:
```csharp
switch (currentTenantType.Value)
{
    case TenantType.Premium:
        // Premium-specific logic
        employees = employees.Where(e => e.Department.IsActive);
        break;
    // ... other cases
}
```

3. **Update Database**:
```sql
INSERT INTO Tenants (..., TenantType, ...) 
VALUES (..., 5, ...); -- Premium = 5
```

### Tenant-Specific Features

```csharp
public async Task<bool> CanAccessAdvancedReports()
{
    var tenantType = await _tenantService.GetCurrentTenantTypeAsync();
    return tenantType == TenantType.Enterprise || tenantType == TenantType.Premium;
}
```

## Best Practices

1. **Always use the enum** for business logic decisions
2. **Keep tenant names** for display purposes and external integrations
3. **Use switch statements** instead of if-else chains for better performance
4. **Document enum values** with XML comments
5. **Consider backward compatibility** when adding new enum values
6. **Use nullable checks** when working with tenant types

## Future Considerations

1. **Feature Flags**: Could be combined with tenant types for granular control
2. **Tenant Hierarchies**: Enum could support inheritance relationships
3. **Dynamic Configuration**: JSON settings could complement enum-based logic
4. **Multi-Dimensional Types**: Could extend to support multiple classification dimensions

## Conclusion

The hybrid approach provides the best of both worlds:
- **Database flexibility** for dynamic tenant management
- **Enum type safety** for reliable business logic
- **Performance optimization** through caching
- **Maintainable code** that's easy to understand and extend

This architecture is suitable for SaaS applications that need both scalability and reliability in their multi-tenant implementations.
