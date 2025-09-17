using CraneCalc.API.Extensions;
using CraneCalc.Application.DtoModelMappers;
using CraneCalc.Application.Options;
using CraneCalc.Infrastructure;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddApiAuthentication(configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!);

services.AddOpenApi();
services.AddControllers();
services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });

    options.OperationFilter<AddAuthHeaderOperationFilter>();
});

services.AddMediatrValidators();

services.AddAutoMapper(
    typeof(EntityModelMappers).Assembly, 
    typeof(DtoModelMapper).Assembly);

services.AddDbContextExtensions(configuration);
services.AddRepositoryAndServices();
services.AddMinioExtension(configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.AddUseExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
