# Multi-Tenant HR API

A comprehensive HR management API built with .NET 8, Entity Framework Core, and SQL Server, designed with multi-tenancy support using the "Database Per Tenant" approach.

## 🏗️ Architecture Overview

The application follows Clean Architecture principles with the following layers:

### 📁 Project Structure

```
src/
├── HRApi.Web/                     # Web API Layer (Controllers, Middleware)
├── HRApi.Application/             # Application Layer (Services, DTOs, Validators)
├── HRApi.Domain/                  # Domain Layer (Entities, Interfaces, Enums)
├── HRApi.Infrastructure/          # Infrastructure Layer (EF Context, Repositories)
└── HRApi.Shared/                  # Shared Components (Constants, Extensions)
```

### 🏢 Multi-Tenant Strategy

**Database Per Tenant**: Each customer has their own isolated database.

**Benefits:**
- Complete data isolation between tenants
- Easy to customize features per tenant
- Better performance (no query filtering overhead)
- Easier compliance with data regulations
- Simple backup/restore per customer

**Tenant Resolution Methods:**
1. **Custom Header**: `X-Tenant-Id: {tenant-id}`
2. **Subdomain**: `customer1.hrapi.com`
3. **JWT Claims**: Tenant information in authentication token

## 🚀 Features

### 👥 Employee Management
- Create, read, update, delete employees
- Employee hierarchy (manager-subordinate relationships)
- Department and position assignments
- Employment status tracking

### 🏖️ Leave Management
- Multiple leave types (Annual, Sick, Personal, Maternity)
- Leave request creation and approval workflow
- Leave balance tracking
- Leave history and reporting

### 💰 Payroll Management (Planned)
- Salary management
- Payslip generation
- Deductions and bonuses

### 📊 Performance Management (Planned)
- Employee evaluations
- Goal setting and tracking
- Performance reviews

## 🛠️ Technology Stack

- **.NET 8**: Latest LTS version
- **Entity Framework Core 8**: ORM with multi-tenant support
- **SQL Server**: Database engine
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **Swagger/OpenAPI**: API documentation
- **Serilog**: Structured logging (planned)

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd APIAppWithMultiTenant
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update connection strings**
   
   Edit `src/HRApi.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "MasterConnection": "Server=(localdb)\\mssqllocaldb;Database=HRApi_Master;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Create and update master database**
   ```bash
   dotnet ef database update --project src/HRApi.Infrastructure --startup-project src/HRApi.Web --context MasterDbContext
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/HRApi.Web
   ```

6. **Access Swagger UI**
   
   Navigate to `https://localhost:5043` to explore the API documentation.

## 🏢 Tenant Management

### Creating a New Tenant

1. **Manual Database Creation** (for now):
   ```sql
   CREATE DATABASE HRApi_Tenant_Customer1;
   ```

2. **Insert Tenant Record**:
   ```sql
   INSERT INTO HRApi_Master.dbo.Tenants (Id, Name, Subdomain, ConnectionString, IsActive, CreatedAt)
   VALUES (NEWID(), 'Customer 1', 'customer1', 'Server=(localdb)\mssqllocaldb;Database=HRApi_Tenant_Customer1;Trusted_Connection=true;MultipleActiveResultSets=true', 1, GETUTCDATE());
   ```

3. **Apply Migrations to Tenant Database**:
   ```bash
   dotnet ef database update --project src/HRApi.Infrastructure --startup-project src/HRApi.Web --context HRDbContext --connection-string "Server=(localdb)\mssqllocaldb;Database=HRApi_Tenant_Customer1;Trusted_Connection=true;MultipleActiveResultSets=true"
   ```

### Using the API with Tenants

Include the tenant header in your API requests:
```
X-Tenant-Id: {tenant-guid}
```

## 📚 API Endpoints

### Employee Management
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `GET /api/employees/by-number/{employeeNumber}` - Get employee by number
- `GET /api/employees/by-department/{departmentId}` - Get employees by department
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Leave Management
- `GET /api/leaverequests` - Get all leave requests
- `GET /api/leaverequests/{id}` - Get leave request by ID
- `GET /api/leaverequests/by-employee/{employeeId}` - Get leave requests by employee
- `GET /api/leaverequests/by-status/{status}` - Get leave requests by status
- `POST /api/leaverequests` - Create new leave request
- `POST /api/leaverequests/{id}/approve` - Approve leave request
- `POST /api/leaverequests/{id}/reject` - Reject leave request
- `POST /api/leaverequests/{id}/cancel` - Cancel leave request
- `DELETE /api/leaverequests/{id}` - Delete leave request

## 🗂️ Database Schema

### Master Database
- **Tenants**: Tenant metadata and configuration
- **UserTenants**: User-tenant relationships

### Tenant Databases
- **Employees**: Employee information
- **Departments**: Organizational departments
- **Positions**: Job positions
- **LeaveTypes**: Types of leave available
- **LeaveRequests**: Leave requests and approvals
- **LeaveBalances**: Employee leave balances
- **Payslips**: Payroll information
- **Evaluations**: Performance evaluations

## 🔒 Security Considerations

- **Data Isolation**: Each tenant has a separate database
- **Connection String Security**: Encrypted and securely stored
- **Tenant Validation**: Always validate tenant access rights
- **Audit Logging**: Track all tenant activities
- **Rate Limiting**: Implement per-tenant rate limiting (planned)

## 🚧 Future Enhancements

- [ ] Authentication & Authorization (JWT/OAuth)
- [ ] Role-based access control per tenant
- [ ] Automated tenant provisioning API
- [ ] Tenant analytics dashboard
- [ ] Background job processing
- [ ] Email notifications
- [ ] Advanced reporting
- [ ] Mobile API optimization
- [ ] Caching strategies
- [ ] Performance monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support, please contact [your-email@example.com] or create an issue in the repository.
