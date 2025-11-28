using AutoMapper;
using Users.Application.DTOs;
using Users.Domain.Models;

namespace Users.Application.MapProfiles;

public class UserApplicationProfile : Profile
{
    public UserApplicationProfile()
    {
        CreateMap<UserResponse, UserModel>().ReverseMap();
        CreateMap<RegisterRequest, UserModel>().ReverseMap();
    }
}