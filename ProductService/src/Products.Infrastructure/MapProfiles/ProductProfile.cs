using AutoMapper;
using Products.Domain.Models;
using Products.Infrastructure.Entities;

namespace Products.Infrastructure.MapProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductModel, ProductEntity>().ReverseMap();
    }
}