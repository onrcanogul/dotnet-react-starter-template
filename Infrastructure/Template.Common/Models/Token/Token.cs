namespace Template.Common.Models.Token;

public class Token
{
    public string AccessToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public string? RefreshToken { get; set; }
}