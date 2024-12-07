using System.Threading.Tasks;

namespace Template.Persistence.UnitOfWork;

public interface IUnitOfWork
{
    void Commit();
    Task CommitAsync();
}