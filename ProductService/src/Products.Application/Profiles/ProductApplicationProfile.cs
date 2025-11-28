using AutoMapper;
using Products.Application.DTOs.Products;
using Products.Domain.Models;

namespace Products.Application.Profiles;

public class ProductApplicationProfile : Profile
{
    public ProductApplicationProfile()
    {
        CreateMap<ProductModel, ProductRequest>().ReverseMap();
        CreateMap<ProductModel, ProductResponse>().ReverseMap();
        
    }
}