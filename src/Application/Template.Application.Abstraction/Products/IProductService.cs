using Template.Application.Abstraction.Base;
using Template.Application.Abstraction.Products.Dtos;
using Template.Domain.Entities;
using Template.Shared.Base.Response;

namespace Template.Application.Abstraction.Products;

public interface IProductService : ICrudService<Product, ProductDto>
{
    Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name, string description);
    Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name);
    Task<ServiceResponse<NoContent>> CreateProduct(ProductDto product);
}