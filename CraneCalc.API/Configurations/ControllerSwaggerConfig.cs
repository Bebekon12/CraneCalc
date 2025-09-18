using CraneCalc.Application.Services;
using Microsoft.OpenApi.Models;

namespace CraneCalc.API.Configurations;

public static class ControllerSwaggerConfig
{
    public static void AddSwaggerAndControllerConfig(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers(options =>
        {
            options.Filters.Add<RefreshTokenFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
        });
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
    
            options.OperationFilter<AuthResponsesOperationFilter>();
        });
    }
}