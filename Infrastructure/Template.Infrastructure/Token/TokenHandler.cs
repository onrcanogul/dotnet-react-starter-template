using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Template.Domain.Entities.Identity;
using Template.Shared.Models.Token;

namespace Template.Infrastructure;

public class TokenHandler(IConfiguration configuration) : ITokenHandler
{
    public Token CreateToken(User user)
    {
        Token token = new();
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]!));
        SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);
        token.Expiration = DateTime.UtcNow.AddMinutes(15);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("name", user.UserName!),
            new("Id", user.Id.ToString())
            //add more claims into jwt token -> https://jwt.io/
        };
        JwtSecurityToken jwtSecurityToken = new(
            issuer: configuration["Token:Issuer"],
            audience: configuration["Token:Audience"],
            notBefore: DateTime.UtcNow,
            expires: token.Expiration,
            signingCredentials: signingCredentials,
            claims: claims
        );
        JwtSecurityTokenHandler handler = new();
        token.AccessToken = handler.WriteToken(jwtSecurityToken);
        token.RefreshToken = CreateRefreshToken();
        return token;
    }
    private static string CreateRefreshToken()
    {
        var number = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(number);
        return Convert.ToBase64String(number);
    }
}