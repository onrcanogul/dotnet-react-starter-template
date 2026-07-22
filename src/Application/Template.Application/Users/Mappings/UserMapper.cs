using Template.Application.Abstraction.Users;
using Template.Application.Abstraction.Users.Dtos;
using Template.Domain.Entities.Identity;

namespace Template.Application.Users.Mappings;

/// <summary>
/// Written by hand on purpose. <see cref="User"/> inherits a dozen ASP.NET
/// Identity columns that no DTO should ever populate, so a generated mapper
/// here would need more suppression attributes than it saves in code.
/// Aggregates deriving from <c>BaseEntity</c> use Mapperly instead - see
/// <c>ProductMapper</c>.
/// </summary>
public class UserMapper : IUserMapper
{
    public UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        Username = user.UserName ?? string.Empty,
    };

    /// <summary>
    /// Builds the entity for registration. The password is deliberately absent:
    /// hashing is <c>UserManager.CreateAsync(user, password)</c>'s job.
    /// </summary>
    public User ToEntity(RegisterDto dto) => new()
    {
        Id = Guid.NewGuid(),
        UserName = dto.UserName,
        FullName = dto.FullName,
        Email = dto.Email,
    };
}
