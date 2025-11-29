using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; 
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

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; 
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                
                ClockSkew = TimeSpan.Zero 
            };
        });

        services.AddAuthorization();

        services.AddControllers();
        
        services.AddAutoMapper(cfg => { },
            typeof(ProductProfile),
            typeof(ProductApplicationProfile));

        services.AddValidatorsFromAssemblyContaining<ProductValidator>();

        services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
        {
            var userServiceUrl = configuration["ServiceUrls:UserService"] ?? "http://localhost:5000/";
            client.BaseAddress = new Uri(userServiceUrl); 
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        
        return services;
    }
}