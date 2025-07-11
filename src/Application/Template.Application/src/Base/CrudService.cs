using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using Template.Application.src.Abstraction.Base;
using Template.Common.Exceptions;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;
using Template.Shared.Base.Dtos;
using Template.Shared.Base.Entities;
using Template.Shared.Base.Response;

namespace Template.Application.src.Base;

public class CrudService<T, TDto>(IRepository<T> repository, IMapper mapper, IUnitOfWork unitOfWork, IStringLocalizer localize) //dependency injections
    : ICrudService<T, TDto> //implementations
    where T : BaseEntity where TDto : BaseDto //constraints
{ 
    public async Task<ServiceResponse<List<TDto>>> ToListAsync(Expression<Func<T?, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var list = await repository.ToListAsync(predicate, orderBy, includeProperties, disableTracking);
        var dto = mapper.Map<List<TDto>>(list);
        return ServiceResponse<List<TDto>>.Success(dto, StatusCodes.Status200OK);
    }
    public async Task<ServiceResponse<TDto>> FirstOrDefaultAsync(Expression<Func<T?, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var entity = await repository.FirstOrDefaultAsync(predicate, orderBy, includeProperties, disableTracking);
        if(entity == null) throw new NotFoundException(localize["NotFound"].Value);
        var dto = mapper.Map<TDto>(entity);
        return ServiceResponse<TDto>.Success(dto, StatusCodes.Status200OK);
    }
    public async Task<ServiceResponse<List<TDto>>> ToPagedListAsync(int page, int size, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null, bool disableTracking = true)
    {
        var list = await repository.ToPagedListAsync(page, size, predicate, orderBy, includeProperties, disableTracking);
        var dto = mapper.Map<List<TDto>>(list);
        return ServiceResponse<List<TDto>>.Success(dto, StatusCodes.Status200OK);
    }
    public async Task<ServiceResponse<TDto>> CreateAsync(TDto dto)
    {
        dto.Id = Guid.NewGuid();
        await repository.CreateAsync(mapper.Map<T>(dto));
        await unitOfWork.CommitAsync();
        return ServiceResponse<TDto>.Success(dto, StatusCodes.Status201Created);
    }
    public async Task<ServiceResponse<TDto>> UpdateAsync(TDto dto)
    {
        var entity = await repository.FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null) throw new NotFoundException(localize["NotFound"].Value);
        entity = mapper.Map(dto, entity);
        repository.Update(entity);
        await unitOfWork.CommitAsync();
        return ServiceResponse<TDto>.Success(dto, StatusCodes.Status200OK);
    }
    public async Task<ServiceResponse<NoContent>> DeleteAsync(Guid id)
    {
        var entity = await repository.FirstOrDefaultAsync(x => x.Id == id);
        if(entity == null) throw new NotFoundException(localize["NotFound"].Value);
        repository.Delete(entity);
        await unitOfWork.CommitAsync();
        return ServiceResponse<NoContent>.Success(StatusCodes.Status200OK);
    }
}