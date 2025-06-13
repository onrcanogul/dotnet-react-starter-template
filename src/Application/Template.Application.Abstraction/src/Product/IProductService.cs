using Template.Application.src.Abstraction.Base;
using Template.Application.src.Abstraction.Dto;
using Template.Domain.Entities;

namespace Template.Application.src.Abstraction;

public interface IProductService : ICrudService<Product, ProductDto>
{
}