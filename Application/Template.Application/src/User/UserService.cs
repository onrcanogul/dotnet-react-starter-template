using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Application.Abstraction.src;
using Template.Application.Abstraction.src.User.Dto;
using Template.Common.Models.Response;
using Template.Common.Models.Token;
using Template.Domain.Entities.Identity;
using Template.Infrastructure;

namespace Template.Application.src;

public class UserService(UserManager<User> service, ITokenHandler tokenHandler, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    : IUserService
{
    public string? GetCurrentUsername()
        => httpContextAccessor.HttpContext?.User.Identity!.Name;
    public async Task<Response<Token>> Login(LoginDto dto)
    {
        var user = await service.FindByNameAsync(dto.UsernameOrEmail) 
                   ?? await service.FindByEmailAsync(dto.UsernameOrEmail)
                   ?? throw new ArgumentNullException();
        var result = await service.CheckPasswordAsync(user, dto.Password);
        if (!result) throw new Exception();
        var token = tokenHandler.CreateToken(user);
        await UpdateRefreshTokenAsync(token.RefreshToken!, user, token.Expiration, 30);
        return Response<Token>.Success(token, StatusCodes.Status200OK);
    }
    public async Task<Response<Token>> LoginWithRefreshToken(string refreshToken)
    {
        var user = await service.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user == null) throw new NullReferenceException("user not found");
        var token = tokenHandler.CreateToken(user);
        await UpdateRefreshTokenAsync(refreshToken, user, token.Expiration, 10);
        return Response<Token>.Success(token, StatusCodes.Status200OK);
    }
    public async Task<Response<NoContent>> Register(UserDto user)
        => (await service.CreateAsync(mapper.Map<User>(user))).Succeeded
            ? Response<NoContent>.Success(StatusCodes.Status201Created)
            : Response<NoContent>.Failure("Failed to create user", StatusCodes.Status400BadRequest);
    private async Task UpdateRefreshTokenAsync(string refreshToken, User user, DateTime accessTokenDate, int addToAccessToken)
    {
        if(user == null) throw new NullReferenceException("user not found");
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = accessTokenDate.AddMinutes(addToAccessToken);
        await service.UpdateAsync(user);
    }
    
}