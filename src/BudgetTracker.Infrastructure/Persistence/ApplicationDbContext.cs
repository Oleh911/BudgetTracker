using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<BudgetOperation> BudgetOperations => Set<BudgetOperation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // budgets
        modelBuilder.Entity<Budget>(b =>
        {
            b.ToTable("budgets");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(150).IsRequired();
            b.Property(x => x.AllocatedAmount).HasColumnType("numeric(14,2)").IsRequired();
            b.Property(x => x.Currency).HasConversion<string>().HasMaxLength(8).IsRequired();
            b.Property(x => x.Note).HasColumnType("text");
            b.Property(x => x.PeriodStart).IsRequired();
            b.Property(x => x.PeriodEnd).IsRequired();
            b.Property(x => x.IsArchived).IsRequired();
            b.Property(x => x.DisplayOrder).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
            b.HasIndex(x => new { x.Name, x.PeriodStart, x.PeriodEnd }).IsUnique();
            b.HasIndex(x => new { x.PeriodStart, x.PeriodEnd });
            b.HasIndex(x => x.Currency);
        });

        // categories
        modelBuilder.Entity<Category>(c =>
        {
            c.ToTable("categories");
            c.HasKey(x => x.Id);
            c.Property(x => x.Name).HasMaxLength(100).IsRequired();
            c.Property(x => x.Kind).HasConversion<string>().HasMaxLength(16).IsRequired();
            c.Property(x => x.Color).HasMaxLength(32);
            c.Property(x => x.Icon).HasMaxLength(64);
            c.Property(x => x.DisplayOrder).IsRequired();
            c.Property(x => x.IsArchived).IsRequired();
            c.Property(x => x.CreatedAt).IsRequired();
            c.Property(x => x.UpdatedAt).IsRequired();
            c.HasIndex(x => new { x.Kind, x.Name }).IsUnique();
            c.HasIndex(x => x.Kind);
        });

        // subcategories
        modelBuilder.Entity<Subcategory>(s =>
        {
            s.ToTable("subcategories");
            s.HasKey(x => x.Id);
            s.Property(x => x.Name).HasMaxLength(100).IsRequired();
            s.HasIndex(x => new { x.CategoryId, x.Name }).IsUnique();
            s.HasIndex(x => x.CategoryId);
            s.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            s.Property(x => x.CreatedAt).IsRequired();
            s.Property(x => x.UpdatedAt).IsRequired();
        });

        // budget_operations
        modelBuilder.Entity<BudgetOperation>(o =>
        {
            o.ToTable("budget_operations");
            o.HasKey(x => x.Id);
            o.Property(x => x.Kind).HasConversion<string>().HasMaxLength(16).IsRequired();

            o.Property(x => x.Amount).HasColumnType("numeric(14,2)");
            o.Property(x => x.DebitAmount).HasColumnType("numeric(14,2)");
            o.Property(x => x.CreditAmount).HasColumnType("numeric(14,2)");

            o.Property(x => x.Note).HasColumnType("text");

            o.Property(x => x.OccurredAt).IsRequired();
            o.Property(x => x.CreatedAt).IsRequired();
            o.Property(x => x.UpdatedAt).IsRequired();

            o.HasIndex(x => new { x.Kind, x.OccurredAt });
            o.HasIndex(x => new { x.BudgetId, x.OccurredAt });
            o.HasIndex(x => new { x.SourceBudgetId, x.OccurredAt });
            o.HasIndex(x => new { x.TargetBudgetId, x.OccurredAt });
            o.HasIndex(x => new { x.SubcategoryId, x.OccurredAt });
            o.HasIndex(x => x.OccurredAt);

            o.HasOne<Budget>().WithMany().HasForeignKey(x => x.BudgetId).OnDelete(DeleteBehavior.Restrict);
            o.HasOne<Budget>().WithMany().HasForeignKey(x => x.SourceBudgetId).OnDelete(DeleteBehavior.Restrict);
            o.HasOne<Budget>().WithMany().HasForeignKey(x => x.TargetBudgetId).OnDelete(DeleteBehavior.Restrict);
            o.HasOne<Subcategory>().WithMany().HasForeignKey(x => x.SubcategoryId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
