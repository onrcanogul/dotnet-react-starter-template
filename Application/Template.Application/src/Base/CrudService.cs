using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Template.Application.src.Abstraction.Base;
using Template.Common.Models.Dtos;
using Template.Common.Models.Entities;
using Template.Common.Models.Response;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;

namespace Template.Application.src.Base;

public class CrudService<T, TDto>(IRepository<T> repository, IMapper mapper, IUnitOfWork unitOfWork) //dependency injections
    : ICrudService<T, TDto> //implementations
    where T : BaseEntity where TDto : BaseDto //constraints
{
     public async Task<Response<List<TDto>>> GetListAsync(Expression<Func<T?, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var list = await repository.GetListAsync(predicate, orderBy, includeProperties, disableTracking);
        var dto = mapper.Map<List<TDto>>(list);
        return Response<List<TDto>>.Success(dto, StatusCodes.Status200OK);
    }

    public async Task<Response<TDto>> GetFirstOrDefaultAsync(Expression<Func<T?, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includeProperties = null,
        bool disableTracking = true)
    {
        var entity = await repository.GetFirstOrDefaultAsync(predicate, orderBy, includeProperties, disableTracking);
        if(entity == null) throw new NullReferenceException();
        var dto = mapper.Map<TDto>(entity);
        return Response<TDto>.Success(dto, StatusCodes.Status200OK);
    }

    public async Task<Response<List<TDto>>> GetPagedListAsync(int page, int size, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeProperties = null, bool disableTracking = true)
    {
        var list = await repository.GetPagedListAsync(page, size, predicate, orderBy, includeProperties, disableTracking);
        var dto = mapper.Map<List<TDto>>(list);
        return Response<List<TDto>>.Success(dto, StatusCodes.Status200OK);
    }

    public async Task<Response<TDto>> CreateAsync(TDto dto)
    {
        dto.Id = Guid.NewGuid();
        await repository.CreateAsync(mapper.Map<T>(dto));
        await unitOfWork.CommitAsync();
        return Response<TDto>.Success(dto, StatusCodes.Status201Created);
    }

    public async Task<Response<TDto>> UpdateAsync(TDto dto)
    {
        var entity = await repository.GetFirstOrDefaultAsync(x => x.Id == dto.Id);
        entity = mapper.Map(dto, entity);
        repository.Update(entity);
        await unitOfWork.CommitAsync();
        return Response<TDto>.Success(dto, StatusCodes.Status200OK);
    }

    public async Task<Response<NoContent>> DeleteAsync(Guid id)
    {
        var entity = await repository.GetFirstOrDefaultAsync(x => x.Id == id);
        repository.Delete(entity);
        await unitOfWork.CommitAsync();
        return Response<NoContent>.Success(StatusCodes.Status200OK);
    }
    
    private IQueryable<T> GetCommon(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null, bool disableTracking = true)
    {
        var query = repository.GetQueryable();
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        if (predicate != null)
        {
            query = query.Where(predicate!);
        }
        if (include != null)
        {
            query = include(query!);
        }
        if (orderBy != null)
        {
            query = orderBy(query!);
        }

        return query;
    }
}