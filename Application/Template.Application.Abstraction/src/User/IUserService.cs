using Template.Application.Abstraction.src.User.Dto;
using Template.Shared.Base.Response;
using Template.Shared.Models.Token;

namespace Template.Application.Abstraction.src;

public interface IUserService
{
    Task<ServiceResponse<Token>> Login(LoginDto dto);
    Task<ServiceResponse<Token>> LoginWithRefreshToken(string refreshToken);
    Task<ServiceResponse> Register(RegisterDto model);
}