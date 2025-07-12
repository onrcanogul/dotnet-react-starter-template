using Microsoft.AspNetCore.Mvc;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Dto;
using Template.WebAPI.Controllers.Base;

namespace Template.WebAPI.Controllers;

/// <summary>
/// API controller for managing product-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService service) : BaseController
{
    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>List of all products.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
        => ApiResult(await service.ToListAsync());

    /// <summary>
    /// Gets a paged list of products.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="size">The number of items per page.</param>
    /// <returns>Paged list of products.</returns>
    [HttpGet("paged/{page:int}/{size:int}")]
    public async Task<IActionResult> Get([FromRoute] int page, [FromRoute] int size)
        => ApiResult(await service.ToPagedListAsync(page, size));

    /// <summary>
    /// Gets a specific product by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>Product matching the given ID.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
        => ApiResult(await service.FirstOrDefaultAsync(x => x.Id == id));
    
    /// <summary>
    /// Creates a new product (without indexing to Elasticsearch).
    /// </summary>
    /// <param name="dto">The product data transfer object.</param>
    /// <returns>Result of the create operation.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
        => ApiResult(await service.CreateAsync(dto));
    
    /// <summary>
    /// Creates a new product and indexes it into Elasticsearch.
    /// </summary>
    /// <param name="dto">The product data transfer object.</param>
    /// <returns>Result of the indexed create operation.</returns>
    [HttpPost("create-indexed")]
    public async Task<IActionResult> CreateIndexed([FromBody] ProductDto dto)
        => ApiResult(await service.CreateProduct(dto));
    
    /// <summary>
    /// Searches for products by name.
    /// </summary>
    /// <param name="name">The product name to search for.</param>
    /// <returns>List of matching products.</returns>
    [HttpPost("{name}")]
    public async Task<IActionResult> Search(string name)
        => ApiResult(await service.SearchProducts(name));
    
    /// <summary>
    /// Searches for products by both name and description.
    /// </summary>
    /// <param name="name">The product name to search for.</param>
    /// <param name="description">The product description to search for.</param>
    /// <returns>List of matching products.</returns>
    [HttpPost("{name}/{description}")]
    public async Task<IActionResult> Search(string name, string description)
        => ApiResult(await service.SearchProducts(name, description));
    
    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="dto">The updated product data transfer object.</param>
    /// <returns>Result of the update operation.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductDto dto)
        => ApiResult(await service.UpdateAsync(dto));
    
    /// <summary>
    /// Deletes a product by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>Result of the delete operation.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ApiResult(await service.DeleteAsync(id));
}
