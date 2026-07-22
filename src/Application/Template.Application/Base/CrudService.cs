using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using Template.Application.Abstraction.Base;
using Template.Shared.Exceptions;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;
using Template.Shared.Base.Dtos;
using Template.Shared.Base.Entities;
using Template.Shared.Base.Response;

namespace Template.Application.Base;

/// <summary>
/// Default CRUD implementation shared by every feature service.
///
/// Feature services inherit from this and add their own operations. They must
/// reuse the protected <see cref="Repository"/>/<see cref="Mapper"/>/
/// <see cref="UnitOfWork"/> members rather than capturing their own copies -
/// declaring the same dependency twice is what triggers CS9107.
/// </summary>
public class CrudService<T, TDto>(IRepository<T> repository, IEntityMapper<T, TDto> mapper, IUnitOfWork unitOfWork, IStringLocalizer localize)
    : ICrudService<T, TDto>
    where T : BaseEntity where TDto : BaseDto
{
    protected IRepository<T> Repository { get; } = repository;
    protected IEntityMapper<T, TDto> Mapper { get; } = mapper;
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IStringLocalizer Localize { get; } = localize;

    public async Task<ServiceResponse<List<TDto>>> ToListAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var list = await Repository.ToListAsync(predicate, orderBy, includeProperties, disableTracking);
        return ServiceResponse<List<TDto>>.Success(Mapper.ToDtoList(list), StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<TDto>> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var entity = await Repository.FirstOrDefaultAsync(predicate, orderBy, includeProperties, disableTracking)
                     ?? throw new NotFoundException(Localize["NotFound"].Value);
        return ServiceResponse<TDto>.Success(Mapper.ToDto(entity), StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<List<TDto>>> ToPagedListAsync(int page, int size, Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null, bool disableTracking = true)
    {
        var list = await Repository.ToPagedListAsync(page, size, predicate, orderBy, includeProperties, disableTracking);
        return ServiceResponse<List<TDto>>.Success(Mapper.ToDtoList(list), StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<TDto>> CreateAsync(TDto dto)
    {
        dto.Id = Guid.NewGuid();
        await Repository.CreateAsync(Mapper.ToEntity(dto));
        await UnitOfWork.CommitAsync();
        return ServiceResponse<TDto>.Success(dto, StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<TDto>> UpdateAsync(TDto dto)
    {
        // Tracking must stay on: the entity is mutated and committed below.
        var entity = await Repository.FirstOrDefaultAsync(x => x.Id == dto.Id, disableTracking: false)
                     ?? throw new NotFoundException(Localize["NotFound"].Value);
        Mapper.Apply(dto, entity);
        Repository.Update(entity);
        await UnitOfWork.CommitAsync();
        return ServiceResponse<TDto>.Success(dto, StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<NoContent>> DeleteAsync(Guid id)
    {
        var entity = await Repository.FirstOrDefaultAsync(x => x.Id == id, disableTracking: false)
                     ?? throw new NotFoundException(Localize["NotFound"].Value);
        Repository.Delete(entity);
        await UnitOfWork.CommitAsync();
        return ServiceResponse<NoContent>.Success(StatusCodes.Status200OK);
    }
}
