using HRApi.Domain.Interfaces;

namespace HRApi.Web.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Skip tenant validation for specific endpoints
        var skipTenantPaths = new[] { "/api/health", "/api/test", "/swagger", "/", "/favicon.ico" };
        if (skipTenantPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await _next(context);
            return;
        }

        try
        {
            var tenantId = await tenantService.GetCurrentTenantIdAsync();
            
            if (string.IsNullOrEmpty(tenantId))
            {
                _logger.LogWarning("No tenant context found for request: {RequestPath}", context.Request.Path);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Tenant context is required");
                return;
            }

            var isActive = await tenantService.IsTenantActiveAsync(tenantId);
            if (!isActive)
            {
                _logger.LogWarning("Inactive tenant attempted access: {TenantId}", tenantId);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Tenant is not active");
                return;
            }

            // Set tenant information in HttpContext for downstream components
            context.Items["TenantId"] = tenantId;
            
            _logger.LogDebug("Tenant resolved: {TenantId} for request: {RequestPath}", tenantId, context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant context");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
            return;
        }

        await _next(context);
    }
}
