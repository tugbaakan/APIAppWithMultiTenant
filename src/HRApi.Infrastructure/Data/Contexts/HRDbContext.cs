using HRApi.Domain.Common;
using HRApi.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRApi.Infrastructure.Data.Contexts;

public class HRDbContext : DbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<LeaveBalance> LeaveBalances { get; set; }
    public DbSet<Payslip> Payslips { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply soft delete filter globally
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var deletedCheck = Expression.Lambda(
                    Expression.Equal(Expression.Property(parameter, "IsDeleted"), Expression.Constant(false)),
                    parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);
            }
        }

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Code).HasMaxLength(10);
            
            entity.HasOne(e => e.ParentDepartment)
                  .WithMany(d => d.SubDepartments)
                  .HasForeignKey(e => e.ParentDepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Manager)
                  .WithMany()
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Position configuration
        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.MinSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MaxSalary).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Position)
                  .WithMany(p => p.Employees)
                  .HasForeignKey(e => e.PositionId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Manager)
                  .WithMany(m => m.Subordinates)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // LeaveType configuration
        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(10);
            
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // LeaveRequest configuration
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.ApprovalComments).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.LeaveRequests)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.LeaveType)
                  .WithMany(lt => lt.LeaveRequests)
                  .HasForeignKey(e => e.LeaveTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.ApprovedByEmployee)
                  .WithMany()
                  .HasForeignKey(e => e.ApprovedBy)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasOne(e => e.RejectedByEmployee)
                  .WithMany()
                  .HasForeignKey(e => e.RejectedBy)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // LeaveBalance configuration
        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.LeaveBalances)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.LeaveType)
                  .WithMany(lt => lt.LeaveBalances)
                  .HasForeignKey(e => e.LeaveTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.Year }).IsUnique();
        });

        // Payslip configuration
        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Allowances).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Overtime).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Bonus).HasColumnType("decimal(18,2)");
            entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxDeduction).HasColumnType("decimal(18,2)");
            entity.Property(e => e.InsuranceDeduction).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OtherDeductions).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalDeductions).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Payslips)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => new { e.EmployeeId, e.Year, e.Month }).IsUnique();
        });

        // Evaluation configuration
        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OverallScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.TechnicalSkillsScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CommunicationScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.TeamworkScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.LeadershipScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Strengths).HasMaxLength(2000);
            entity.Property(e => e.AreasForImprovement).HasMaxLength(2000);
            entity.Property(e => e.Goals).HasMaxLength(2000);
            entity.Property(e => e.EmployeeComments).HasMaxLength(2000);
            entity.Property(e => e.EvaluatorComments).HasMaxLength(2000);
            
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Evaluations)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Evaluator)
                  .WithMany()
                  .HasForeignKey(e => e.EvaluatorId)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = GetCurrentUser(); // You'll implement this
            }
            
            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = GetCurrentUser(); // You'll implement this
            }
        }
    }

    private string? GetCurrentUser()
    {
        // TODO: Implement getting current user from HttpContext or JWT
        return "system"; // Placeholder
    }
}
