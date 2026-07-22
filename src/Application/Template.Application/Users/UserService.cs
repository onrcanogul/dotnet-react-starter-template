using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Template.Application.Abstraction.Users;
using Template.Application.Abstraction.Users.Dtos;
using Template.Domain.Entities.Identity;
using Template.Infrastructure.Authentication;
using Template.Shared.Base.Response;
using Template.Shared.Base.Tokens;
using Template.Shared.Exceptions;

namespace Template.Application.Users;

public class UserService(UserManager<User> service, IJwtTokenHandler tokenHandler, IUserMapper mapper, IHttpContextAccessor httpContextAccessor, IStringLocalizer localize)
    : IUserService
{
    /// <summary>How long a refresh token stays usable beyond the access token it was issued with.</summary>
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public string? GetCurrentUsername() => httpContextAccessor.HttpContext?.User.Identity?.Name;

    public async Task<ServiceResponse<Token>> Login(LoginDto dto)
    {
        var user = await service.FindByNameAsync(dto.UsernameOrEmail)
                   ?? await service.FindByEmailAsync(dto.UsernameOrEmail);

        // Deliberately the same failure for "no such user" and "wrong password":
        // telling them apart lets an attacker enumerate valid accounts.
        if (user is null || !await service.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedException(localize["InvalidCredentials"].Value);

        var token = tokenHandler.CreateToken(user);
        await UpdateRefreshTokenAsync(token.RefreshToken!, user);
        return ServiceResponse<Token>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse<Token>> LoginWithRefreshToken(string refreshToken)
    {
        var user = await service.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

        // The stored expiry has to be checked, otherwise a leaked refresh token
        // would grant access forever.
        if (user is null || user.RefreshTokenExpiration is null || user.RefreshTokenExpiration <= DateTime.UtcNow)
            throw new UnauthorizedException(localize["InvalidRefreshToken"].Value);

        var token = tokenHandler.CreateToken(user);
        await UpdateRefreshTokenAsync(token.RefreshToken!, user);
        return ServiceResponse<Token>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse> Register(RegisterDto model)
    {
        await Validations(model);
        var user = mapper.ToEntity(model);
        var result = await service.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        return ServiceResponse.Success(StatusCodes.Status201Created);
    }

    private async Task UpdateRefreshTokenAsync(string refreshToken, User user)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.Add(RefreshTokenLifetime);
        await service.UpdateAsync(user);
    }

    private async Task Validations(RegisterDto user)
    {
        if (user.Password != user.ConfirmPassword)
            throw new BadRequestException(localize["PasswordsDoNotMatch"].Value);
        if (await service.Users.AnyAsync(x => x.UserName == user.UserName || x.Email == user.Email))
            throw new BadRequestException(localize["UserAlreadyExists"].Value);
    }
}
