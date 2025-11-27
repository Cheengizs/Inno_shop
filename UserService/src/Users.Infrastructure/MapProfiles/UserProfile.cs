using AutoMapper;
using Users.Domain.Models;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure.MapProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserModel, UserEntity>().ReverseMap();
    }
}