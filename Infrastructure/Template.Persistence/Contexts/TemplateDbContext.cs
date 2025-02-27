using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Template.Common.Models.Entities;
using Template.Domain.Entities;
using Template.Domain.Entities.Identity;

namespace Template.Persistence.Contexts;

public class TemplateDbContext(DbContextOptions<TemplateDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<User,Role,Guid>(options)
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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
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
                    baseEntity.UpdatedBy = "oogul";
                    break;
                case EntityState.Added:
                    baseEntity.CreatedDate = DateTime.UtcNow;
                    baseEntity.UpdatedDate = DateTime.UtcNow;
                    baseEntity.CreatedBy = "oogul";
                    break;
            }
        }
    }
    private string? GetCurrentUsername()
        => httpContextAccessor.HttpContext?.User.Identity!.Name;
}