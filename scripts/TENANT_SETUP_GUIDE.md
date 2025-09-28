# Multi-Tenant Database Setup Guide

This guide will help you set up the tenant databases for your HR API multi-tenant application.

## Prerequisites

- SQL Server instance running (.\MSSQLSERVER01)
- Master database (HRApi_Master) already created and migrated
- Tenant records already inserted in the Master database
- .NET 8 SDK installed
- Entity Framework Core tools installed

## Configuration

The tenant connection strings are now configured in `src/HRApi.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MasterConnection": "Server=.\\MSSQLSERVER01;Database=HRApi_Master;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "Company1Connection": "Server=.\\MSSQLSERVER01;Database=HRApi_Company1;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "Company2Connection": "Server=.\\MSSQLSERVER01;Database=HRApi_Company2;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

This approach provides several benefits:
- ✅ No need to specify connection strings in command line
- ✅ Centralized configuration management
- ✅ Environment-specific configurations (Development, Production)
- ✅ Easy to maintain and update

## Current Tenant Configuration

Based on your `insert_tenant_seed_data.sql`, you have two tenants configured:

| Tenant ID | Name | Subdomain | Database |
|-----------|------|-----------|----------|
| 580874a2-1657-49ba-94f5-ec5f2ec3c306 | Company1 | Company1 | HRApi_Company1 |
| 4b9d09a2-0d62-49bf-9c4e-abbb87619732 | Company2 | Company2 | HRApi_Company2 |

## Step-by-Step Setup Process

### Step 1: Create Tenant Databases

Execute the database creation scripts to create the physical databases:

```bash
# For Company1
sqlcmd -S .\MSSQLSERVER01 -E -i scripts/migrate_company1_database.sql

# For Company2
sqlcmd -S .\MSSQLSERVER01 -E -i scripts/migrate_company2_database.sql
```

### Step 2: Apply EF Core Migrations to Tenant Databases

Navigate to your project root directory and run the migration scripts. The connection strings are now configured in `appsettings.json`.

#### Option A: Using PowerShell Scripts (Recommended)
```powershell
# Apply migrations to Company1 database
.\scripts\migrate-company1.ps1

# Apply migrations to Company2 database
.\scripts\migrate-company2.ps1
```

#### Option B: Using Batch Files (Windows CMD)
```cmd
# Apply migrations to Company1 database
scripts\migrate-company1.bat

# Apply migrations to Company2 database
scripts\migrate-company2.bat
```

#### Option C: Manual Commands with Tenant Arguments
```bash
# For Company1 - specify tenant directly in command
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1

# For Company2 - specify tenant directly in command
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2
```

#### Option D: Using Environment Variables (Legacy/CI-CD)
```bash
# For Company1
set EF_TENANT=Company1
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web

# For Company2
set EF_TENANT=Company2
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web
```

### Step 3: Seed Tenant Data

Execute the seed data scripts to populate each tenant database with initial data:

```bash
# Seed Company1 data
sqlcmd -S .\MSSQLSERVER01 -E -i scripts/seed_company1_data.sql

# Seed Company2 data
sqlcmd -S .\MSSQLSERVER01 -E -i scripts/seed_company2_data.sql
```

## Verification Steps

### 1. Verify Database Creation

```sql
-- Check if databases exist
SELECT name FROM sys.databases WHERE name IN ('HRApi_Company1', 'HRApi_Company2');
```

### 2. Verify Schema Migration

```sql
-- Check tables in Company1
USE HRApi_Company1;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check tables in Company2
USE HRApi_Company2;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
```

### 3. Verify Seed Data

```sql
-- Check Company1 data
USE HRApi_Company1;
SELECT 'Departments' as DataType, COUNT(*) as Count FROM Departments WHERE IsDeleted = 0
UNION ALL
SELECT 'Positions', COUNT(*) FROM Positions WHERE IsDeleted = 0
UNION ALL
SELECT 'Employees', COUNT(*) FROM Employees WHERE IsDeleted = 0
UNION ALL
SELECT 'LeaveTypes', COUNT(*) FROM LeaveTypes WHERE IsDeleted = 0
UNION ALL
SELECT 'LeaveBalances', COUNT(*) FROM LeaveBalances WHERE IsDeleted = 0;

