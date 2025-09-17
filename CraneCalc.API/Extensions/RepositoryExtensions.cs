using CraneCalc.Application.Interfaces;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Infrastructure.Repositories;

namespace CraneCalc.API.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepository(this IServiceCollection services)
    {
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartCargoRepository, CartCargoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}