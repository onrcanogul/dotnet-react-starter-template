using AutoMapper;
using Template.Application.src.Abstraction.Dto;
using Template.Domain.Entities;
using Template.Shared.Base.Dtos;
using Template.Shared.Base.Entities;

namespace Template.Application.src.Mappings;

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