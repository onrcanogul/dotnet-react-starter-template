using AutoMapper;
using Template.Application.Abstraction.src.User.Dto;
using Template.Domain.Entities.Identity;

namespace Template.Application.src.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>();
    }
}