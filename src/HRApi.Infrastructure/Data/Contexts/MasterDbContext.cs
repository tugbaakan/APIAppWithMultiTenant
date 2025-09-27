using HRApi.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Infrastructure.Data.Contexts;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<UserTenant> UserTenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant configuration
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ConnectionString).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(15);
            entity.Property(e => e.Settings).HasMaxLength(2000);
            
            entity.HasIndex(e => e.Subdomain).IsUnique();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserTenant configuration
        modelBuilder.Entity<UserTenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.UserTenants)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
        });
    }
}
