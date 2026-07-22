using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Template.Application.Abstraction.Base.Search;
using Template.Application.Abstraction.Products.Dtos;
using Template.Application.Products;
using Template.Application.Products.Mappings;
using Template.Domain.Entities;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;
using Template.Shared.Exceptions;

/// <summary>
/// The mapper is used for real rather than mocked: it is a generated pure
/// function with no dependencies, so faking it would only assert that the test
/// author and the service agree - not that the mapping itself is correct.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _repositoryMock = new();
    private readonly Mock<ILogger<ProductService>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStringLocalizer> _localizerMock = new();
    private readonly Mock<IElasticSearchService> _elasticSearchServiceMock = new();
    private readonly ProductMapper _mapper = new();

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(
            _repositoryMock.Object,
            _loggerMock.Object,
            _mapper,
            _unitOfWorkMock.Object,
            _elasticSearchServiceMock.Object,
            _localizerMock.Object
        );
    }

    private void SetupNotFoundMessage()
        => _localizerMock.Setup(l => l["NotFound"]).Returns(new LocalizedString("NotFound", "Not found"));

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_AndReturnDto()
    {
        var dto = new ProductDto { Name = "Test", Description = "Test Desc" };

        var result = await _productService.CreateAsync(dto);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Data!.Name.Should().Be("Test");
        result.Data.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_ShouldReturnProductDto_WhenFound()
    {
        var entity = new Product { Id = Guid.NewGuid(), Name = "Test" };

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync(entity);

        var result = await _productService.FirstOrDefaultAsync(x => x.Id == entity.Id);

        result.Should().NotBeNull();
        result.Data!.Id.Should().Be(entity.Id);
        result.Data.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_ShouldThrow_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync((Product?)null);
        SetupNotFoundMessage();

        Func<Task> act = async () => await _productService.FirstOrDefaultAsync(x => x.Id == Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Not found");
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnMappedList()
    {
        var entities = new List<Product> { new() { Name = "Product 1" } };

        _repositoryMock.Setup(r => r.ToListAsync(null, null, null, true)).ReturnsAsync(entities);

        var result = await _productService.ToListAsync();

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Product 1");
    }

    [Fact]
    public async Task GetPagedListAsync_ShouldReturnPagedList()
    {
        var entities = new List<Product> { new() { Name = "Paged Product" } };

        _repositoryMock.Setup(r => r.ToPagedListAsync(1, 10, null, null, null, true)).ReturnsAsync(entities);

        var result = await _productService.ToPagedListAsync(1, 10);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Paged Product");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenFound()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Updated" };
        var entity = new Product { Id = dto.Id, Name = "Old" };

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync(entity);

        var result = await _productService.UpdateAsync(dto);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data!.Name.Should().Be("Updated");
        entity.Name.Should().Be("Updated", "the tracked entity is mutated in place");
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotOverwriteAuditColumns()
    {
        var created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Updated", CreatedBy = "attacker", CreatedDate = DateTime.UtcNow };
        var entity = new Product { Id = dto.Id, Name = "Old", CreatedBy = "system", CreatedDate = created };

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync(entity);

        await _productService.UpdateAsync(dto);

        entity.CreatedBy.Should().Be("system");
        entity.CreatedDate.Should().Be(created);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenEntityNotFound()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Doesn't Matter" };

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync((Product?)null);
        SetupNotFoundMessage();

        Func<Task> act = async () => await _productService.UpdateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Not found");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenEntityFound()
    {
        var id = Guid.NewGuid();
        var entity = new Product { Id = id, Name = "To Be Deleted" };

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync(entity);

        var result = await _productService.DeleteAsync(id);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenEntityNotFound()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, It.IsAny<bool>()))
                       .ReturnsAsync((Product?)null);
        SetupNotFoundMessage();

        Func<Task> act = async () => await _productService.DeleteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Not found");
    }
}
