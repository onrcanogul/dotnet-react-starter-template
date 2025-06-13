using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Dto;
using Template.Shared.Base.Response;
using Template.WebAPI.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock = new();
    private readonly Mock<IStringLocalizer> _localizerMock = new();
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _controller = new ProductController(_productServiceMock.Object, _localizerMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOkWithProductList()
    {
        var expected = ServiceResponse<List<ProductDto>>.Success(new List<ProductDto> { new() { Name = "Test Product" } }, StatusCodes.Status200OK);
        _productServiceMock.Setup(s => s.GetListAsync(null, null, null, true))
            .ReturnsAsync(expected);

        var result = await _controller.Get();

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        var response = objectResult.Value as ServiceResponse<List<ProductDto>>;
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenFound()
    {
        var id = Guid.NewGuid();
        var dto = new ProductDto { Id = id, Name = "Product" };
        var response = ServiceResponse<ProductDto>.Success(dto, StatusCodes.Status200OK);

        _productServiceMock.Setup(s => s.GetFirstOrDefaultAsync(x => x.Id == id, null, null, true))
            .ReturnsAsync(response);

        var result = await _controller.Get(id);

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        (objectResult.Value as ServiceResponse<ProductDto>)!.Data.Id.Should().Be(id);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedStatus()
    {
        var dto = new ProductDto { Name = "Yeni" };
        var response = ServiceResponse<ProductDto>.Success(dto, StatusCodes.Status201Created);

        _productServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        var result = await _controller.Create(dto);

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        var dto = new ProductDto { Id = Guid.NewGuid(), Name = "Güncel" };
        var response = ServiceResponse<ProductDto>.Success(dto, StatusCodes.Status200OK);

        _productServiceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(response);

        var result = await _controller.Update(dto);

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenDeleted()
    {
        var id = Guid.NewGuid();
        var response = ServiceResponse<NoContent>.Success(StatusCodes.Status200OK);

        _productServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(response);

        var result = await _controller.Delete(id);

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetPagedList_ShouldReturnPagedProducts()
    {
        var page = 1;
        var size = 10;
        var dtos = new List<ProductDto> { new() { Name = "Paged Product" } };
        var response = ServiceResponse<List<ProductDto>>.Success(dtos, StatusCodes.Status200OK);

        _productServiceMock.Setup(s => s.GetPagedListAsync(page, size, null, null, null, true)).ReturnsAsync(response);

        var result = await _controller.Get(page, size);

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        (objectResult.Value as ServiceResponse<List<ProductDto>>)?.Data.Should().HaveCount(1);
    }
}
