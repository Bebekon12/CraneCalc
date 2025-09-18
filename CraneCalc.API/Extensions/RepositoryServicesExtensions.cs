using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Services;
using CraneCalc.Infrastructure.Repositories;

namespace CraneCalc.API.Extensions;

public static class RepositoryServicesExtensions
{
    public static void AddRepositoryAndServices(this IServiceCollection services)
    {
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartCargoRepository, CartCargoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }
}