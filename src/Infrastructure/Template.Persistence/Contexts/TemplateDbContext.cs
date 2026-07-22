using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Domain.Entities.Identity;
using Template.Shared.Base.Entities;

namespace Template.Persistence.Contexts;

public class TemplateDbContext(DbContextOptions<TemplateDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<User, Role, Guid>(options)
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

    /// <summary>
    /// Stamps the audit columns on every <see cref="BaseEntity"/> about to be
    /// written. This is the only place they are set - services and DTOs must
    /// never write them.
    /// </summary>
    private void AuditingEntities()
    {
        var now = DateTime.UtcNow;
        var currentUser = GetCurrentUsername() ?? SystemUser;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            var entity = entry.Entity;
            switch (entry.State)
            {
                case EntityState.Modified:
                    entity.UpdatedDate = now;
                    entity.UpdatedBy = currentUser;
                    break;
                case EntityState.Added:
                    entity.CreatedDate = now;
                    entity.UpdatedDate = now;
                    entity.CreatedBy = currentUser;
                    break;
            }
        }
    }

    /// <summary>Attributed to writes with no signed-in user: migrations, seeding, background jobs.</summary>
    private const string SystemUser = "system";

    private string? GetCurrentUsername()
        => httpContextAccessor.HttpContext?.User.Identity?.Name;
}
