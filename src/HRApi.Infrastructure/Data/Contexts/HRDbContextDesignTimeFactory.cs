using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HRApi.Infrastructure.Data.Contexts;

/// <summary>
/// Design-time factory for HRDbContext to support EF Core migrations
/// This factory allows EF Core tools to create instances of HRDbContext during design-time operations
/// </summary>
public class HRDbContextDesignTimeFactory : IDesignTimeDbContextFactory<HRDbContext>
{
    public HRDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "HRApi.Web"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();

        // Determine which connection string to use
        string connectionStringName = "Company1Connection"; // Default to Company1
        string tenantName = "Company1"; // Default tenant
        
        // Priority 1: Check command line arguments for tenant specification
        if (args.Length > 0)
        {
            // Look for --tenant=TenantName argument
            var tenantArg = args.FirstOrDefault(arg => arg.StartsWith("--tenant="));
            if (tenantArg != null)
            {
                tenantName = tenantArg.Split('=')[1];
                connectionStringName = $"{tenantName}Connection";
            }
            // Also support -- TenantName format (space separated)
            else
            {
                var tenantIndex = Array.IndexOf(args, "--tenant");
                if (tenantIndex >= 0 && tenantIndex + 1 < args.Length)
                {
                    tenantName = args[tenantIndex + 1];
                    connectionStringName = $"{tenantName}Connection";
                }
            }
        }

        // Priority 2: Check environment variable for tenant selection (fallback for CI/CD)
        if (connectionStringName == "Company1Connection") // Only if not set by command line
        {
            var envTenant = Environment.GetEnvironmentVariable("EF_TENANT");
            if (!string.IsNullOrEmpty(envTenant))
            {
                tenantName = envTenant;
                connectionStringName = $"{envTenant}Connection";
            }
        }

        var connectionString = configuration.GetConnectionString(connectionStringName);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{connectionStringName}' not found in configuration. Available tenants: Company1, Company2");
        }

        Console.WriteLine($"Using tenant: {tenantName} with connection: {connectionStringName}");
        
        optionsBuilder.UseSqlServer(connectionString, b => 
            b.MigrationsAssembly("HRApi.Infrastructure")
             .MigrationsHistoryTable("__EFMigrationsHistory", "dbo"));

        return new HRDbContext(optionsBuilder.Options);
    }
}
