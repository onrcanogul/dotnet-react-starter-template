using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Base.Search;
using Template.Application.src.Abstraction.Dto;
using Template.Application.src.Base;
using Template.Domain.Entities;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;
using Template.Shared.Base.Response;

namespace Template.Application.src;

public class ProductService(
        IRepository<Product> repository,
        ILogger<ProductService> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IElasticSearchService elasticSearchService,
        IStringLocalizer localize) 
    : CrudService<Product, ProductDto>(repository, mapper, unitOfWork, localize), IProductService
{
    public async Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name, string description)
    {
        var filters = new Dictionary<string, string> { { "name", name }, { "description", description } };
        var result = await elasticSearchService.MultiFieldFilterSearchAsync<Product>("products", filters);
        
        return ServiceResponse<List<ProductDto>>.Success(mapper.Map<List<ProductDto>>(result), StatusCodes.Status200OK);
    }
    
    public async Task<ServiceResponse<List<ProductDto>>> SearchProducts(string name)
    {
        var results = await elasticSearchService.SearchAsync<Product>("products", name, "name");
        logger.LogInformation("Elasticsearch returned {Count} results for keyword '{Keyword}'", results.Count, name);
        return ServiceResponse<List<ProductDto>>.Success(mapper.Map<List<ProductDto>>(results), StatusCodes.Status200OK);
    }
    
    public async Task<ServiceResponse<NoContent>> CreateProduct(ProductDto product)
    {
        var productEntity = mapper.Map<Product>(product);
        productEntity.Id = Guid.NewGuid();
        
        await repository.CreateAsync(productEntity);
        await unitOfWork.CommitAsync();
        
        await elasticSearchService.IndexAsync("products", productEntity);
        logger.LogInformation("Product indexed to Elasticsearch: {ProductId}", product.Id);
        return ServiceResponse<NoContent>.Success(StatusCodes.Status201Created);
    }
}