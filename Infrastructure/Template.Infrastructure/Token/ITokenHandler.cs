using Template.Common.Models.Token;
using Template.Domain.Entities.Identity;

namespace Template.Infrastructure;

public interface ITokenHandler
{
    Token CreateToken(User user);
}