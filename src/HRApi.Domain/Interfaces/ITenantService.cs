using HRApi.Domain.Entities.Master;

namespace HRApi.Domain.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetCurrentTenantAsync();
    Task<string> GetCurrentTenantIdAsync();
    Task<string> GetTenantConnectionStringAsync(string tenantId);
    Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
    Task<Tenant?> GetTenantByIdAsync(string tenantId);
    Task<bool> IsTenantActiveAsync(string tenantId);
}
