using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Moq;
using System.Linq.Expressions;
using Template.Application.src;
using Template.Application.src.Abstraction.Dto;
using Template.Domain.Entities;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStringLocalizer> _localizerMock = new();

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _localizerMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_AndReturnDto()
    {
        var dto = new ProductDto { Name = "Test", Description = "Test Desc" };
        var entity = new Product { Name = "Test", Description = "Test Desc" };

        _mapperMock.Setup(x => x.Map<Product>(dto)).Returns(entity);

        var result = await _productService.CreateAsync(dto);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Data.Should().BeEquivalentTo(dto, options => options.Excluding(d => d.Id));
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_ShouldReturnProductDto_WhenFound()
    {
        var entity = new Product { Id = Guid.NewGuid(), Name = "Test" };
        var dto = new ProductDto { Id = entity.Id, Name = "Test" };

        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true))
                       .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ProductDto>(entity)).Returns(dto);

        var result = await _productService.GetFirstOrDefaultAsync(x => x.Id == entity.Id);

        result.Should().NotBeNull();
        result.Data.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_ShouldThrow_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true))
                       .ReturnsAsync((Product?)null);
        _localizerMock.Setup(l => l["NotFound"]).Returns(new LocalizedString("NotFound", "Not found"));

        Func<Task> act = async () => await _productService.GetFirstOrDefaultAsync(x => x.Id == Guid.NewGuid());
        await act.Should().ThrowAsync<Template.Common.Exceptions.NotFoundException>().WithMessage("Not found");
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnMappedList()
    {
        var entities = new List<Product> { new() { Name = "Product 1" } };
        var dtos = new List<ProductDto> { new() { Name = "Product 1" } };

        _repositoryMock.Setup(r => r.GetListAsync(null, null, null, true)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<List<ProductDto>>(entities)).Returns(dtos);

        var result = await _productService.GetListAsync();

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetPagedListAsync_ShouldReturnPagedList()
    {
        var entities = new List<Product> { new() { Name = "Paged Product" } };
        var dtos = new List<ProductDto> { new() { Name = "Paged Product" } };

        _repositoryMock.Setup(r => r.GetPagedListAsync(1, 10, null, null, null, true)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<List<ProductDto>>(entities)).Returns(dtos);

        var result = await _productService.GetPagedListAsync(1, 10);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenFound()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Updated" };
        var entity = new Product { Id = dto.Id, Name = "Old" };

        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(dto, entity)).Returns(new Product { Id = dto.Id, Name = "Updated" });

        var result = await _productService.UpdateAsync(dto);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenEntityNotFound()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Doesn't Matter" };

        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true)).ReturnsAsync((Product?)null);
        _localizerMock.Setup(l => l["NotFound"]).Returns(new LocalizedString("NotFound", "Not found"));

        Func<Task> act = async () => await _productService.UpdateAsync(dto);

        await act.Should().ThrowAsync<Template.Common.Exceptions.NotFoundException>().WithMessage("Not found");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenEntityFound()
    {
        var id = Guid.NewGuid();
        var entity = new Product { Id = id, Name = "To Be Deleted" };

        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true))
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

        _repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null, true))
                       .ReturnsAsync((Product?)null);

        _localizerMock.Setup(l => l["NotFound"]).Returns(new LocalizedString("NotFound", "Not found"));

        Func<Task> act = async () => await _productService.DeleteAsync(id);

        await act.Should().ThrowAsync<Template.Common.Exceptions.NotFoundException>().WithMessage("Not found");
    }
}
