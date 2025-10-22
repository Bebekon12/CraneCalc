using CraneCalc.API.Configurations;
using CraneCalc.Api.Middleware;

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
app.UseMiddleware<TokenBlacklistMiddleware>();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
