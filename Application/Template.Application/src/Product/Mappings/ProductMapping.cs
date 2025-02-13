using AutoMapper;
using Template.Application.src.Abstraction.Dto;
using Template.Common.Models.Dtos;
using Template.Common.Models.Entities;
using Template.Domain.Entities;

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