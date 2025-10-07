using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CraneCalc.API.Configurations;

public static class ControllerSwaggerConfig
{
    public static void AddSwaggerAndControllerConfig(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CraneCalc API", Version = "v1" });
            
            // Добавляем схему аутентификации
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            // Глобальное требование безопасности
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
            
            // Добавляем фильтр операций
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        
        // Важно: Добавляем CORS если ещё не добавлен
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }
}

// Исправленный фильтр операций
public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Проверяем есть ли атрибут AllowAnonymous
        var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType!.GetCustomAttributes(true))
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (hasAllowAnonymous)
        {
            return; // Не требуем аутентификацию для методов с AllowAnonymous
        }

        // Проверяем есть ли атрибут Authorize
        var hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType!.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .Any();

        if (hasAuthorize)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }
}