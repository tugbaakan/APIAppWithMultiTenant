using HRApi.Domain.Entities.Master;
using HRApi.Domain.Interfaces;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HRApi.Infrastructure.MultiTenant;

public class TenantService : ITenantService
{
    private readonly MasterDbContext _masterContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;
    private const string TenantCacheKeyPrefix = "tenant_";
    private const int CacheExpirationMinutes = 30;

    public TenantService(
        MasterDbContext masterContext, 
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
    {
        _masterContext = masterContext;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<Tenant?> GetCurrentTenantAsync()
    {
        var tenantId = await GetCurrentTenantIdAsync();
        if (string.IsNullOrEmpty(tenantId))
            return null;

        return await GetTenantByIdAsync(tenantId);
    }

    public async Task<string> GetCurrentTenantIdAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return string.Empty;

        // Try to get tenant from custom header first
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            return tenantIdHeader.FirstOrDefault() ?? string.Empty;
        }

        // Try to get tenant from subdomain
        var host = httpContext.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);
        
        if (!string.IsNullOrEmpty(subdomain))
        {
            var tenant = await GetTenantBySubdomainAsync(subdomain);
            return tenant?.Id.ToString() ?? string.Empty;
        }

        // Try to get tenant from JWT claims
        var tenantClaim = httpContext.User?.FindFirst("tenant_id");
        if (tenantClaim != null)
        {
            return tenantClaim.Value;
        }

        return string.Empty;
    }

    public async Task<string> GetTenantConnectionStringAsync(string tenantId)
    {
        var tenant = await GetTenantByIdAsync(tenantId);
        return tenant?.ConnectionString ?? string.Empty;
    }

    public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
    {
        var cacheKey = $"{TenantCacheKeyPrefix}subdomain_{subdomain}";
        
        if (_cache.TryGetValue(cacheKey, out Tenant? cachedTenant))
        {
            return cachedTenant;
        }

        var tenant = await _masterContext.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive);

        if (tenant != null)
        {
            _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(CacheExpirationMinutes));
        }

        return tenant;
    }

    public async Task<Tenant?> GetTenantByIdAsync(string tenantId)
    {
        if (!Guid.TryParse(tenantId, out var guid))
            return null;

        var cacheKey = $"{TenantCacheKeyPrefix}id_{tenantId}";
        
        if (_cache.TryGetValue(cacheKey, out Tenant? cachedTenant))
        {
            return cachedTenant;
        }

        var tenant = await _masterContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == guid && t.IsActive);

        if (tenant != null)
        {
            _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(CacheExpirationMinutes));
        }

        return tenant;
    }

    public async Task<bool> IsTenantActiveAsync(string tenantId)
    {
        var tenant = await GetTenantByIdAsync(tenantId);
        return tenant?.IsActive ?? false;
    }

    private static string ExtractSubdomain(string host)
    {
        if (string.IsNullOrEmpty(host))
            return string.Empty;

        var parts = host.Split('.');
        if (parts.Length >= 3)
        {
            // Assuming format: subdomain.domain.com
            return parts[0];
        }

        return string.Empty;
    }
}
