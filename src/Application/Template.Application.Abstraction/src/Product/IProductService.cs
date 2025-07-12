using Template.Application.src.Abstraction.Base;
using Template.Application.src.Abstraction.Dto;
using Template.Domain.Entities;
using Template.Shared.Base.Response;

namespace Template.Application.src.Abstraction;

public interface IProductService : ICrudService<Product, ProductDto>
{
    Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name, string description);
    Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name);
    Task<ServiceResponse<NoContent>> CreateProduct(ProductDto product);
}