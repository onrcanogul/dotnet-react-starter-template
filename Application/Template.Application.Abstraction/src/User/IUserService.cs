using Template.Application.Abstraction.src.User.Dto;
using Template.Common.Models.Response;
using Template.Common.Models.Token;

namespace Template.Application.Abstraction.src;

public interface IUserService
{
    Task<ServiceResponse<Token>> Login(LoginDto dto);
    Task<ServiceResponse<Token>> LoginWithRefreshToken(string refreshToken);
    Task<ServiceResponse> Register(RegisterDto model);
}