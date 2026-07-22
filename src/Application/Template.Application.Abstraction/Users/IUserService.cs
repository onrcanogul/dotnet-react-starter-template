using Template.Application.Abstraction.Users.Dtos;
using Template.Shared.Base.Response;
using Template.Shared.Base.Tokens;

namespace Template.Application.Abstraction.Users;

public interface IUserService
{
    Task<ServiceResponse<Token>> Login(LoginDto dto);
    Task<ServiceResponse<Token>> LoginWithRefreshToken(string refreshToken);
    Task<ServiceResponse> Register(RegisterDto model);
}