using Template.Domain.Entities.Identity;
using Template.Shared.Base.Tokens;

namespace Template.Infrastructure.Authentication;

public interface IJwtTokenHandler
{
    Token CreateToken(User user);
}