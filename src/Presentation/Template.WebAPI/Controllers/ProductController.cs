using Microsoft.AspNetCore.Mvc;
using Template.Application.Abstraction.Products;
using Template.Application.Abstraction.Products.Dtos;

namespace Template.WebAPI.Controllers;

/// <summary>
/// Reference controller. Copy this shape for a new feature: derive from
/// <see cref="BaseController"/> (which supplies [ApiController] and the route),
/// inject the feature service, and let every action be a one-liner that hands a
/// <c>ServiceResponse</c> to <c>ApiResult</c>. No logic lives here.
/// </summary>
public class ProductController(IProductService service) : BaseController
{
    /// <summary>Gets all products.</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
        => ApiResult(await service.ToListAsync());

    /// <summary>Gets one page of products.</summary>
    /// <param name="page">Page number, 1-based.</param>
    /// <param name="size">Items per page.</param>
    [HttpGet("paged/{page:int}/{size:int}")]
    public async Task<IActionResult> Get([FromRoute] int page, [FromRoute] int size)
        => ApiResult(await service.ToPagedListAsync(page, size));

    /// <summary>Gets a single product by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
        => ApiResult(await service.FirstOrDefaultAsync(x => x.Id == id));

    /// <summary>Full-text search over product names.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
        => ApiResult(await service.SearchByNameAsync(name));

    /// <summary>Search filtered by both name and description.</summary>
    [HttpGet("search/advanced")]
    public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] string description)
        => ApiResult(await service.SearchAsync(name, description));

    /// <summary>Creates a product and indexes it for search.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
        => ApiResult(await service.CreateIndexedAsync(dto));

    /// <summary>Updates an existing product.</summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductDto dto)
        => ApiResult(await service.UpdateAsync(dto));

    /// <summary>Soft-deletes a product by id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ApiResult(await service.DeleteAsync(id));
}
