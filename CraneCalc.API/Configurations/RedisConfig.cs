using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Services;

namespace CraneCalc.API.Configurations;

public static class RedisConfig
{
    public static void AddRedisConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        
        services.AddScoped<ITokenStorage, RedisTokenStorage>();
    }
}