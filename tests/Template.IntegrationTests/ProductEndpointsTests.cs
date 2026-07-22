using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Domain.Entities;
using Template.Persistence.Contexts;

namespace Template.IntegrationTests;

/// <summary>
/// Exercises the reference feature against a real database, so the parts a
/// mocked repository cannot check are covered: that the audit columns are
/// actually written, and that the soft-delete query filter actually hides rows.
/// </summary>
[Collection(nameof(ApiCollection))]
public class ProductEndpointsTests(TemplateApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private sealed record ProductEnvelope(ProductPayload Data);
    private sealed record ProductListEnvelope(List<ProductPayload> Data);
    private sealed record ProductPayload(Guid Id, string Name, string Description, string CreatedBy, DateTime CreatedDate);

    private async Task<Guid> CreateProductAsync(string name)
    {
        // Goes straight to the database: the create endpoint also indexes into
        // Elasticsearch, which is not part of this test's fixture.
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
        var product = new Product { Id = Guid.NewGuid(), Name = name, Description = "created by an integration test" };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product.Id;
    }

    [Fact]
    public async Task Get_ReturnsTheProduct_WithAuditColumnsPopulated()
    {
        var id = await CreateProductAsync("Integration Product");

        var response = await _client.GetAsync($"/api/product/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ProductEnvelope>();
        body!.Data.Name.Should().Be("Integration Product");

        // Stamped by TemplateDbContext.AuditingEntities, never by the caller.
        body.Data.CreatedBy.Should().Be("system");
        body.Data.CreatedDate.Should().BeAfter(DateTime.UtcNow.AddMinutes(-5));
    }

    [Fact]
    public async Task Get_UnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/api/product/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ChangesTheProduct_AndLeavesCreatedByAlone()
    {
        var id = await CreateProductAsync("Before");

        var response = await _client.PutAsJsonAsync("/api/product", new
        {
            id,
            name = "After",
            description = "updated",
            createdBy = "attacker",   // must be ignored
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reread = await _client.GetFromJsonAsync<ProductEnvelope>($"/api/product/{id}");
        reread!.Data.Name.Should().Be("After");
        reread.Data.CreatedBy.Should().Be("system", "audit columns belong to the persistence layer");
    }

    [Fact]
    public async Task Delete_IsSoft_AndHidesTheRowFromReads()
    {
        var id = await CreateProductAsync("To Delete");

        var deleted = await _client.DeleteAsync($"/api/product/{id}");
        deleted.StatusCode.Should().Be(HttpStatusCode.OK);

        var reread = await _client.GetAsync($"/api/product/{id}");
        reread.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // The row is still there - the global query filter is what hides it.
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
        var row = context.Products.IgnoreQueryFilters().Single(p => p.Id == id);
        row.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task List_ExcludesSoftDeletedProducts()
    {
        var kept = await CreateProductAsync($"Kept {Guid.NewGuid():N}");
        var removed = await CreateProductAsync($"Removed {Guid.NewGuid():N}");
        await _client.DeleteAsync($"/api/product/{removed}");

        var list = await _client.GetFromJsonAsync<ProductListEnvelope>("/api/product");

        list!.Data.Should().Contain(p => p.Id == kept);
        list.Data.Should().NotContain(p => p.Id == removed);
    }
}
