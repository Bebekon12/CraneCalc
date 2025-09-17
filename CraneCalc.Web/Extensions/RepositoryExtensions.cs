using CraneCalc.Application.Interfaces;
using CraneCalc.Infrastructure.Repositories;

namespace CraneCalc.Web.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepository(this IServiceCollection services)
    {
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
    }
}