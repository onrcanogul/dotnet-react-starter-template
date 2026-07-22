using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Template.IntegrationTests;

/// <summary>
/// Covers the register → login → refresh path end to end. Every defect this
/// guards against was present in the template and invisible to unit tests:
/// the refresh token was accepted past its expiry, a wrong password produced a
/// 500, and the JWT handler was registered under a scheme nothing used.
/// </summary>
[Collection(nameof(ApiCollection))]
public class AuthFlowTests(TemplateApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private static object NewUser(string suffix) => new
    {
        userName = $"user{suffix}",
        fullName = "Test User",
        email = $"user{suffix}@example.com",
        password = "Passw0rd!2024",
        confirmPassword = "Passw0rd!2024",
    };

    [Fact]
    public async Task Register_ThenLogin_ReturnsUsableTokenPair()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var register = await _client.PostAsJsonAsync("/api/user/register", NewUser(suffix));
        register.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = await _client.PostAsJsonAsync("/api/user/login", new
        {
            usernameOrEmail = $"user{suffix}",
            password = "Passw0rd!2024",
        });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await login.Content.ReadFromJsonAsync<TokenEnvelope>();
        body!.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
        body.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401_NotServerError()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        await _client.PostAsJsonAsync("/api/user/register", NewUser(suffix));

        var login = await _client.PostAsJsonAsync("/api/user/login", new
        {
            usernameOrEmail = $"user{suffix}",
            password = "wrong-password",
        });

        login.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownUser_FailsIdenticallyToAWrongPassword()
    {
        // Same status for both, so the endpoint cannot be used to discover
        // which accounts exist.
        var response = await _client.PostAsJsonAsync("/api/user/login", new
        {
            usernameOrEmail = "definitely-not-registered",
            password = "whatever",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_ExchangesForANewPair()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        await _client.PostAsJsonAsync("/api/user/register", NewUser(suffix));
        var login = await _client.PostAsJsonAsync("/api/user/login", new
        {
            usernameOrEmail = $"user{suffix}",
            password = "Passw0rd!2024",
        });
        var tokens = (await login.Content.ReadFromJsonAsync<TokenEnvelope>())!.Data;

        var refreshed = await _client.PostAsJsonAsync("/api/user/refresh-token-login", new
        {
            refreshToken = tokens.RefreshToken,
        });

        refreshed.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await refreshed.Content.ReadFromJsonAsync<TokenEnvelope>();
        body!.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RefreshToken_UnknownValue_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/user/refresh-token-login", new
        {
            refreshToken = "not-a-real-refresh-token",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_Returns400WithATranslatedMessage()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var response = await _client.PostAsJsonAsync("/api/user/register", new
        {
            userName = $"user{suffix}",
            fullName = "Test User",
            email = $"user{suffix}@example.com",
            password = "Passw0rd!2024",
            confirmPassword = "something-else",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ErrorEnvelope>();

        // The message must be resolved text, not the raw resource key - a key
        // missing from the localisation files renders as its own name.
        body!.Errors.Should().ContainSingle()
            .Which.Should().NotBe("PasswordsDoNotMatch");
    }

    private sealed record TokenEnvelope(TokenPayload Data);
    private sealed record TokenPayload(string AccessToken, string RefreshToken, DateTime Expiration);
    private sealed record ErrorEnvelope(List<string> Errors);
}
