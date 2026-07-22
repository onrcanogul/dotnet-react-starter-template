using AutoMapper;
using Template.Application.Abstraction.Users.Dtos;
using Template.Domain.Entities.Identity;

namespace Template.Application.Users.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, RegisterDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
    }
}