using Microsoft.AspNetCore.Mvc;
using Template.Application.Abstraction.Users;
using Template.Application.Abstraction.Users.Dtos;

namespace Template.WebAPI.Controllers;

/// <summary>
/// Authentication endpoints. All three are anonymous by design - they are how a
/// caller obtains a token in the first place.
/// </summary>
public class UserController(IUserService service) : BaseController
{
    /// <summary>Exchanges credentials for an access/refresh token pair.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
        => ApiResult(await service.Login(dto));

    /// <summary>Exchanges a valid refresh token for a fresh token pair.</summary>
    [HttpPost("refresh-token-login")]
    public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenDto dto)
        => ApiResult(await service.LoginWithRefreshToken(dto.RefreshToken));

    /// <summary>Creates a new account.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto user)
        => ApiResult(await service.Register(user));
}
