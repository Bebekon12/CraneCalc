using CraneCalc.API.Extensions;
using CraneCalc.Application.DtoModelMappers;
using CraneCalc.Infrastructure;
using CraneCalc.Infrastructure.EntityMappers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOpenApi();
services.AddControllers();
services.AddSwaggerGen();

services.AddAutoMapper(
    typeof(EntityModelMappers).Assembly, 
    typeof(DtoModelMapper).Assembly);

services.AddDbContextExtensions(configuration);
services.AddRepository();
services.AddMinioExtension(configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseRouting();

//authorize authentication

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
