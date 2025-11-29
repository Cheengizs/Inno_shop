using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Clients;
using Users.Application.DTOs;
using Users.Application.MapProfiles;
using Users.Application.RepositoryInterfaces;
using Users.Application.Services;
using Users.Application.Services.Interfaces;
using Users.Application.Validators;
using Users.Infrastructure.Clients;
using Users.Infrastructure.DbContexts;
using Users.Infrastructure.MapProfiles;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Services;

namespace Users.Api.Configuration;

public static class AddApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DbConnection"));
        });

        services.AddControllers();
        
        services.AddAutoMapper(cfg => { },
            typeof(UserProfile), 
            typeof(UserApplicationProfile));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTokenService, EmailTokenService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        
        
        services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["AppSettings:ProductService"]); 
        });
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
            });
        
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Введите 'Bearer' [пробел] и ваш токен.\r\n\r\nПример: \"Bearer eyJhbGciOiJIUzI1NiIsInR5...\""
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
        return services;
    }
}