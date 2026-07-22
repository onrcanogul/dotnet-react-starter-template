using Template.Shared.Base.Dtos;
using Template.Shared.Base.Entities;

namespace Template.Application.Abstraction.Base;

/// <summary>
/// Entity &lt;-&gt; DTO translation for one aggregate.
///
/// Implementations are generated at compile time by Mapperly, so a property
/// that cannot be mapped is a build error rather than a surprise at runtime.
/// Every feature ships exactly one implementation, registered in
/// <c>Template.Application.ServiceRegistration</c>.
/// </summary>
public interface IEntityMapper<TEntity, TDto>
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    TDto ToDto(TEntity entity);

    TEntity ToEntity(TDto dto);

    List<TDto> ToDtoList(IEnumerable<TEntity> entities);

    /// <summary>
    /// Copies the editable fields of <paramref name="dto"/> onto an already
    /// tracked <paramref name="target"/>. Identity and audit columns are owned
    /// by the persistence layer and must not be overwritten here.
    /// </summary>
    void Apply(TDto dto, TEntity target);
}
