namespace Template.Application.Abstraction.Users.Dtos;

/// <summary>
/// Carries the refresh token in the request body. It must not travel in the
/// URL: paths are recorded by proxies, access logs and browser history.
/// </summary>
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = null!;
}