-- Check Company2 data
USE HRApi_Company2;
SELECT 'Departments' as DataType, COUNT(*) as Count FROM Departments WHERE IsDeleted = 0
UNION ALL
SELECT 'Positions', COUNT(*) FROM Positions WHERE IsDeleted = 0
UNION ALL
SELECT 'Employees', COUNT(*) FROM Employees WHERE IsDeleted = 0
UNION ALL
SELECT 'LeaveTypes', COUNT(*) FROM LeaveTypes WHERE IsDeleted = 0
UNION ALL
SELECT 'LeaveBalances', COUNT(*) FROM LeaveBalances WHERE IsDeleted = 0;
```

## Expected Results

After successful setup, you should have:

### Company1 Database:
- 4 Departments (HR, IT, Finance, Marketing)
- 5 Positions
- 5 Leave Types
- 5 Employees
- 25 Leave Balance records (5 employees × 5 leave types)

### Company2 Database:
- 5 Departments (HR, Technology, Operations, Sales, Customer Support)
- 6 Positions
- 6 Leave Types
- 6 Employees
- 36 Leave Balance records (6 employees × 6 leave types)

## Testing the Multi-Tenant API

### Using Swagger UI

1. Start your application: `dotnet run --project src/HRApi.Web`
2. Navigate to: `https://localhost:7xxx` (check your launchSettings.json for the exact port)
3. In Swagger UI, use the "X-Tenant-Id" header with one of these values:
   - `580874a2-1657-49ba-94f5-ec5f2ec3c306` (Company1)
   - `4b9d09a2-0d62-49bf-9c4e-abbb87619732` (Company2)

### Using HTTP Requests

```http
# Get Company1 employees
GET https://localhost:7xxx/api/employees
X-Tenant-Id: 580874a2-1657-49ba-94f5-ec5f2ec3c306

# Get Company2 employees
GET https://localhost:7xxx/api/employees
X-Tenant-Id: 4b9d09a2-0d62-49bf-9c4e-abbb87619732
```

## Troubleshooting

### Common Issues:

1. **Migration fails**: Ensure the database exists before running migrations
2. **Seed data fails**: Check if tables exist and have the correct schema
3. **Tenant not found**: Verify tenant IDs match exactly in the master database
4. **Connection issues**: Verify SQL Server instance name and authentication

### Useful Commands:

```bash
# Check EF migrations status for Company1
dotnet ef migrations list --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1

# Check EF migrations status for Company2
dotnet ef migrations list --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2

# Generate new migration (applies to all tenants)
dotnet ef migrations add MigrationName --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web

# Remove last migration (applies to all tenants)
dotnet ef migrations remove --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web

# Alternative: Using environment variables (for CI/CD or scripting)
set EF_TENANT=Company1
dotnet ef migrations list --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web
```

### Design-Time Factory

The project now includes a `HRDbContextDesignTimeFactory` that:
- Reads connection strings from `appsettings.json`
- **Prioritizes command-line arguments** for tenant selection (recommended)
- Supports both `--tenant=TenantName` and `--tenant TenantName` formats
- Falls back to `EF_TENANT` environment variable for CI/CD scenarios
- Defaults to Company1 if no tenant is specified
- Provides clear console output showing which tenant is being used

**Command-line argument formats supported:**
```bash
# Format 1: --tenant=Company1
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1

# Format 2: --tenant Company1 (space separated)
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant Company1
```

## Notes

- Each tenant has its own isolated database with customized data
- Company1 focuses on traditional corporate structure
- Company2 has a more modern tech-oriented structure
- Leave policies differ between tenants to demonstrate multi-tenant flexibility
- All tenant databases share the same schema but contain different data

## Next Steps

After successful setup, you can:
1. Test the API endpoints with different tenant headers
2. Add more employees, departments, or leave requests through the API
3. Implement additional tenant-specific configurations
4. Set up automated tenant provisioning if needed
