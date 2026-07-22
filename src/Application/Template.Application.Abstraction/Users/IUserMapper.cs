using Template.Application.Abstraction.Users.Dtos;
using Template.Domain.Entities.Identity;

namespace Template.Application.Abstraction.Users;

/// <summary>
/// User translation lives outside <c>IEntityMapper</c> because <see cref="User"/>
/// derives from ASP.NET Identity rather than from <c>BaseEntity</c>.
/// </summary>
public interface IUserMapper
{
    UserDto ToDto(User user);

    User ToEntity(RegisterDto dto);
}
