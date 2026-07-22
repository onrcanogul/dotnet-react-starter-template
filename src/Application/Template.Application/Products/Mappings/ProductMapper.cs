using AutoMapper;
using Template.Application.Abstraction.Products.Dtos;
using Template.Domain.Entities;
using Template.Shared.Base.Dtos;
using Template.Shared.Base.Entities;

namespace Template.Application.Products.Mappings;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductDto>()
            .IncludeBase<BaseEntity, BaseDto>();
        CreateMap<ProductDto, Product>()
            .IncludeBase<BaseDto, BaseEntity>();
        CreateMap<BaseEntity, BaseDto>().ReverseMap();
    }
}