using HRApi.Domain.Interfaces;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRApi.Infrastructure.MultiTenant;

public class TenantDbContextFactory
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantDbContextFactory> _logger;

    public TenantDbContextFactory(
        ITenantService tenantService,
        ILogger<TenantDbContextFactory> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<HRDbContext> CreateDbContextAsync()
    {
        var tenantId = await _tenantService.GetCurrentTenantIdAsync();
        
        if (string.IsNullOrEmpty(tenantId))
        {
            _logger.LogError("No tenant context found");
            throw new InvalidOperationException("No tenant context found");
        }

        var connectionString = await _tenantService.GetTenantConnectionStringAsync(tenantId);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("No connection string found for tenant {TenantId}", tenantId);
            throw new InvalidOperationException($"No connection string found for tenant {tenantId}");
        }

        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new HRDbContext(optionsBuilder.Options);
    }

    public HRDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new HRDbContext(optionsBuilder.Options);
    }
}
