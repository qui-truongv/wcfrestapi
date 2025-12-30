using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using System.Reflection;

namespace QMS.Infrastructure.Data;

public class QMSDbContext : DbContext
{
    public QMSDbContext(DbContextOptions<QMSDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<QMS_CLIENT> QMS_CLIENT { get; set; }
    public DbSet<QMS_KIOSK> QMS_KIOSK { get; set; }
    public DbSet<QMS_KIOSK_QUEUE> QMS_KIOSK_QUEUE { get; set; }
    public DbSet<QMS_PARAMETER> QMS_PARAMETER { get; set; }
    public DbSet<QMS_QUEUE> QMS_QUEUE { get; set; }
    public DbSet<QMS_QUEUE_GROUP> QMS_QUEUE_GROUP { get; set; }
    public DbSet<QMS_QUEUE_ITEM> QMS_QUEUE_ITEM { get; set; }
    public DbSet<QMS_QUEUE_ITEM_REPORT> QMS_QUEUE_ITEM_REPORT { get; set; }
    public DbSet<QMS_QUEUE_TYPE> QMS_QUEUE_TYPE { get; set; }
    public DbSet<QMS_SCREEN> QMS_SCREEN { get; set; }
    public DbSet<QMS_SECTION> QMS_SECTION { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update NGAYCAPNHAT
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.NGAYCAPNHAT = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}