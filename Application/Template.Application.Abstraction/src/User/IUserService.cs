using Template.Application.Abstraction.src.User.Dto;
using Template.Common.Models.Response;
using Template.Common.Models.Token;

namespace Template.Application.Abstraction.src;

public interface IUserService
{
    Task<Response<Token>> Login(LoginDto dto);
    Task<Response<Token>> LoginWithRefreshToken(string refreshToken);
    Task<Response<NoContent>> Register(UserDto user);
}