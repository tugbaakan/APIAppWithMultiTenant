using HRApi.Application.Interfaces;
using HRApi.Application.Mappings;
using HRApi.Application.Services;
using HRApi.Application.Validators;
using HRApi.Domain.Interfaces;
using HRApi.Infrastructure.Data.Contexts;
using HRApi.Infrastructure.Data.Repositories;
using HRApi.Infrastructure.MultiTenant;
using HRApi.Web.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeDtoValidator>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(EmployeeProfile), typeof(LeaveProfile));

// Add Entity Framework contexts
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MasterConnection")));

// Add HTTP context accessor for tenant resolution
builder.Services.AddHttpContextAccessor();

// Add memory cache for tenant caching
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<TenantDbContextFactory>();

// Register tenant-aware services using factory pattern
builder.Services.AddScoped<HRDbContext>(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<TenantDbContextFactory>();
    
    try 
    {
        return factory.CreateDbContextAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating tenant context: {ex.Message}");
        // For now, return a context with default connection for debugging
        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=HRApi_Tenant_Demo;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");
        return new HRDbContext(optionsBuilder.Options);
    }
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR API", Version = "v1", Description = "Multi-tenant HR Management API" });
    
    // Add custom header for tenant ID
    c.OperationFilter<TenantHeaderOperationFilter>();
    
    // Include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at apps root
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Add tenant resolution middleware
app.UseMiddleware<TenantResolutionMiddleware>();

// Add authentication and authorization (to be implemented later)
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
