using Microsoft.EntityFrameworkCore;
using Template.Common.Models.Entities;
using Template.Domain.Entities;

namespace Template.Persistence.Contexts;

public class TemplateDbContext(DbContextOptions<TemplateDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AuditingEntities();
        return base.SaveChangesAsync(cancellationToken);
    }
    public override int SaveChanges()
    {
        AuditingEntities();
        return base.SaveChanges();
    }
    private void AuditingEntities()
    {
        var dataList = ChangeTracker.Entries<BaseEntity>().ToList();

        foreach (var data in dataList)
        {
            var baseEntity = data.Entity;
            switch (data.State)
            {
                case EntityState.Modified:
                    baseEntity.UpdatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Added:
                    baseEntity.CreatedDate = DateTime.UtcNow;
                    baseEntity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }
    }
}