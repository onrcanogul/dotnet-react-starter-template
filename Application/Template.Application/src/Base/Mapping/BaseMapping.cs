using AutoMapper;
using Template.Common.Models.Dtos;
using Template.Common.Models.Entities;

namespace Template.Application.src.Base.Mapping;

public class BaseMapping : Profile
{
    protected BaseMapping()
    {
        CreateMap<BaseEntity, BaseDto>().ReverseMap();
    }
}