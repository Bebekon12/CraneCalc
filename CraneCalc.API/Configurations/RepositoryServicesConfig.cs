using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Services;
using CraneCalc.Infrastructure.Repositories;

namespace CraneCalc.API.Configurations;

public static class RepositoryServicesConfig
{
    public static void AddRepositoryAndServices(this IServiceCollection services)
    {
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<ICraneOrderRepository, CraneOrderRepository>();
        services.AddScoped<ICraneCargoRepository, CraneCargoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHttpClient<ICraneCalculationService, CraneCalculationService>();
    }
}