using Template.Application.Abstraction.Base;
using Template.Application.Abstraction.Products.Dtos;
using Template.Domain.Entities;
using Template.Shared.Base.Response;

namespace Template.Application.Abstraction.Products;

/// <summary>
/// Reference feature service contract: inherit the standard CRUD from
/// <see cref="ICrudService{T,TDto}"/> and declare only what this feature adds.
/// </summary>
public interface IProductService : ICrudService<Product, ProductDto>
{
    Task<ServiceResponse<List<ProductDto>>> SearchByNameAsync(string name);

    Task<ServiceResponse<List<ProductDto>>> SearchAsync(string name, string description);

    /// <summary>Persists the product and indexes it for search in one operation.</summary>
    Task<ServiceResponse<NoContent>> CreateIndexedAsync(ProductDto product);
}
