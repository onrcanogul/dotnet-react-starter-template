using AutoMapper;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Dto;
using Template.Application.src.Base;
using Template.Domain.Entities;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;

namespace Template.Application.src;

public class ProductService(IRepository<Product> repository, IMapper mapper, IUnitOfWork unitOfWork) 
    : CrudService<Product, ProductDto>(repository, mapper, unitOfWork),IProductService
{
    
}