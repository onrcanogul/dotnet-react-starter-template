using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Template.Persistence.Contexts;

namespace Template.Persistence.UnitOfWork;

public class UnitOfWork(TemplateDbContext context) : IUnitOfWork
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