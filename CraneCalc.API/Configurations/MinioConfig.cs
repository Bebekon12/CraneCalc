using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Services;
using Minio;

namespace CraneCalc.API.Configurations;

public static class MinioConfig
{
    public static void AddMinioExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMinio(configureClient => 
        {
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            var useSsl = bool.Parse(configuration["Minio:UseSSL"] ?? "false");

            configureClient
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSsl);
        });
        
        services.AddScoped<IFileStorageService, FileStorageService>();
    }
}