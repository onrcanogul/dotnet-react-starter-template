using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Template.Application.Abstraction.Products;
using Template.Application.Abstraction.Base.Search;
using Template.Application.Abstraction.Base;
using Template.Application.Abstraction.Products.Dtos;
using Template.Application.Base;
using Template.Domain.Entities;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;
using Template.Shared.Base.Response;

namespace Template.Application.Products;

/// <summary>
/// Reference feature service. Copy this shape for new features: inherit
/// <see cref="CrudService{T,TDto}"/> for the standard CRUD, implement the
/// feature interface for anything beyond it, and reach the shared dependencies
/// through the base class's protected members - never re-capture them.
/// </summary>
public class ProductService(
        IRepository<Product> repository,
        ILogger<ProductService> logger,
        IEntityMapper<Product, ProductDto> mapper,
        IUnitOfWork unitOfWork,
        IElasticSearchService elasticSearchService,
        IStringLocalizer localize)
    : CrudService<Product, ProductDto>(repository, mapper, unitOfWork, localize), IProductService
{
    private const string IndexName = "products";

    public async Task<ServiceResponse<List<ProductDto>>> SearchAsync(string name, string description)
    {
        var filters = new Dictionary<string, string> { { "name", name }, { "description", description } };
        var result = await elasticSearchService.MultiFieldFilterSearchAsync<Product>(IndexName, filters);

        return ServiceResponse<List<ProductDto>>.Success(Mapper.ToDtoList(result), StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<List<ProductDto>>> SearchByNameAsync(string name)
    {
        var results = await elasticSearchService.SearchAsync<Product>(IndexName, name, "name");
        logger.LogInformation("Elasticsearch returned {Count} results for keyword '{Keyword}'", results.Count, name);
        return ServiceResponse<List<ProductDto>>.Success(Mapper.ToDtoList(results), StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<NoContent>> CreateIndexedAsync(ProductDto product)
    {
        var productEntity = Mapper.ToEntity(product);
        productEntity.Id = Guid.NewGuid();

        await Repository.CreateAsync(productEntity);
        await UnitOfWork.CommitAsync();

        await elasticSearchService.IndexAsync(IndexName, productEntity);
        logger.LogInformation("Product indexed to Elasticsearch: {ProductId}", productEntity.Id);
        return ServiceResponse<NoContent>.Success(StatusCodes.Status201Created);
    }
}
