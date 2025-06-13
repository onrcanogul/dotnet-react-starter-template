namespace Template.Application.Abstraction.src.User.Dto;

public class LoginDto
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}