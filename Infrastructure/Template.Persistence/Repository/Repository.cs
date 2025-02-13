using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Common.Models.Entities;
using Template.Persistence.Contexts;

namespace Template.Persistence.Repository;

public class Repository<T>(TemplateDbContext context)
    : IRepository<T> where T : BaseEntity
{
    private DbSet<T> Table => context.Set<T>();
    public IQueryable<T?> GetQueryable() => Table.AsQueryable();
    public async Task<List<T?>> GetListAsync(Expression<Func<T?, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true)
         => await GetCommon(predicate, orderBy, include, disableTracking).ToListAsync();
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T?, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true)
        => await GetCommon(predicate, orderBy, include, disableTracking).FirstOrDefaultAsync();
    public async Task<List<T?>> GetPagedListAsync(int page, int size, Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null, bool disableTracking = true)
        => await GetCommon(predicate, orderBy, include, disableTracking).Skip((page - 1) * size).Take(size).ToListAsync();
    public async Task CreateAsync(T? entity)
    {
        await Table.AddAsync(entity);
    }
    public void Update(T? entity)
    {
        Table.Update(entity);
    }
    public void Delete(T? entity)
    {
        entity.IsDeleted = true;
        Update(entity);
    }
    private IQueryable<T?> GetCommon(Expression<Func<T?, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null, bool disableTracking = true)
    {
        var query = GetQueryable();
        if (disableTracking)
            query = query.AsNoTracking();
        if (predicate != null)
            query = query.Where(predicate);
        if (include != null)
            query = include(query!);
        if (orderBy != null)
            query = orderBy(query!);

        return query;
    }
}