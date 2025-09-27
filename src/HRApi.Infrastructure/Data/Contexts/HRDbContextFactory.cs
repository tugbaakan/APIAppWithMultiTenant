using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HRApi.Infrastructure.Data.Contexts;

public class HRDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
{
    public HRDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        
        // Use a default connection string for design-time migrations
        // This will be replaced with tenant-specific connection strings at runtime
        optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=HRApi_Tenant_Demo;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");

        return new HRDbContext(optionsBuilder.Options);
    }
}
