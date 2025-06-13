using Template.Domain.Entities.Identity;
using Template.Shared.Models.Token;

namespace Template.Infrastructure;

public interface ITokenHandler
{
    Token CreateToken(User user);
}