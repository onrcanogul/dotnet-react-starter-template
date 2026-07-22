using Riok.Mapperly.Abstractions;
using Template.Application.Abstraction.Base;
using Template.Application.Abstraction.Products.Dtos;
using Template.Domain.Entities;

namespace Template.Application.Products.Mappings;

/// <summary>
/// Reference mapper. Copy this shape for a new feature: a partial class marked
/// <c>[Mapper]</c> whose method bodies Mapperly generates at compile time.
///
/// Identity and audit columns are written by the persistence layer
/// (<c>TemplateDbContext.AuditingEntities</c>), never by an inbound DTO, so
/// <see cref="Apply"/> ignores them explicitly.
/// </summary>
[Mapper]
public partial class ProductMapper : IEntityMapper<Product, ProductDto>
{
    // IsDeleted drives the soft-delete query filter; it is never exposed to clients.
    [MapperIgnoreSource(nameof(Product.IsDeleted))]
    public partial ProductDto ToDto(Product entity);

    [MapperIgnoreTarget(nameof(Product.IsDeleted))]
    public partial Product ToEntity(ProductDto dto);

    public partial List<ProductDto> ToDtoList(IEnumerable<Product> entities);

    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.IsDeleted))]
    [MapperIgnoreTarget(nameof(Product.CreatedDate))]
    [MapperIgnoreTarget(nameof(Product.CreatedBy))]
    [MapperIgnoreTarget(nameof(Product.UpdatedDate))]
    [MapperIgnoreTarget(nameof(Product.UpdatedBy))]
    [MapperIgnoreSource(nameof(ProductDto.Id))]
    [MapperIgnoreSource(nameof(ProductDto.CreatedDate))]
    [MapperIgnoreSource(nameof(ProductDto.CreatedBy))]
    [MapperIgnoreSource(nameof(ProductDto.UpdatedDate))]
    [MapperIgnoreSource(nameof(ProductDto.UpdatedBy))]
    public partial void Apply(ProductDto dto, Product target);
}
