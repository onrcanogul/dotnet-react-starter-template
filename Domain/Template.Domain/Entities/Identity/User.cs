using Microsoft.AspNetCore.Identity;

namespace Template.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}