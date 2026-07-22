using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Persistence.Contexts;
using Testcontainers.PostgreSql;

namespace Template.IntegrationTests;

/// <summary>
/// Boots the real application against a throwaway PostgreSQL container.
///
/// Everything the app does at startup runs for real - DI wiring, middleware
/// order, EF model, authentication - which is the point: these are the failures
/// unit tests structurally cannot see. Registering the JWT handler under the
/// wrong scheme, or omitting UseAuthentication, both look fine in a unit test
/// and 500 in production.
///
/// Only the database is substituted. Redis and Elasticsearch connect lazily and
/// are not touched by these tests.
/// </summary>
public class TemplateApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("template-tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.UseSetting("ConnectionStrings:PostgreSQL", _postgres.GetConnectionString());
        // Left empty on purpose: the database log sink is optional, and tests
        // have no use for a "logs" table.
        builder.UseSetting("ConnectionStrings:LoggingDb", "");
        // A fixed key: these tokens never leave the test process.
        builder.UseSetting("Token:SecurityKey", "integration-tests-signing-key-not-used-anywhere-else-0123456789");
        builder.UseSetting("Token:Issuer", "http://localhost");
        builder.UseSetting("Token:Audience", "http://localhost");
        builder.UseSetting("OTEL_SDK_DISABLED", "true");
        builder.UseSetting("Serilog:MinimumLevel:Default", "Warning");
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Apply migrations rather than EnsureCreated: this also proves the
        // migrations themselves still apply to an empty database.
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TemplateDbContext>().Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}

[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<TemplateApiFactory>;
