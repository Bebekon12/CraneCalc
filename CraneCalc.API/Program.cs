using CraneCalc.API.Configurations;
using CraneCalc.Application.DtoModelMappers;
using CraneCalc.Application.Options;
using CraneCalc.Application.Services;
using CraneCalc.Infrastructure;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddApiAuthentication(configuration);

services.AddSwaggerAndControllerConfig();
services.AddMediatrValidators();
services.AddDbContextExtensions(configuration);
services.AddRedisConfig(configuration);
services.AddRepositoryAndServices();
services.AddMinioExtension(configuration);

var app = builder.Build();

app.UseDbContextExtensions();
app.AddUseExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
