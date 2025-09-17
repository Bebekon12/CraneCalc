using System.Text.Json;
using CraneCalc.Application.Features.Cargo.Commands.CreateCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;
using CraneCalc.Application.Features.Shared;
using CraneCalc.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;

namespace CraneCalc.API.Extensions;

public static class MediatrValidatorExtensions
{
    public static void AddMediatrValidators(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetCargosPaginatedQuery).Assembly));
        services.AddValidatorsFromAssemblyContaining<CreateCargoCommandValidator>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    public static void AddUseExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                switch (exception)
                {
                    case ValidationException validationException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            Status = 400,
                            Message = "Validation errors",
                            Errors = validationException.Errors
                                .Select(e => new { e.PropertyName, e.ErrorMessage })
                        }));
                        break;
                    case EntityException accountException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            Status = 400,
                            accountException.Message
                        }));
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            Status = 500,
                            Message = "An internal server error occurred."
                        }));
                        break;
                }
            });
        });
    }
}