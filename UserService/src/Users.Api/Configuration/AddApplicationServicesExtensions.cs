using Microsoft.EntityFrameworkCore;
using Users.Application.RepositoryInterfaces;
using Users.Infrastructure.DbContexts;
using Users.Infrastructure.MapProfiles;
using Users.Infrastructure.Repositories;

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
            typeof(UserProfile));
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}