using System.Linq.Expressions;
using Template.Shared.Base.Entities;

namespace Template.Persistence.Repository;

/// <summary>
/// Read/write access to a single aggregate. Nothing here commits - call
/// <c>IUnitOfWork.CommitAsync()</c> once per use case instead.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetQueryable();

    Task<List<T>> ToListAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true);

    Task<List<T>> ToPagedListAsync(int page, int size, Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true);

    Task CreateAsync(T entity);
    void Update(T entity);

    /// <summary>Soft delete - sets <c>IsDeleted</c>, never removes the row.</summary>
    void Delete(T entity);
}
