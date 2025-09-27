using HRApi.Domain.Entities.Master;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRApi.Infrastructure.Services;

public class TenantSetupService
{
    private readonly MasterDbContext _masterContext;
    private readonly ILogger<TenantSetupService> _logger;

    public TenantSetupService(MasterDbContext masterContext, ILogger<TenantSetupService> logger)
    {
        _masterContext = masterContext;
        _logger = logger;
    }

    public async Task<Tenant> CreateTenantAsync(string name, string subdomain, string connectionString)
    {
        _logger.LogInformation("Creating new tenant: {TenantName} with subdomain: {Subdomain}", name, subdomain);

        // Check if tenant already exists
        var existingTenant = await _masterContext.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain || t.Name == name);

        if (existingTenant != null)
        {
            throw new InvalidOperationException($"Tenant with subdomain '{subdomain}' or name '{name}' already exists");
        }

        var tenant = new Tenant
        {
            Name = name,
            Subdomain = subdomain,
            ConnectionString = connectionString,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _masterContext.Tenants.Add(tenant);
        await _masterContext.SaveChangesAsync();

        _logger.LogInformation("Tenant created successfully: {TenantId}", tenant.Id);

        return tenant;
    }

    public async Task<bool> CreateTenantDatabaseAsync(string connectionString)
    {
        try
        {
            _logger.LogInformation("Creating tenant database with connection: {ConnectionString}", 
                MaskConnectionString(connectionString));

            var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new HRDbContext(optionsBuilder.Options);
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Apply migrations
            await context.Database.MigrateAsync();

            _logger.LogInformation("Tenant database created and migrated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create tenant database");
            return false;
        }
    }

    public async Task<bool> SeedTenantDataAsync(string connectionString)
    {
        try
        {
            _logger.LogInformation("Seeding tenant database with initial data");

            var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new HRDbContext(optionsBuilder.Options);

            // Seed initial data if database is empty
            if (!await context.Departments.AnyAsync())
            {
                await SeedInitialDataAsync(context);
                _logger.LogInformation("Tenant database seeded successfully");
            }
            else
            {
                _logger.LogInformation("Tenant database already contains data, skipping seeding");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed tenant database");
            return false;
        }
    }

    private async Task SeedInitialDataAsync(HRDbContext context)
    {
        // Seed departments
        var departments = new[]
        {
            new Domain.Entities.HR.Department { Name = "Human Resources", Code = "HR", IsActive = true },
            new Domain.Entities.HR.Department { Name = "Information Technology", Code = "IT", IsActive = true },
            new Domain.Entities.HR.Department { Name = "Finance", Code = "FIN", IsActive = true },
            new Domain.Entities.HR.Department { Name = "Marketing", Code = "MKT", IsActive = true },
            new Domain.Entities.HR.Department { Name = "Sales", Code = "SAL", IsActive = true }
        };

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        // Seed positions
        var positions = new[]
        {
            new Domain.Entities.HR.Position { Title = "Software Developer", Code = "DEV", DepartmentId = departments[1].Id, IsActive = true },
            new Domain.Entities.HR.Position { Title = "HR Manager", Code = "HRM", DepartmentId = departments[0].Id, IsActive = true },
            new Domain.Entities.HR.Position { Title = "Financial Analyst", Code = "FA", DepartmentId = departments[2].Id, IsActive = true },
            new Domain.Entities.HR.Position { Title = "Marketing Specialist", Code = "MS", DepartmentId = departments[3].Id, IsActive = true },
            new Domain.Entities.HR.Position { Title = "Sales Representative", Code = "SR", DepartmentId = departments[4].Id, IsActive = true }
        };

        context.Positions.AddRange(positions);
        await context.SaveChangesAsync();

        // Seed leave types
        var leaveTypes = new[]
        {
            new Domain.Entities.HR.LeaveType { Name = "Annual Leave", Code = "AL", MaxDaysPerYear = 25, IsActive = true },
            new Domain.Entities.HR.LeaveType { Name = "Sick Leave", Code = "SL", MaxDaysPerYear = 10, IsActive = true },
            new Domain.Entities.HR.LeaveType { Name = "Personal Leave", Code = "PL", MaxDaysPerYear = 5, IsActive = true },
            new Domain.Entities.HR.LeaveType { Name = "Maternity Leave", Code = "ML", MaxDaysPerYear = 90, IsActive = true }
        };

        context.LeaveTypes.AddRange(leaveTypes);
        await context.SaveChangesAsync();
    }

    private static string MaskConnectionString(string connectionString)
    {
        // Simple masking for logging purposes
        if (string.IsNullOrEmpty(connectionString))
            return string.Empty;

        var parts = connectionString.Split(';');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                parts[i].Contains("Pwd", StringComparison.OrdinalIgnoreCase))
            {
                var equalIndex = parts[i].IndexOf('=');
                if (equalIndex > 0)
                {
                    parts[i] = parts[i].Substring(0, equalIndex + 1) + "***";
                }
            }
        }
        return string.Join(";", parts);
    }
}
