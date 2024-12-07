using Template.Application.src.Abstraction.Dto;
using Template.Application.src.Base.Mapping;
using Template.Common.Models.Dtos;
using Template.Common.Models.Entities;
using Template.Domain.Entities;

namespace Template.Application.src.Mappings;

public class ProductMapper : BaseMapping
{
    public ProductMapper()
    {
        CreateMap<Product, ProductDto>().ReverseMap()
            .IncludeBase<BaseEntity, BaseDto>();
    }
}