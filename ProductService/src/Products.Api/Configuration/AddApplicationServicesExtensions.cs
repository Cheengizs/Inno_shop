using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Products.Application.Clients;
using Products.Application.Profiles;
using Products.Application.RepositoryInterfaces;
using Products.Application.Services;
using Products.Application.Services.Interfaces;
using Products.Application.Validators;
using Products.Infrastructure.Clients;
using Products.Infrastructure.DbContexts;
using Products.Infrastructure.MapProfiles;
using Products.Infrastructure.Repositories;

namespace Products.Api.Configuration;

public static class AddApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProductsDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DbConnection"), 
                sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null));
        });
        
        services.AddControllers();
        
        services.AddAutoMapper(cfg => { },
            typeof(ProductProfile),
            typeof(ProductApplicationProfile));

        services.AddValidatorsFromAssemblyContaining<ProductValidator>();

        services.AddScoped<IUserServiceClient, UserServiceClient>();
        
        services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000/"); 
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        
        return services;
    }
}