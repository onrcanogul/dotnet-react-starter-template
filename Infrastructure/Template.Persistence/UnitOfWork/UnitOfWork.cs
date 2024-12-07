using Microsoft.EntityFrameworkCore;

namespace Template.Persistence.UnitOfWork;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    public void Commit()
    {
        context.SaveChanges();
    }
    public async Task CommitAsync()
    {
        await context.SaveChangesAsync();
    }
}